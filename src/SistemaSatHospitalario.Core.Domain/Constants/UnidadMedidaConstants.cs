namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// IDs fijos del catálogo UnidadesMedida (3FN).
    /// Deben coincidir con el seed de la migración EF y con el enum legacy UnidadMedida.
    /// </summary>
    public static class UnidadMedidaConstants
    {
        public const int UnidadId = 1;
        public const int KgId = 2;
        public const int GramoId = 3;
        public const int DecigramoId = 4;
        public const int MiligramoId = 5;
        public const int LitroId = 6;
        public const int MililitroId = 7;

        /// <summary>Mapea el enum legacy al ID del catálogo.</summary>
        public static int FromEnum(Enums.UnidadMedida unidad) => (int)unidad;

        /// <summary>Mapea el ID del catálogo al enum legacy.</summary>
        public static Enums.UnidadMedida ToEnum(int unidadId) => (Enums.UnidadMedida)unidadId;

        /// <summary>Mapea el código de texto (varchar legacy) al ID del catálogo.</summary>
        public static int FromCodigo(string? codigo) => codigo?.Trim().ToUpperInvariant() switch
        {
            "UNIDAD" => UnidadId,
            "KG" => KgId,
            "G" => GramoId,
            "DG" => DecigramoId,
            "MG" => MiligramoId,
            "L" => LitroId,
            "ML" => MililitroId,
            _ => UnidadId
        };

        /// <summary>Mapea el ID del catálogo al código de texto (alias legacy).</summary>
        public static string ToCodigo(int unidadId) => unidadId switch
        {
            KgId => "KG",
            GramoId => "G",
            DecigramoId => "DG",
            MiligramoId => "MG",
            LitroId => "L",
            MililitroId => "ML",
            _ => "UNIDAD"
        };
    }
}
