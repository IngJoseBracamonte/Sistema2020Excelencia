using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Inventario
{
    public class OrdenCompraInventarioDto
    {
        public Guid Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public Guid? ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotalUSD { get; set; }
        public decimal MontoTotalBs { get; set; }
        public decimal TotalAbonadoUSD { get; set; }
        public decimal SaldoPendienteUSD { get; set; }
        public string Estado { get; set; } = "PorPagar";
        public string? Observaciones { get; set; }
        public List<PagoProveedorDto> Pagos { get; set; } = new();
    }

    public class PagoProveedorDto
    {
        public Guid Id { get; set; }
        public Guid OrdenCompraId { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string ProveedorNombre { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal MontoAbonadoUSD { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal MontoAbonadoBs { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class RegistrarPagoProveedorRequest
    {
        public Guid OrdenCompraId { get; set; }
        public decimal MontoAbonadoUSD { get; set; }
        public decimal TasaCambio { get; set; }
        public string MetodoPago { get; set; } = "Transferencia";
        public string Referencia { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
