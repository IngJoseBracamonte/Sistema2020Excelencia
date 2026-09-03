namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>IDs fijos del catálogo EstadosCaja (3FN).</summary>
    public static class EstadoCajaConstants
    {
        public const int AbiertaId = 1;
        public const int CerradaPorAsistenteId = 2;
        public const int CerradaId = 3;

        public static int FromLegacyString(string? estado) => estado?.Trim().ToUpperInvariant() switch
        {
            "ABIERTA" => AbiertaId,
            "CERRADAPORASISTENTE" => CerradaPorAsistenteId,
            "CERRADA" => CerradaId,
            _ => AbiertaId
        };

        public static string ToLegacyString(int estadoId) => estadoId switch
        {
            CerradaPorAsistenteId => EstadoConstants.CajaCerradaPorAsistente,
            CerradaId => EstadoConstants.CajaCerrada,
            _ => EstadoConstants.CajaAbierta
        };
    }

    /// <summary>IDs fijos del catálogo EstadosCuenta (3FN).</summary>
    public static class EstadoCuentaConstants
    {
        public const int AbiertaId = 1;
        public const int FacturadaId = 2;
        public const int AnuladaId = 3;
        public const int ValidadaId = 4;

        public static int FromLegacyString(string? estado) => estado?.Trim().ToUpperInvariant() switch
        {
            "ABIERTA" => AbiertaId,
            "FACTURADA" => FacturadaId,
            "ANULADA" => AnuladaId,
            "VALIDADA" => ValidadaId,
            _ => AbiertaId
        };

        public static string ToLegacyString(int estadoId) => estadoId switch
        {
            FacturadaId => EstadoConstants.Facturada,
            AnuladaId => EstadoConstants.Anulada,
            ValidadaId => EstadoConstants.Validada,
            _ => EstadoConstants.Abierta
        };
    }

    /// <summary>IDs fijos del catálogo TiposIngreso (3FN).</summary>
    public static class TipoIngresoConstants
    {
        public const int ParticularId = 1;
        public const int SeguroId = 2;
        public const int HospitalizacionId = 3;
        public const int EmergenciaId = 4;
        public const int UciId = 5;

        public static int FromLegacyString(string? tipo) => tipo?.Trim().ToUpperInvariant() switch
        {
            "PARTICULAR" => ParticularId,
            "SEGURO" => SeguroId,
            "HOSPITALIZACION" or "HOSPITALIZACIÓN" => HospitalizacionId,
            "EMERGENCIA" => EmergenciaId,
            "UCI" => UciId,
            _ => ParticularId
        };

        public static string ToLegacyString(int tipoId) => tipoId switch
        {
            SeguroId => EstadoConstants.Seguro,
            HospitalizacionId => EstadoConstants.Hospitalizacion,
            EmergenciaId => EstadoConstants.Emergencia,
            UciId => EstadoConstants.UCI,
            _ => EstadoConstants.Particular
        };
    }

    /// <summary>IDs fijos del catálogo EstadosFiscales (3FN).</summary>
    public static class EstadoFiscalConstants
    {
        public const int BorradorId = 1;
        public const int EmitidaId = 2;
        public const int AnuladaId = 3;

        public static int FromLegacyString(string? estado) => estado?.Trim().ToUpperInvariant() switch
        {
            "BORRADOR" => BorradorId,
            "EMITIDA" => EmitidaId,
            "ANULADA" => AnuladaId,
            _ => BorradorId
        };

        public static string ToLegacyString(int estadoId) => estadoId switch
        {
            EmitidaId => EstadoConstants.Emitida,
            AnuladaId => EstadoConstants.Anulada,
            _ => EstadoConstants.Borrador
        };
    }
}
