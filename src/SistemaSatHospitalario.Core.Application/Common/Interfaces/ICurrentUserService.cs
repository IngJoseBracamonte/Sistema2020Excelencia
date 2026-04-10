using System;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    /// <summary>
    /// Abstracción de seguridad para acceder a los datos del usuario actual 
    /// sin depender de HttpContext en la capa de Aplicación (Senior Design Pattern).
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool IsAdmin();
    }
}
