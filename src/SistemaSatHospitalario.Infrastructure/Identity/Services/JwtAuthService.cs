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

        public async Task<JwtAuthResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
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
                bool isBypassMode = !result && user.RequirePasswordReset;

                if (!result && !isBypassMode)
                {
                    Log.Warning("Intento de login fallido: Contraseña incorrecta. Username: {Username}", username);
                    return null;
                }

                if (isBypassMode)
                {
                    Log.Information("Login mediante Bypass de Emergencia (Password Reset requerido). Username: {Username}", username);
                }
                else
                {
                    Log.Information("Login exitoso. Username: {Username}", username);
                }

                var roles = await _userManager.GetRolesAsync(user);
                var allPermissions = new List<string>();

                // Si es modo bypass, NO cargamos permisos para evitar escalada de privilegios
                if (!isBypassMode)
                {
                    // 1. Permissions from ROLES
                    foreach (var roleName in roles)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var roleClaims = await _roleManager.GetClaimsAsync(role);
                            var permissions = roleClaims
                                .Where(c => c.Type == PermissionConstants.Type)
                                .Select(c => c.Value);
                            allPermissions.AddRange(permissions);
                        }
                    }

                    // 2. Permissions directly on the USER (User Claims)
                    var userClaimsDirect = await _userManager.GetClaimsAsync(user);
                    var directPermissions = userClaimsDirect
                        .Where(c => c.Type == PermissionConstants.Type)
                        .Select(c => c.Value);
                    allPermissions.AddRange(directPermissions);

                    allPermissions = allPermissions.Distinct().ToList();
                }
                else
                {
                    // En modo bypass, solo permitimos una "permisión" mínima si fuera necesaria para la UI
                    allPermissions.Add("Identity.Password.Reset");
                }
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyStr = _configuration["JwtConfig:Secret"];
                if (string.IsNullOrEmpty(keyStr) || keyStr.Length < 32)
                    throw new InvalidOperationException("Revise JwtConfig:Secret en appsettings. Debe tener al menos 32 caracteres.");

                var key = Encoding.ASCII.GetBytes(keyStr);
                var expiration = isBypassMode ? DateTime.UtcNow.AddMinutes(15) : DateTime.UtcNow.AddHours(8); 

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? "unknown"),
                    new Claim("IsRestrictedSession", isBypassMode.ToString().ToLower())
                };
                
                if (!isBypassMode)
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    foreach (var permission in allPermissions)
                    {
                        claims.Add(new Claim(PermissionConstants.Type, permission));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
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
                    Username = user.UserName,
                    Role = isBypassMode ? "Restringido" : (roles.Count > 0 ? roles[0] : "AsignarRol"),
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
