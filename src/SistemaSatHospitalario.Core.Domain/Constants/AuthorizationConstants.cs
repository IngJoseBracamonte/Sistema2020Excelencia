using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// Centraliza los nombres de roles para asegurar consistencia entre la Base de Datos, 
    /// la lógica de Aplicación y el Frontend. (Senior Pattern: Constants Layer)
    /// </summary>
    public static class AuthorizationConstants
    {
        public const string Admin = "Admin";
        public const string Medico = "Médico";
        public const string AsistenteParticular = "Asistente Particular";
        public const string AsistenteSeguro = "Asistente Seguro";
        public const string AsistenteRX = "Asistente RX";
        public const string Supervisor = "Supervisor";

        // Métodos de utilidad para validaciones proactivas
        public static bool IsAdmin(string? role) => 
            string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase);

        public static bool IsCajero(string? role) => 
            IsAdmin(role) || 
            string.Equals(role, AsistenteParticular, StringComparison.OrdinalIgnoreCase) || 
            string.Equals(role, AsistenteSeguro, StringComparison.OrdinalIgnoreCase);

        public static bool IsLaboratorio(string? role) => 
            IsAdmin(role) || 
            string.Equals(role, AsistenteRX, StringComparison.OrdinalIgnoreCase);
    }
}
