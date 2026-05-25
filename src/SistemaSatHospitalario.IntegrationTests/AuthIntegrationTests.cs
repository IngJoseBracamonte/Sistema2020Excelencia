using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Domain.DTOs;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;
using System;
using System.Linq;

namespace SistemaSatHospitalario.IntegrationTests
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var username = "testuser";
            var password = "Password123!";
            await SeedUser(username, password, false);

            var command = new LoginCommand { Username = username, Password = password };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JwtAuthResult>();
            Assert.NotNull(result.Token);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var username = "wronguser";
            var command = new LoginCommand { Username = username, Password = "WrongPassword" };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithRequireReset_ReturnsOkAndRequireResetTrue()
        {
            // Arrange
            var client = _factory.CreateClient();
            var username = "resetuser";
            var password = "SomePassword123!";
            await SeedUser(username, password, true);

            // Must use the correct password now
            var command = new LoginCommand { Username = username, Password = password };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JwtAuthResult>();
            Assert.True(result.RequirePasswordReset);
            Assert.NotNull(result.Token);
        }

        private static readonly object _dbLock = new object();

        private async Task SeedUser(string username, string password, bool requireReset)
        {
            lock (_dbLock)
            {
                using var scope = _factory.Services.CreateScope();
                var identityDb = scope.ServiceProvider.GetRequiredService<SatHospitalarioIdentityDbContext>();
                var appDb = scope.ServiceProvider.GetRequiredService<SatHospitalarioDbContext>();
                
                identityDb.Database.EnsureCreated();
                appDb.Database.EnsureCreated();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioHospital>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                
                var adminRoleName = "Administrador";
                var roleExists = roleManager.RoleExistsAsync(adminRoleName).GetAwaiter().GetResult();
                if (!roleExists)
                {
                    var roleResult = roleManager.CreateAsync(new IdentityRole<Guid> 
                    { 
                        Name = adminRoleName,
                        NormalizedName = adminRoleName.ToUpper()
                    }).GetAwaiter().GetResult();
                    
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"[ROLE ERROR] {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }

                var user = userManager.FindByNameAsync(username).GetAwaiter().GetResult();
                if (user == null)
                {
                    user = new UsuarioHospital 
                    { 
                        UserName = username, 
                        Email = $"{username}@test.com",
                        NombreReal = "Test",
                        ApellidoReal = "User",
                        EsActivo = true,
                        RequirePasswordReset = requireReset
                    };
                    var result = userManager.CreateAsync(user, password).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, adminRoleName).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Console.WriteLine($"[USER ERROR] {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    user.RequirePasswordReset = requireReset;
                    userManager.UpdateAsync(user).GetAwaiter().GetResult();
                }
            }
        }
    }
}
