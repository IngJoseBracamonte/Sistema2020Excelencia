using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CamaMonitoreoDetalleCargoDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public decimal Cantidad { get; set; }
        public bool IncluidoEnTarifaBase { get; set; }
        public decimal Total { get; set; }
    }
}
