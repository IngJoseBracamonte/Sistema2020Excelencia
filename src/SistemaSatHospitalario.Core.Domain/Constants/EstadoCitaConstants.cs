namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// IDs fijos del catálogo EstadosCitaMedica (3FN).
    /// Deben coincidir con el seed de la migración EF.
    /// </summary>
    public static class EstadoCitaConstants
    {
        public const int PendienteId = 1;
        public const int ConfirmadaId = 2;
        public const int AtendidaId = 3;
        public const int CanceladaId = 4;

        /// <summary>Mapea el texto legacy de estado al ID del catálogo.</summary>
        public static int FromLegacyString(string? estado) => estado?.Trim().ToUpperInvariant() switch
        {
            "PENDIENTE" => PendienteId,
            "CONFIRMADA" => ConfirmadaId,
            "ATENDIDA" => AtendidaId,
            "CANCELADO" or "CANCELADA" => CanceladaId,
            _ => PendienteId
        };

        /// <summary>Mapea el ID del catálogo al texto legacy (alias de compatibilidad).</summary>
        public static string ToLegacyString(int estadoId) => estadoId switch
        {
            ConfirmadaId => EstadoConstants.Confirmada,
            AtendidaId => EstadoConstants.Atendida,
            CanceladaId => EstadoConstants.Cancelado,
            _ => EstadoConstants.Pendiente
        };
    }
}
