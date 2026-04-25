using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// Centraliza los nombres de roles para asegurar consistencia entre la Base de Datos, 
    /// la lógica de Aplicación y el Frontend. (Senior Pattern: Constants Layer)
    /// </summary>
    public static class AuthorizationConstants
    {
        // Roles Base (Sincronizados con DB)
        public const string Admin = "Admin";
        public const string Administrador = "Administrador"; // Nombre largo legado
        public const string Medico = "Médico";
        public const string Cajero = "Cajero";
        public const string Supervisor = "Supervisor";
        
        // Asistentes
        public const string AsistenteParticular = "Asistente Particular";
        public const string AsistenteSeguro = "Asistente Seguro";
        public const string AsistenteDeSeguros = "Asistente de Seguros"; // Variación detectada
        public const string AsistenteRX = "Asistente RX";
        public const string Farmacia = "Farmacia";

        // Grupos Lógicos (Para [Authorize(Roles = ...)])
        public const string AdminRoles = Admin + "," + Administrador;
        public const string AllStaff = Admin + "," + Administrador + "," + Cajero + "," + Supervisor;

        // Métodos de utilidad para validaciones proactivas
        public static bool IsAdmin(string? role) => 
            string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, Administrador, StringComparison.OrdinalIgnoreCase);

        public static bool IsCajero(string? role) => 
            IsAdmin(role) || 
            string.Equals(role, Cajero, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, AsistenteParticular, StringComparison.OrdinalIgnoreCase) || 
            string.Equals(role, AsistenteSeguro, StringComparison.OrdinalIgnoreCase);

        public static bool IsLaboratorio(string? role) => 
            IsAdmin(role) || 
            string.Equals(role, AsistenteRX, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, Farmacia, StringComparison.OrdinalIgnoreCase);
    }
}
