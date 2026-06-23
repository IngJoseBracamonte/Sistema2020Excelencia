using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using System.Linq;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Seeds
{
    public class SystemDbInitializer : IDatabaseInitializer
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ILogger<SystemDbInitializer> _logger;

        public SystemDbInitializer(
            SatHospitalarioDbContext context, 
            ILegacyLabRepository legacyRepository,
            ILogger<SystemDbInitializer> logger)
        {
            _context = context;
            _legacyRepository = legacyRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Detectadas {Count} migraciones pendientes. Aplicando a System Database...", pendingMigrations.Count());
                    
                    // Senior Robust Fix: Abrir conexión manualmente para asegurar que el SET SESSION 
                    // persista durante toda la operación de MigrateAsync.
                    var connection = _context.Database.GetDbConnection();
                    if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();
                    
                    await _context.Database.ExecuteSqlRawAsync("SET SESSION sql_require_primary_key = 0;");
                    
                    try 
                    {
                        await _context.Database.MigrateAsync();
                        _logger.LogInformation("Migraciones aplicadas con éxito.");
                    }
                    catch (Exception ex) when (
                        ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
                        ex.Message.Contains("ya existe", StringComparison.OrdinalIgnoreCase) ||
                        (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == 1050))
                    {
                        _logger.LogWarning("Conflicto detectado: Las tablas ya existen pero el historial de EF Core está ausente.");
                        _logger.LogInformation("Sincronizando historial de migraciones manualmente (Baseline: InitialApplication)...");
                        
                        // Aseguramos que la tabla de historial exista antes del insert
                        await _context.Database.ExecuteSqlRawAsync(
                            "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`)) CHARACTER SET=utf8mb4;");
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('20260522161430_InitialApplication', '10.0.5');");
                        
                        // Una vez insertado el baseline, reintentamos aplicar el resto de migraciones pendientes
                        await _context.Database.MigrateAsync();
                        _logger.LogInformation("Sincronización de Baseline completada y migraciones restantes aplicadas con éxito.");
                    }
                }
                else
                {
                    _logger.LogInformation("System Database ya está actualizada. No se requieren migraciones.");
                }

                // Self-healing: Ensure Direccion column exists in PacientesAdmision table (V12.1 Requirement)
                try
                {
                    bool hasDireccion = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(PacientesAdmision);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    if (reader["name"].ToString().Equals("Direccion", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasDireccion = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `PacientesAdmision` LIKE 'Direccion';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    hasDireccion = true;
                                }
                            }
                        }
                    }
                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }

                    if (!hasDireccion)
                    {
                        _logger.LogInformation("La columna 'Direccion' no existe en PacientesAdmision. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `PacientesAdmision` ADD COLUMN `Direccion` VARCHAR(500) NULL;");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear la columna 'Direccion' en PacientesAdmision.");
                }

                // Self-healing: Ensure PermiteFraccionamiento and UnidadMedida columns exist in ServiciosClinicos table
                try
                {
                    bool hasFraccionamiento = false;
                    bool hasUnidadMedida = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(ServiciosClinicos);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var colName = reader["name"].ToString();
                                    if (colName.Equals("PermiteFraccionamiento", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasFraccionamiento = true;
                                    }
                                    else if (colName.Equals("UnidadMedida", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasUnidadMedida = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `ServiciosClinicos`;";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"].ToString();
                                    if (field.Equals("PermiteFraccionamiento", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasFraccionamiento = true;
                                    }
                                    else if (field.Equals("UnidadMedida", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasUnidadMedida = true;
                                    }
                                }
                            }
                        }
                    }
                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }

                    if (!hasFraccionamiento)
                    {
                        _logger.LogInformation("La columna 'PermiteFraccionamiento' no existe en ServiciosClinicos. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `ServiciosClinicos` ADD COLUMN `PermiteFraccionamiento` TINYINT(1) NOT NULL DEFAULT 0;");
                    }

                    if (!hasUnidadMedida)
                    {
                        _logger.LogInformation("La columna 'UnidadMedida' no existe en ServiciosClinicos. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `ServiciosClinicos` ADD COLUMN `UnidadMedida` VARCHAR(50) NULL;");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear las columnas 'PermiteFraccionamiento' y 'UnidadMedida' en ServiciosClinicos.");
                }

                _logger.LogInformation("Poblando System Database con datos de prueba...");

                await SeedEspecialidadesAsync();
                await SeedServiciosClinicosAsync();
                await SeedMedicosAsync();
                await SeedServiciosSugerenciasAsync();
                await SeedHonorariosMedicosServiciosAsync();
                await SeedPacientesAsync();
                await SeedCajaDiariaAsync();
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                await SeedConveniosAsync();
                
                // Senior Maintenance Pattern: Asegurar integridad de fechas de recaudación
                await FixOrphanPaymentDatesAsync();
                await SeedMonedasAsync();
                await SeedMetodosPagoAsync();
                await SeedHonorarioConfigAsync();

                _logger.LogInformation("System Database Inicializada Correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando System Database.");
                throw;
            }
        }

        private async Task SeedConveniosAsync()
        {
            if (await _context.SegurosConvenios.AnyAsync()) return;

            _logger.LogInformation("Sembrando convenios de seguros por defecto...");

            var convenios = new List<SeguroConvenio>
            {
                new SeguroConvenio("PDVSA", "RTN-5", "Av. Principal PDVSA", "0212-1234567", "pdvsa@test.com"),
                new SeguroConvenio("Seguros Caracas", "RTN-CARACAS", "Centro Seguros Caracas", "0212-7654321", "caracas@test.com"),
                new SeguroConvenio("Mercantil Seguros", "RTN-MERCANTIL", "Torre Mercantil Seguros", "0212-9999999", "mercantil@test.com"),
                new SeguroConvenio("Sanitas", "RTN-SANITAS", "Las Mercedes", "0212-8888888", "sanitas@test.com")
            };

            _context.SegurosConvenios.AddRange(convenios);
            await _context.SaveChangesAsync();
        }

        private async Task SeedServiciosClinicosAsync()
        {
            var defaults = new List<ServicioClinico>
            {
                new ServicioClinico("S001", "Consulta Medica General", 30.00m, "Consulta") { HonorariumCategory = "CONSULTA" },
                new ServicioClinico("S002", "Radiografía Tórax", 45.00m, "RX") { HonorariumCategory = "RX" },
                new ServicioClinico("S003", "Informe Médico Especializado", 15.00m, "Informe") { HonorariumCategory = "INFORME" },
                new ServicioClinico("S004", "Consulta Ginecologica", 60.00m, "Consulta") { HonorariumCategory = "CONSULTA" },
                new ServicioClinico("S005", "Citologia", 25.00m, "Citologia") { HonorariumCategory = "CITOLOGIA" },
                new ServicioClinico("S006", "Eco Ginecologico", 40.00m, "Eco") { HonorariumCategory = "INFORME" }
            };

            foreach (var s in defaults)
            {
                var existing = await _context.ServiciosClinicos.FirstOrDefaultAsync(x => x.Codigo == s.Codigo);
                if (existing == null)
                {
                    _context.ServiciosClinicos.Add(s);
                }
                else if (string.IsNullOrEmpty(existing.HonorariumCategory))
                {
                    // Senior Maintenance: Actualizamos servicios existentes sin categoría (Migración Pro)
                    existing.HonorariumCategory = s.HonorariumCategory;
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();

            // Link Consulta Ginecologica with specialty Ginecología
            var specGine = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == "Ginecología");
            var consultaGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S004");
            if (specGine != null && consultaGine != null && consultaGine.EspecialidadId != specGine.Id)
            {
                consultaGine.SetEspecialidad(specGine.Id);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedEspecialidadesAsync()
        {
            var names = new[] { "Diagnóstico Diferencial", "Oncología", "Cardiología", "Pediatría", "Traumatología", "Ginecología", "Imagenología" };

            foreach (var name in names)
            {
                if (!await _context.Especialidades.AnyAsync(e => e.Nombre == name))
                {
                    _context.Especialidades.Add(new Especialidad(name));
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedMedicosAsync()
        {
            var medicDefaults = new[]
            {
                (Name: "Gregory House", Speciality: "Diagnóstico Diferencial"),
                (Name: "James Wilson", Speciality: "Oncología"),
                (Name: "Stephen Strange", Speciality: "Cardiología"),
                (Name: "Patch Adams", Speciality: "Pediatría"),
                (Name: "John Watson", Speciality: "Traumatología"),
                (Name: "Lisa Cuddy", Speciality: "Ginecología"),
                (Name: "José Bracamonte", Speciality: "Imagenología"),
                (Name: "María Gutiérrez", Speciality: "Imagenología")
            };

            foreach (var m in medicDefaults)
            {
                if (!await _context.Medicos.AnyAsync(x => x.Nombre == m.Name))
                {
                    var spec = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == m.Speciality);
                    if (spec != null)
                    {
                        _context.Medicos.Add(new Medico(m.Name, spec.Id));
                    }
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedServiciosSugerenciasAsync()
        {
            var consultaGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S004");
            var citologia = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S005");
            var ecoGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S006");
            var rxTorax = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S002");
            var informeMedico = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S003");

            if (consultaGine != null && citologia != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == consultaGine.Id && ss.ServicioSugeridoId == citologia.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(consultaGine.Id, citologia.Id));
                }
            }

            if (consultaGine != null && ecoGine != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == consultaGine.Id && ss.ServicioSugeridoId == ecoGine.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(consultaGine.Id, ecoGine.Id));
                }
            }

            if (rxTorax != null && informeMedico != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == rxTorax.Id && ss.ServicioSugeridoId == informeMedico.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(rxTorax.Id, informeMedico.Id));
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedHonorariosMedicosServiciosAsync()
        {
            var jose = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre == "José Bracamonte");
            var maria = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre == "María Gutiérrez");
            var informe = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S003");

            if (informe != null)
            {
                if (jose != null)
                {
                    var exists = await _context.HonorariosMedicosServicios.AnyAsync(h => h.ServicioId == informe.Id && h.MedicoId == jose.Id);
                    if (!exists)
                    {
                        _context.HonorariosMedicosServicios.Add(new HonorarioMedicoServicio(informe.Id, jose.Id, 10.00m, "System"));
                    }
                }

                if (maria != null)
                {
                    var exists = await _context.HonorariosMedicosServicios.AnyAsync(h => h.ServicioId == informe.Id && h.MedicoId == maria.Id);
                    if (!exists)
                    {
                        _context.HonorariosMedicosServicios.Add(new HonorarioMedicoServicio(informe.Id, maria.Id, 8.00m, "System"));
                    }
                }

                if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
            }
        }

        private async Task SeedPacientesAsync()
        {
            if (!await _context.PacientesAdmision.AnyAsync())
            {
                _logger.LogInformation("Sincronizando pacientes desde Sistema Legacy (Concatenación V11.6)...");
                
                try 
                {
                    // Obtener una muestra de pacientes recientes del legacy para poblar el baseline
                    var legacyPatients = await _legacyRepository.SearchPatientsLimitedAsync("A", default); // Buscar genérica para traer iniciales
                    
                    if (legacyPatients != null && legacyPatients.Any())
                    {
                        foreach (var lp in legacyPatients.Take(100)) // Limitamos a 100 para el seed inicial
                        {
                            var fullName = $"{lp.Nombre} {lp.Apellidos}".Trim();
                            var mainPhone = !string.IsNullOrEmpty(lp.Celular) ? lp.Celular : lp.Telefono;
                            
                            var nativePatient = new PacienteAdmision(lp.Cedula, fullName, mainPhone ?? "", lp.IdPersona);
                            _context.PacientesAdmision.Add(nativePatient);
                        }
                        
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Sincronización inicial completada con éxito.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo completar la sincronización legacy durante el seed. Se usarán datos estáticos de respaldo.");
                    
                    _context.PacientesAdmision.AddRange(
                        new PacienteAdmision("0999999999", "John Doe (Backup)", "0987654321", 1),
                        new PacienteAdmision("0888888888", "Jane Smith (Backup)", "0912345678", 2)
                    );
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task SeedCajaDiariaAsync()
        {
            // Evitar duplicar cajas si el sistema se reinicia (Check por NombreUsuario 'admin')
            if (!await _context.CajasDiarias.AnyAsync(c => c.NombreUsuario == "admin"))
            {
                _context.CajasDiarias.Add(
                    new CajaDiaria(50.00m, 1500.00m, "1", "admin")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedConfiguracionAsync()
        {
            if (!await _context.ConfiguracionGeneral.AnyAsync())
            {
                _context.ConfiguracionGeneral.Add(
                    new ConfiguracionGeneral("SAT HOSPITALARIO - EXCELENCIA", "J-12345678-9", 16.00m, "1234", false, false)
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedTasaCambioAsync()
        {
            // Solo sembramos si no hay NINGUNA tasa definida.
            if (!await _context.TasaCambio.AnyAsync())
            {
                _logger.LogInformation("Sembrando tasa de cambio inicial (Baseline 36.50)...");
                _context.TasaCambio.Add(new TasaCambio(36.50m));
                await _context.SaveChangesAsync();
            }
        }

        private async Task FixOrphanPaymentDatesAsync()
        {
            try 
            {
                // Corregimos cualquier pago con fecha default (0001-01-01) igualando a la del recibo padre.
                // Esto asegura que el Dashboard sea preciso con datos anteriores al cambio de esquema.
                int affected = await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE DetallesPago d JOIN RecibosFacturas r ON d.ReciboFacturaId = r.Id SET d.FechaPago = r.FechaEmision WHERE d.FechaPago = '0001-01-01 00:00:00'"
                );

                if (affected > 0)
                {
                    _logger.LogInformation($"Auto-Mantenimiento: Se corrigieron {affected} fechas de recaudación históricas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "El auto-mantenimiento de fechas de pago falló. El Dashboard podría mostrar datos incompletos transitoriamente.");
            }
        }
        private async Task SeedMetodosPagoAsync()
        {
            if (await _context.CatalogoMetodosPago.AnyAsync()) return;

            _logger.LogInformation("Sembrando catálogo de métodos de pago y vueltos...");

            var metodos = new List<CatalogoMetodoPago>
            {
                // Métodos de Pago (GrupoMoneda: 1 = USD, 2 = VES)
                new CatalogoMetodoPago("EFECTIVO DOLAR ($)", "Dolar Efectivo", 1, false, 1),
                new CatalogoMetodoPago("ZELLE", "Zelle", 1, false, 2),
                new CatalogoMetodoPago("USDT (BINANCE)", "USDT", 1, false, 3),
                new CatalogoMetodoPago("PUNTO DE VENTA USD", "Punto Dolares", 1, false, 4),
                new CatalogoMetodoPago("EFECTIVO (BS)", "Efectivo BS", 2, false, 5),
                new CatalogoMetodoPago("PAGO MÓVIL", "Pago Movil", 2, false, 6),
                new CatalogoMetodoPago("TRANSFERENCIA", "Transferencia", 2, false, 7),
                new CatalogoMetodoPago("PUNTO DE VENTA BS", "Punto", 2, false, 8),

                // Métodos de Vuelto
                new CatalogoMetodoPago("VUELTO EFECTIVO ($)", "Vuelto Efectivo USD", 1, true, 1),
                new CatalogoMetodoPago("VUELTO PAGO MÓVIL (BS)", "Vuelto Pago Movil", 2, true, 2),
                new CatalogoMetodoPago("VUELTO EFECTIVO (BS)", "Vuelto Efectivo BS", 2, true, 3)
            };

            _context.CatalogoMetodosPago.AddRange(metodos);
            await _context.SaveChangesAsync();
        }
        private async Task SeedMonedasAsync()
        {
            if (await _context.Monedas.AnyAsync()) return;

            _logger.LogInformation("Sembrando monedas en la base de datos...");

            var monedas = new List<Moneda>
            {
                new Moneda(1, "USD", "Dólar", "$", true),
                new Moneda(2, "VES", "Bolívar", "Bs.", false),
                new Moneda(3, "EUR", "Euro", "€", false),
                new Moneda(4, "COP", "Peso Colombiano", "COP$", false),
                new Moneda(5, "ARS", "Peso Argentino", "ARS$", false)
            };

            _context.Monedas.AddRange(monedas);
            await _context.SaveChangesAsync();
        }

        private async Task SeedHonorarioConfigAsync()
        {
            if (await _context.HonorariosConfig.AnyAsync()) return;

            _logger.LogInformation("Sembrando configuración inicial de honorarios...");

            // Intentamos obtener un médico por defecto para el seed (Gregory House si existe)
            var medicoDefault = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre.Contains("House"));
            var usuario = "Sistema";

            var categories = new[] { "CONSULTA", "RX", "INFORME", "CITOLOGIA" };

            foreach (var cat in categories)
            {
                var conf = new HonorarioConfig(cat, usuario);
                if (medicoDefault != null) conf.AsignarMedicoDefault(medicoDefault.Id, usuario, "Auto-asignado por Initializer");
                _context.HonorariosConfig.Add(conf);
            }

            // También sembramos las reglas de mapeo para fallback (compatibilidad con Sistema 2020 si aplicara)
            if (!await _context.HonorariumMappingRules.AnyAsync())
            {
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("RX", "RX", MappingRuleType.Contains, 1, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("RADI", "RX", MappingRuleType.Contains, 2, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("INFO", "INFORME", MappingRuleType.Contains, 3, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("CONS", "CONSULTA", MappingRuleType.Contains, 4, usuario));
            }

            await _context.SaveChangesAsync();
        }
    }
}
