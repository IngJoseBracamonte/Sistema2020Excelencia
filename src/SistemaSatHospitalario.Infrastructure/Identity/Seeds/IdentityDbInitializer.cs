using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SistemaSatHospitalario.Infrastructure.Identity.Models;

namespace SistemaSatHospitalario.Infrastructure.Identity.Seeds
{
    public static class IdentityDbInitializer
    {
        public static async Task SeedAsync(UserManager<UsuarioHospital> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Seed Roles
            var roles = new[] { "Admin", "Asistente Particular", "Asistente Seguro", "Asistente RX", "Supervisor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }

            // Seed Admin User
            if (userManager.Users.All(u => u.UserName != "admin"))
            {
                var defaultUser = new UsuarioHospital
                {
                    UserName = "admin@hospital.local",
                    Email = "admin@hospital.local",
                    NombreReal = "Administrador",
                    ApellidoReal = "Maestro",
                    LegacyCajeroId = 1, // Ejemplo de mapeo con DB Legacy
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(defaultUser, "Admin123*!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "Admin");
                }
            }
        }
    }
}
