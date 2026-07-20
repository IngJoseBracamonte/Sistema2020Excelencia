using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    /// <summary>
    /// IDs fijos predefinidos para las entidades de seed del sistema.
    /// 
    /// REGLA CRÍTICA: Todos los IDs en esta clase son INMUTABLES y FIJOS.
    /// El SystemDbInitializer los usa como Ids explícitos (Upsert).
    /// Toda la lógica de negocio (InventoryService, CargarServicioACuenta, etc.)
    /// usa estas constantes para comparar directamente por Guid, SIN NINGUNA
    /// búsqueda por string de Codigo de Sede o AreaClinica.
    /// 
    /// Patrón de numeración:
    ///   1000...000X => Sedes físicas del hospital
    ///   2000...000X => Áreas clínicas / Ubicaciones (Camas, Boxes, Habitaciones)
    /// </summary>
    public static class SeedConstants
    {
        // ─────────────────────────────────────────────────────────────
        // SEDES (IDs fijos, uno por área de inventario del hospital)
        // ─────────────────────────────────────────────────────────────

        /// <summary>Sede principal hospitalaria (almacén central de insumos).</summary>
        public static readonly Guid SedeId_Principal = new Guid("10000000-0000-0000-0000-000000000001");

        /// <summary>Sede del área de Emergencias. Stock de medicamentos e insumos de urgencia.</summary>
        public static readonly Guid SedeId_Emergencia = new Guid("10000000-0000-0000-0000-000000000002");

        /// <summary>Sede del área de Hospitalización (planta médica). Stock de medicamentos e insumos de internamiento.</summary>
        public static readonly Guid SedeId_Hospitalizacion = new Guid("10000000-0000-0000-0000-000000000003");

        /// <summary>
        /// Sede exclusiva de la Unidad de Cuidados Intensivos (UCI).
        /// UCI es una sub-área de Hospitalización en términos de facturación
        /// (TipoIngreso='Hospitalizacion', SubAreaClinica='UCI'), pero tiene
        /// su propio depósito de inventario independiente.
        /// </summary>
        public static readonly Guid SedeId_UCI = new Guid("10000000-0000-0000-0000-000000000004");


        // ─────────────────────────────────────────────────────────────
        // ÁREAS CLÍNICAS (IDs fijos para mapear por ID y no por strings)
        // ─────────────────────────────────────────────────────────────
        public static readonly Guid AreaId_Emergencia = new Guid("30000000-0000-0000-0000-000000000001");
        public static readonly Guid AreaId_Hospitalizacion = new Guid("30000000-0000-0000-0000-000000000002");
        public static readonly Guid AreaId_UCI = new Guid("30000000-0000-0000-0000-000000000003");
        public static readonly Guid AreaId_Farmacia = new Guid("30000000-0000-0000-0000-000000000004");
        public static readonly Guid AreaId_Laboratorio = new Guid("30000000-0000-0000-0000-000000000005");

        // ─────────────────────────────────────────────────────────────
        // ÁREAS CLÍNICAS / UBICACIONES FÍSICAS (Camas, Boxes, Habitaciones)
        // ─────────────────────────────────────────────────────────────

        // — Emergencia: Boxes de Observación —
        public static readonly Guid UbicacionId_Box1 = new Guid("20000000-0000-0000-0000-000000000001");
        public static readonly Guid UbicacionId_Box2 = new Guid("20000000-0000-0000-0000-000000000002");
        public static readonly Guid UbicacionId_Box3 = new Guid("20000000-0000-0000-0000-000000000003");
        public static readonly Guid UbicacionId_SalaReanimacion1 = new Guid("20000000-0000-0000-0000-000000000004");

        // — Hospitalización: Habitaciones —
        public static readonly Guid UbicacionId_Hab101 = new Guid("20000000-0000-0000-0000-000000000010");
        public static readonly Guid UbicacionId_Hab102 = new Guid("20000000-0000-0000-0000-000000000011");
        public static readonly Guid UbicacionId_Hab103 = new Guid("20000000-0000-0000-0000-000000000012");
        public static readonly Guid UbicacionId_Hab104A = new Guid("20000000-0000-0000-0000-000000000013");
        public static readonly Guid UbicacionId_Hab113 = new Guid("20000000-0000-0000-0000-000000000014");

        // — UCI: Camas de Cuidados Intensivos —
        public static readonly Guid UbicacionId_UCICama1 = new Guid("20000000-0000-0000-0000-000000000020");
        public static readonly Guid UbicacionId_UCICama2 = new Guid("20000000-0000-0000-0000-000000000021");
        public static readonly Guid UbicacionId_UCICama3 = new Guid("20000000-0000-0000-0000-000000000022");

        // ─────────────────────────────────────────────────────────────
        // HELPERS: Mapeo TipoIngreso → SedeId de inventario
        // (Elimina el switch de strings en InventoryService.cs)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Retorna el SedeId de inventario correspondiente al tipo de ingreso y sub-área de la cuenta.
        /// No requiere ninguna consulta a la base de datos.
        /// </summary>
        public static Guid ResolveSedeInventario(string tipoIngreso, string? subAreaClinica)
        {
            // UCI tiene su propio depósito aunque el TipoIngreso sea 'Hospitalizacion'
            if (string.Equals(subAreaClinica, SubAreas.UCI, StringComparison.OrdinalIgnoreCase))
                return SedeId_UCI;

            return tipoIngreso?.ToUpperInvariant() switch
            {
                "EMERGENCIA"      => SedeId_Emergencia,
                "HOSPITALIZACION" => SedeId_Hospitalizacion,
                _                 => SedeId_Principal
            };
        }
    }

    /// <summary>
    /// Sub-áreas clínicas dentro de un tipo de ingreso.
    /// Actualmente solo UCI es sub-área (de Hospitalización).
    /// </summary>
    public static class SubAreas
    {
        /// <summary>
        /// Unidad de Cuidados Intensivos.
        /// TipoIngreso = 'Hospitalizacion', SubAreaClinica = 'UCI'.
        /// En reportes se muestra como "UCI" (sin prefijo de Hospitalización).
        /// </summary>
        public const string UCI = "UCI";
    }

    /// <summary>
    /// Mantiene una versión global y secuencial del estado de ocupación de camas.
    /// </summary>
    public static class GlobalStateVersion
    {
        private static int _version = 1000;
        private static readonly object _lock = new object();

        public static int Increment()
        {
            lock (_lock)
            {
                return ++_version;
            }
        }

        public static int Current
        {
            get
            {
                lock (_lock)
                {
                    return _version;
                }
            }
        }
    }
}
