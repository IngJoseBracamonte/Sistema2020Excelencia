using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CamaMonitoreoTriageDto
    {
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; } = string.Empty;
        public string MotivoConsulta { get; set; } = string.Empty;
        public string TensionArterial { get; set; } = string.Empty;
        public int FrecuenciaCardiaca { get; set; }
        public int FrecuenciaRespiratoria { get; set; }
        public decimal Temperatura { get; set; }
        public int SaturacionO2 { get; set; }
        public int GlasgowTotal { get; set; }
    }
}
