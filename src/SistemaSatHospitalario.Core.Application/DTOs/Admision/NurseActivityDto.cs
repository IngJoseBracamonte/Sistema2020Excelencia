using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class NurseActivityDto
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;
        public string PacienteNombre { get; set; } = string.Empty;
        public string TipoActividad { get; set; } = string.Empty; // "Carga de Insumo" o "Triage/Signos Vitales"
        public string Detalle { get; set; } = string.Empty;
    }
}
