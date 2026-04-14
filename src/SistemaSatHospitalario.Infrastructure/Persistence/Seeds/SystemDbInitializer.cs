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

                await SeedServiciosClinicosAsync();
                await SeedEspecialidadesAsync();
                await SeedMedicosAsync();
                await SeedPacientesAsync();
                await SeedCajaDiariaAsync();
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                
                // Senior Maintenance Pattern: Asegurar integridad de fechas de recaudación
                await FixOrphanPaymentDatesAsync();

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
            if (!await _context.ServiciosClinicos.AnyAsync())
            {
                _context.ServiciosClinicos.AddRange(
                    new ServicioClinico("S001", "Consulta Medica General", 30.00m, "Consulta"),
                    new ServicioClinico("S002", "Radiografía Tórax", 45.00m, "RX")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedEspecialidadesAsync()
        {
            if (!await _context.Especialidades.AnyAsync())
            {
                _context.Especialidades.AddRange(
                    new Especialidad("Diagnóstico Diferencial"),
                    new Especialidad("Oncología"),
                    new Especialidad("Cardiología"),
                    new Especialidad("Pediatría"),
                    new Especialidad("Traumatología")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedMedicosAsync()
        {
            if (!await _context.Medicos.AnyAsync())
            {
                var diagDif = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == "Diagnóstico Diferencial");
                var onco = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == "Oncología");

                if (diagDif != null && onco != null)
                {
                    _context.Medicos.AddRange(
                        new Medico("Gregory House", diagDif.Id),
                        new Medico("James Wilson", onco.Id)
                    );
                    await _context.SaveChangesAsync();
                }
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
            if (!await _context.CajasDiarias.AnyAsync())
            {
                // Un cajero genérico asumiendo un ID de de admin
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
                    new ConfiguracionGeneral("SAT HOSPITALARIO - EXCELENCIA", "J-12345678-9", 16.00m)
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedTasaCambioAsync()
        {
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
    }
}
