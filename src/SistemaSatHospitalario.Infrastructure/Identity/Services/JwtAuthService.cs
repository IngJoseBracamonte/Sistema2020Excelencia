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

namespace SistemaSatHospitalario.Infrastructure.Identity.Services
{
    public class JwtAuthService : IAuthService
    {
        private readonly UserManager<UsuarioHospital> _userManager;
        private readonly IConfiguration _configuration;

        public JwtAuthService(UserManager<UsuarioHospital> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<JwtAuthResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            try 
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null || !user.EsActivo) return null;

                var result = await _userManager.CheckPasswordAsync(user, password);
                if (!result) return null;

                var roles = await _userManager.GetRolesAsync(user);
                
                var tokenHandler = new JwtSecurityTokenHandler();
            var keyStr = _configuration["JwtConfig:Secret"];
            if (string.IsNullOrEmpty(keyStr) || keyStr.Length < 32)
                throw new InvalidOperationException("Revise JwtConfig:Secret en appsettings. Debe tener al menos 32 caracteres.");

            var key = Encoding.ASCII.GetBytes(keyStr);
            var expiration = DateTime.UtcNow.AddHours(8); // Turno típico de trabajo

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "unknown"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            // Si el user tiene rol inyectarlo en array (ej simplificado toma el 1ero)
            var userRole = roles.Count > 0 ? roles[0] : "AsignarRol";
            claims.Add(new Claim(ClaimTypes.Role, userRole));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtConfig:Issuer"],
                Audience = _configuration["JwtConfig:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtAuthResult
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration,
                UserId = user.Id,
                Username = user.UserName,
                Role = userRole
            };
            }
            catch (Exception ex)
            {
                // Registramos el error real en los logs para diagnóstico senior
                throw new InvalidOperationException($"Error durante la autenticación: {ex.Message}. Verifique la conexión a Base de Datos (MySql) y la secret JWT.", ex);
            }
        }
    }
}
