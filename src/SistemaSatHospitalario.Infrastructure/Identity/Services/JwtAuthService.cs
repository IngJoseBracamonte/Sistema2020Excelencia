using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaSatHospitalario.Core.Domain.DTOs;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;
using Serilog;

namespace SistemaSatHospitalario.Infrastructure.Identity.Services
{
    public class JwtAuthService : IAuthService
    {
        private readonly UserManager<UsuarioHospital> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;

        public JwtAuthService(
            UserManager<UsuarioHospital> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<JwtAuthResult?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            try 
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    Log.Warning("Intento de login fallido: Usuario no encontrado. Username: {Username}", username);
                    return null;
                }

                if (!user.EsActivo)
                {
                    Log.Warning("Intento de login fallido: Usuario desactivado. Username: {Username}", username);
                    return null;
                }

                var result = await _userManager.CheckPasswordAsync(user, password);

                if (!result && !user.RequirePasswordReset)
                {
                    Log.Warning("Intento de login fallido: Contraseña incorrecta. Username: {Username}", username);
                    return null;
                }

                Log.Information("Login exitoso. Username: {Username}", username);

                var roles = await _userManager.GetRolesAsync(user);
                var allPermissions = new List<string>();

                // 1. Permissions from ROLES
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        var rolePerms = roleClaims.Where(c => c.Type == PermissionConstants.Type).Select(c => c.Value);
                        allPermissions.AddRange(rolePerms);
                    }
                }

                // 2. Direct user claims (Claims Granulares Directos)
                var userClaims = await _userManager.GetClaimsAsync(user);
                var userPerms = userClaims.Where(c => c.Type == PermissionConstants.Type).Select(c => c.Value);
                allPermissions.AddRange(userPerms);

                // Deduplicate permissions
                allPermissions = allPermissions.Distinct().ToList();

                var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"] ?? "SuperSecretKeyHospitalario2026_Excelencia_V15_System_Token_Validation_Key!");
                var tokenHandler = new JwtSecurityTokenHandler();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim("NombreReal", user.NombreReal ?? ""),
                    new Claim("ApellidoReal", user.ApellidoReal ?? ""),
                    new Claim("LegacyCajeroId", user.LegacyCajeroId?.ToString() ?? "0"),
                    new Claim("RequirePasswordReset", user.RequirePasswordReset.ToString().ToLower())
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                foreach (var perm in allPermissions)
                {
                    claims.Add(new Claim(PermissionConstants.Type, perm));
                }

                var expiration = DateTime.UtcNow.AddHours(12);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiration,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI",
                    Audience = _configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA"
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new JwtAuthResult
                {
                    Token = tokenHandler.WriteToken(token),
                    Expiration = expiration,
                    UserId = user.Id,
                    Username = user.UserName ?? username,
                    Role = roles.Count > 0 ? roles[0] : "AsignarRol",
                    Permissions = allPermissions,
                    RequirePasswordReset = user.RequirePasswordReset
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ERROR CRÍTICO durante la autenticación del usuario {Username}", username);
                throw new InvalidOperationException($"Error durante la autenticación: {ex.Message}.", ex);
            }
        }
    }
}
