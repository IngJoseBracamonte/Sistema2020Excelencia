using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    public static class HonorarioConstants
    {
        public const string CategoriaConsulta = "CONSULTA";
        public const string CategoriaRX = "RX";
        public const string CategoriaInforme = "INFORME";
        public const string CategoriaCitologia = "CITOLOGIA";
        public const string CategoriaBiopsia = "BIOPSIA";
        public const string CategoriaOtros = "OTROS";

        // Reglas de Mapeo (Senior Rule-Engine Pattern)
        public static readonly string[] RXPrefixes = { "RX", "IMAG", "RADI", "ECOG", "TOMO" };
        public static readonly string[] InformePrefixes = { "INFO", "DESC", "EPIC" };
        public static readonly string[] CitologiaPrefixes = { "CITO", "GINE", "PAPA" };
        public static readonly string[] BiopsiaPrefixes = { "BIOP", "PATO", "HIST" };
        public static readonly string[] ConsultaPrefixes = { "CONS", "MEDI", "MÉDI", "ENTR", "CITA" };

        // Acciones de Auditoría
        public const string AccionAsignacionManual = "ASIGNACION_MANUAL";
        public const string AccionReasignacion = "REASIGNACION";
        public const string AccionAsignacionDefault = "ASIGNACION_DEFAULT";
        public const string AccionLimpiezaManual = "LIMPIEZA_MANUAL";
        public const string AccionConfiguracionCambio = "CONFIG_CAMBIO";
    }
}
