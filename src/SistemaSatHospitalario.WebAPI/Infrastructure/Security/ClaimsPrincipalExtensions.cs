using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SistemaSatHospitalario.WebAPI.Infrastructure.Security
{
    /// <summary>
    /// [PHASE-2] Senior Extensions for identity management.
    /// Simplifies data extraction from JWT claims, reducing technical debt.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ?? "Sistama";
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                   user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? 
                   Guid.Empty.ToString();
        }

        public static bool IsPrivileged(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin") || 
                   user.IsInRole("Administrador") || 
                   user.IsInRole("Supervisor");
        }

        public static string GetCajeroName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ?? 
                   user.FindFirstValue(JwtRegisteredClaimNames.Email) ?? 
                   "Cajero Desconocido";
        }
    }
}
