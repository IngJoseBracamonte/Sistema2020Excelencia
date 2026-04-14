using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;

namespace SistemaSatHospitalario.Infrastructure.Identity.Seeds
{
    public class IdentityDbInitializer : IDatabaseInitializer
    {
        private readonly SatHospitalarioIdentityDbContext _context;
        private readonly UserManager<UsuarioHospital> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<IdentityDbInitializer> _logger;

        public IdentityDbInitializer(
            SatHospitalarioIdentityDbContext context,
            UserManager<UsuarioHospital> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<IdentityDbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Detectadas {Count} migraciones pendientes. Aplicando a Identity Database...", pendingMigrations.Count());
                    
                    // Senior Robust Fix: Abrir conexión manualmente para asegurar que el SET SESSION 
                    // persista durante toda la operación de MigrateAsync en Aiven.
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
                        _logger.LogInformation("Sincronizando historial de migraciones manualmente (Baseline: InitialIdentityMySql)...");
                        
                        // Aseguramos que la tabla de historial exista antes del insert
                        await _context.Database.ExecuteSqlRawAsync(
                            "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`)) CHARACTER SET=utf8mb4;");
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('20260414054517_InitialIdentityMySql', '9.0.2');");
                        
                        _logger.LogInformation("Sincronización de Baseline completada. El sistema puede continuar.");
                    }
                }
                else
                {
                    _logger.LogInformation("Identity Database ya está actualizada. No se requieren migraciones.");
                }

                _logger.LogInformation("Poblando Identity Roles y Usuarios Defaults...");

                // Seed Roles
                var roles = new[] { "Admin", "Cajero", "Supervisor", "Asistente de Seguros", "Médico", "Asistente Particular", "Asistente RX", "Asistente de Tomografía" };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                    }
                }

                // Seed Admin User
                if (_userManager.Users.All(u => u.UserName != "admin"))
                {
                    var defaultUser = new UsuarioHospital
                    {
                        UserName = "admin",
                        Email = "admin@hospital.local",
                        NombreReal = "Administrador",
                        ApellidoReal = "Maestro",
                        LegacyCajeroId = 1, 
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        EsActivo = true
                    };

                    var result = await _userManager.CreateAsync(defaultUser, "Admin123*!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(defaultUser, "Admin");
                    }
                }

                _logger.LogInformation("Identity Database Inicializada Correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando Identity Database.");
                throw;
            }
        }
    }
}
