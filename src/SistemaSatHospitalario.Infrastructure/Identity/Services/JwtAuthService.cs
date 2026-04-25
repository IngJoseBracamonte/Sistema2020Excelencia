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
                if (user == null || !user.EsActivo) return null;

                var result = await _userManager.CheckPasswordAsync(user, password);
                
                // Pachón Pro V14.0: Emergency Bypass for Approved Resets
                // If the user forgot their password but an admin already approved the reset,
                // we allow them to "login" only to be forced into the Change Password screen.
                if (!result && !user.RequirePasswordReset) return null;
                
                // If password check failed but RequirePasswordReset is true, we still let them through
                // but they won't have a fully functional session until they complete the reset.

                var roles = await _userManager.GetRolesAsync(user);
                
                // --- FETCH PERMISSIONS (Role Claims + User Claims) ---
                var allPermissions = new List<string>();

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
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyStr = _configuration["JwtConfig:Secret"];
                if (string.IsNullOrEmpty(keyStr) || keyStr.Length < 32)
                    throw new InvalidOperationException("Revise JwtConfig:Secret en appsettings. Debe tener al menos 32 caracteres.");

                var key = Encoding.ASCII.GetBytes(keyStr);
                var expiration = DateTime.UtcNow.AddHours(8); 

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? "unknown")
                };
                
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                foreach (var permission in allPermissions)
                {
                    claims.Add(new Claim(PermissionConstants.Type, permission));
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
                    Role = roles.Count > 0 ? roles[0] : "AsignarRol",
                    Permissions = allPermissions,
                    RequirePasswordReset = user.RequirePasswordReset
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error durante la autenticación: {ex.Message}.", ex);
            }
        }
    }
}
