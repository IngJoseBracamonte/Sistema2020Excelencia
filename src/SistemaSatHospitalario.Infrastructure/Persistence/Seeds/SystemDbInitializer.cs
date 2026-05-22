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
                    catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Conflicto detectado: Las tablas ya existen pero el historial de EF Core está ausente.");
                        _logger.LogInformation("Sincronizando historial de migraciones manualmente (Baseline: InitialSystemMySql)...");
                        
                        // Aseguramos que la tabla de historial exista antes del insert
                        await _context.Database.ExecuteSqlRawAsync(
                            "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`)) CHARACTER SET=utf8mb4;");
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('20260414054504_InitialSystemMySql', '9.0.2');");
                        
                        _logger.LogInformation("Sincronización de Baseline completada. El sistema puede continuar.");
                    }
                }
                else
                {
                    _logger.LogInformation("System Database ya está actualizada. No se requieren migraciones.");
                }

                _logger.LogInformation("Poblando System Database con datos de prueba...");

                await SeedEspecialidadesAsync();
                await SeedServiciosClinicosAsync();
                await SeedMedicosAsync();
                await SeedServiciosSugerenciasAsync();
                await SeedPacientesAsync();
                await SeedCajaDiariaAsync();
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                
                // Senior Maintenance Pattern: Asegurar integridad de fechas de recaudación
                await FixOrphanPaymentDatesAsync();
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
            var names = new[] { "Diagnóstico Diferencial", "Oncología", "Cardiología", "Pediatría", "Traumatología", "Ginecología" };

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
                (Name: "Lisa Cuddy", Speciality: "Ginecología")
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

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
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
                // Métodos de Pago
                new CatalogoMetodoPago("EFECTIVO DOLAR ($)", "Dolar Efectivo", true, false, 1),
                new CatalogoMetodoPago("ZELLE", "Zelle", true, false, 2),
                new CatalogoMetodoPago("USDT (BINANCE)", "USDT", true, false, 3),
                new CatalogoMetodoPago("PUNTO DE VENTA USD", "Punto Dolares", true, false, 4),
                new CatalogoMetodoPago("EFECTIVO (BS)", "Efectivo BS", false, false, 5),
                new CatalogoMetodoPago("PAGO MÓVIL", "Pago Movil", false, false, 6),
                new CatalogoMetodoPago("TRANSFERENCIA", "Transferencia", false, false, 7),
                new CatalogoMetodoPago("PUNTO DE VENTA BS", "Punto", false, false, 8),

                // Métodos de Vuelto
                new CatalogoMetodoPago("VUELTO EFECTIVO ($)", "Vuelto Efectivo USD", true, true, 1),
                new CatalogoMetodoPago("VUELTO PAGO MÓVIL (BS)", "Vuelto Pago Movil", false, true, 2),
                new CatalogoMetodoPago("VUELTO EFECTIVO (BS)", "Vuelto Efectivo BS", false, true, 3)
            };

            _context.CatalogoMetodosPago.AddRange(metodos);
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
