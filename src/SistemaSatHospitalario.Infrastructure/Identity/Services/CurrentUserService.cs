using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Infrastructure.Identity.Services
{
    /// <summary>
    /// Implementación concreta de ICurrentUserService basada en HttpContext.
    /// (V14.1 Senior Patch - Identity Decoupling)
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                return Guid.TryParse(userId, out var guid) ? guid : null;
            }
        }

        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("unique_name");

        public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("role");

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public bool IsAdmin()
        {
            return IsInRole(AuthorizationConstants.Admin);
        }
    }
}

// Nota: JwtRegisteredClaimNames puede requerir System.IdentityModel.Tokens.Jwt
