using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.UnitTests.Common
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        // En tu interfaz UserId es Guid?
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Role { get; set; }
        public List<string> Roles { get; set; } = new();

        public FakeCurrentUserService(
            Guid? userId = null,
            string userName = "usuario.prueba",
            string role = "Admin",
            bool isAuthenticated = true)
        {
            // Si no le pasas ID, genera uno por defecto para los tests
            UserId = userId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");
            UserName = userName;
            Role = role;
            IsAuthenticated = isAuthenticated;

            if (!string.IsNullOrEmpty(role))
            {
                Roles.Add(role);
            }
        }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role)) return false;
            return Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsAdmin()
        {
            return IsInRole("Admin") || IsInRole("Administrador");
        }
    }
}