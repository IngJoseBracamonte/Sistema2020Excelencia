using System;

namespace SistemaSatHospitalario.Core.Domain.Constants
{
    public static class TipoServicioConstants
    {
        public const int Medico = 1;
        public const int Laboratorio = 2;
        public const int RX = 3;
        public const int RayosX = 3;
        public const int Tomo = 4;
        public const int Tomografia = 4;
        public const int Insumo = 5;
        public const int Informe = 6;

        public const string RayosXString = "RX";
        public const string TomografiaString = "TOMO";
        public const string InformeString = "INFORME";
        public const string RadiologiaEspecialidad = "RADIOLOGIA";

        public const string CategoriaMedicamento = "Medicamento";
        public const string CategoriaInsumo = "Insumo";
        public const string TipoInsumoMedicamento = "Insumo / Medicamento";
    }
}
