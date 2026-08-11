using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CatalogItemDto : IPricedItem
    {
        public string Id { get; set; } // Puede ser Guid o IdPersona/IdPerfil string
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Compatibilidad (será Bs)
        public decimal PrecioBs { get; set; }
        public decimal PrecioUsd { get; set; }
        public decimal HonorarioBase { get; set; }
        public string Tipo { get; set; } // Legacy compatibility
        public int TipoServicioId { get; set; } // 1=Medico, 2=Laboratorio, 3=RX, 4=Tomo, 5=Insumo, 6=Informe
        public string EditorType { get; set; } = "SERVICIO"; // Estandarizado para Frontend y Redis
        public int CategoryId { get; set; } // Consultation=1, Lab=2, etc. (V5.2)
        public bool EsLegacy { get; set; }
        public bool Activo { get; set; }
        public string? HonorariumCategory { get; set; }
        public Guid? EspecialidadId { get; set; }
        public Guid? ServicioInformeId { get; set; }
        public bool EsServicioInforme { get; set; }
        public List<string> SugerenciasIds { get; set; } = new List<string>();
        public List<DoctorHonorarioDto> HonorariosMedicos { get; set; } = new List<DoctorHonorarioDto>();
        public List<DoctorHonorarioDto> HonorariosEspecificos { get; set; } = new List<DoctorHonorarioDto>();
        public List<ServicioInsumoRecetaDto> Receta { get; set; } = new List<ServicioInsumoRecetaDto>();
        public List<ServicioInsumoRecetaDto> InsumosReceta { get; set; } = new List<ServicioInsumoRecetaDto>();
        public string? UnidadMedida { get; set; }
        public bool PermiteFraccionamiento { get; set; }
        public bool IsConsultation => CategoryId == 1 || (Tipo ?? "").Contains("CONSULT", StringComparison.OrdinalIgnoreCase);

        public void CalculatePrices(decimal tasa)
        {
            if (tasa <= 0) tasa = 1;
            PrecioBs = PrecioUsd * tasa;
            Precio = PrecioBs;
        }
    }
}
