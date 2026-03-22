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
                _logger.LogInformation("Aplicando migraciones a Identity Database...");
                await _context.Database.MigrateAsync();

                _logger.LogInformation("Poblando Identity Roles y Usuarios Defaults...");

                // Seed Roles
                var roles = new[] { "Admin", "Médico", "Asistente Particular", "Asistente Seguro", "Asistente RX", "Supervisor" };

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
