using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ServicioInsumoRecetaDto
    {
        public Guid Id { get; set; }
        public Guid ServicioClinicoId { get; set; }
        public string ServicioCodigo { get; set; } = string.Empty;
        public Guid InsumoId { get; set; }
        public string InsumoNombre { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string UnidadMedidaConsumo { get; set; } = "UNIDAD";
    }
}
