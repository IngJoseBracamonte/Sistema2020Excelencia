using System;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ModificarTriageYValoracionCommand : IRequest<TriageYValoracionDto>
    {
        public Guid TriageId { get; set; }
        public Guid ValoracionId { get; set; }
        
        // Signos Vitales
        public string MotivoConsulta { get; set; } = string.Empty;
        public string TensionArterial { get; set; } = string.Empty;
        public int FrecuenciaCardiaca { get; set; }
        public int FrecuenciaRespiratoria { get; set; }
        public decimal Temperatura { get; set; }
        public int SaturacionO2 { get; set; }
        public int? GlicemiaCapilar { get; set; }

        // Triage State Descriptions
        public string? DescripcionRapida { get; set; }
        public string? DescripcionDetallada { get; set; }

        // Modular update selectors
        public bool RegistrarConstantesVitales { get; set; } = true;
        public bool RegistrarValoracionFisica { get; set; } = true;
        public bool RegistrarAntecedentes { get; set; } = true;
        public bool RegistrarEstadoActual { get; set; } = true;
        
        // Valoración Física
        public string EstadoConciencia { get; set; } = string.Empty;
        public int GlasgowOcular { get; set; }
        public int GlasgowVerbal { get; set; }
        public int GlasgowMotor { get; set; }
        public int GlasgowTotal { get; set; }
        public string ViaAerea { get; set; } = string.Empty;
        public string Ventilacion { get; set; } = string.Empty;
        public string Pulso { get; set; } = string.Empty;
        public string PielMucosas { get; set; } = string.Empty;
        public string LlenadoCapilar { get; set; } = string.Empty;
        public string Pupilas { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string AccesosVenosos { get; set; } = string.Empty;
        public string Pertenencias { get; set; } = string.Empty;
        public string AntecedentesMedicos { get; set; } = string.Empty;
        
        public string UsuarioRegistro { get; set; } = string.Empty;
    }
}
