using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ReciboPdfDto
    {
        public Guid Id { get; set; }
        public string NumeroRecibo { get; set; }
        public DateTime FechaEmision { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public string TipoIngreso { get; set; }
        public decimal TotalUSD { get; set; }
        public decimal TotalBS { get; set; }
        public decimal TasaBcv { get; set; }
        public List<ReciboDetallePdfDto> Detalles { get; set; } = new();
        public List<PagoDetallePdfDto> Pagos { get; set; } = new();
    }

    public class ReciboDetallePdfDto
    {
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class PagoDetallePdfDto
    {
        public string MetodoPago { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal EquivalenteBase { get; set; }
        public string Referencia { get; set; }
    }
}
