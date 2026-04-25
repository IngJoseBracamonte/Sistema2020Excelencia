using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.Core.Domain.Constants;

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

                _logger.LogInformation("Poblando Identity Roles, Usuarios y Permisos Defaults (Arquitectura V15.0)...");

                // --- 1. SEED ROLES ---
                var roles = new[] { 
                    "Admin", "Cajero", "Supervisor", "Asistente de Seguros", 
                    "Médico", "Asistente Particular", "Asistente RX", "Asistente de Tomografía" 
                };

                foreach (var roleName in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
                    }
                }

                // --- 2. DEFINICIÓN DE PERMISOS PREDETERMINADOS (DICCIONARIO DE ARQUITECTURA) ---
                var roleDefaults = new System.Collections.Generic.Dictionary<string, string[]>
                {
                    { "Admin", new[] { 
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Facturacion.View, PermissionConstants.Facturacion.Create, PermissionConstants.Facturacion.Cancel, PermissionConstants.Facturacion.Convenio, PermissionConstants.Facturacion.Particular,
                        PermissionConstants.Citas.ViewControl, PermissionConstants.Citas.Manage, PermissionConstants.Citas.Cancel, PermissionConstants.Citas.Atender,
                        PermissionConstants.Reportes.ViewCxC, PermissionConstants.Reportes.ViewExpediente, PermissionConstants.Reportes.ViewAudit, PermissionConstants.Reportes.ViewOrders,
                        PermissionConstants.Admin.AccessSettings, PermissionConstants.Admin.ManageUsers, PermissionConstants.Admin.ManageCatalog, PermissionConstants.Admin.ManageMedicos, PermissionConstants.Admin.ManageTasa
                    } },
                    { "Cajero", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Facturacion.View, PermissionConstants.Facturacion.Create, PermissionConstants.Facturacion.Particular,
                        PermissionConstants.Citas.ViewControl,
                        PermissionConstants.Reportes.ViewExpediente
                    } },
                    { "Supervisor", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Facturacion.View, PermissionConstants.Facturacion.Create, PermissionConstants.Facturacion.Cancel, PermissionConstants.Facturacion.Convenio, PermissionConstants.Facturacion.Particular,
                        PermissionConstants.Citas.ViewControl, PermissionConstants.Citas.Manage, PermissionConstants.Citas.Cancel,
                        PermissionConstants.Reportes.ViewCxC, PermissionConstants.Reportes.ViewExpediente, PermissionConstants.Reportes.ViewAudit, PermissionConstants.Reportes.ViewOrders,
                        PermissionConstants.Admin.AccessSettings, PermissionConstants.Admin.ManageCatalog, PermissionConstants.Admin.ManageMedicos
                    } },
                    { "Asistente de Seguros", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Facturacion.View, PermissionConstants.Facturacion.Create, PermissionConstants.Facturacion.Convenio,
                        PermissionConstants.Citas.ViewControl,
                        PermissionConstants.Reportes.ViewCxC, PermissionConstants.Reportes.ViewExpediente
                    } },
                    { "Médico", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Citas.ViewControl, PermissionConstants.Citas.Atender,
                        PermissionConstants.Reportes.ViewOrders
                    } },
                    { "Asistente Particular", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Facturacion.View, PermissionConstants.Facturacion.Create, PermissionConstants.Facturacion.Particular,
                        PermissionConstants.Citas.ViewControl,
                        PermissionConstants.Reportes.ViewExpediente
                    } },
                    { "Asistente RX", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Reportes.ViewOrders,
                        PermissionConstants.Citas.ViewControl
                    } },
                    { "Asistente de Tomografía", new[] {
                        PermissionConstants.Dashboard.View,
                        PermissionConstants.Reportes.ViewOrders
                    } }
                };

                // --- 3. SEED TEST USERS (USER-CENTRIC PERMISSIONS) ---
                var testUsers = new[] {
                    new { User = "admin", Role = "Admin", Email = "admin@hospital.local", Pwd = "Admin123*!" },
                    new { User = "user_cajero", Role = "Cajero", Email = "cajero@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_supervisor", Role = "Supervisor", Email = "supervisor@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_seguros", Role = "Asistente de Seguros", Email = "seguros@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_medico", Role = "Médico", Email = "medico@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_particular", Role = "Asistente Particular", Email = "particular@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_rx", Role = "Asistente RX", Email = "rx@test.local", Pwd = "Hospital2026*!" },
                    new { User = "user_tomografia", Role = "Asistente de Tomografía", Email = "tomografia@test.local", Pwd = "Hospital2026*!" }
                };

                foreach (var userData in testUsers)
                {
                    var user = await _userManager.FindByNameAsync(userData.User);
                    if (user == null)
                    {
                        user = new UsuarioHospital
                        {
                            UserName = userData.User,
                            Email = userData.Email,
                            NombreReal = userData.User.Replace("user_", "").ToUpper(),
                            ApellidoReal = "Personal",
                            EmailConfirmed = true,
                            EsActivo = true
                        };

                        var result = await _userManager.CreateAsync(user, userData.Pwd);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, userData.Role);
                            
                            // [USER-CENTRIC] Assign permissions directly to the user as Claims
                            if (roleDefaults.TryGetValue(userData.Role, out var permissions))
                            {
                                foreach (var perm in permissions)
                                {
                                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(PermissionConstants.Type, perm));
                                }
                            }
                        }
                    }
                    else
                    {
                        // [V15.1 Patch] Asegurar que la contraseña coincida con el seeder para pruebas de automatización
                        var passToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, passToken, userData.Pwd);

                        // Asegurar que el usuario tenga su rol y sus permisos base si no los tiene
                        if (!await _userManager.IsInRoleAsync(user, userData.Role))
                        {
                            await _userManager.AddToRoleAsync(user, userData.Role);
                        }

                        // Sincronización proactiva de permisos (V15.4 Robust Sync)
                        var userClaims = await _userManager.GetClaimsAsync(user);
                        var currentPermissionClaims = userClaims.Where(c => c.Type == PermissionConstants.Type).ToList();
                        
                        if (roleDefaults.TryGetValue(userData.Role, out var targetPermissions))
                        {
                            // Si el número de permisos cambió o hay permisos faltantes, sincronizamos
                            var currentPermStrings = currentPermissionClaims.Select(c => c.Value).ToList();
                            bool needsSync = targetPermissions.Any(p => !currentPermStrings.Contains(p)) || 
                                             currentPermStrings.Any(p => !targetPermissions.Contains(p));

                            if (needsSync)
                            {
                                _logger.LogInformation("Sincronizando permisos para el usuario {User} (Rol: {Role})...", userData.User, userData.Role);
                                
                                // Eliminar permisos actuales de tipo Permission
                                foreach (var claim in currentPermissionClaims)
                                {
                                    await _userManager.RemoveClaimAsync(user, claim);
                                }

                                // Agregar los nuevos permisos por defecto
                                foreach (var perm in targetPermissions)
                                {
                                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(PermissionConstants.Type, perm));
                                }
                            }
                        }
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
