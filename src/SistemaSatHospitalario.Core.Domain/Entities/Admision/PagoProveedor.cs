using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PagoProveedor
    {
        public Guid Id { get; private set; }
        public Guid OrdenCompraId { get; private set; }
        public virtual OrdenCompraInventario OrdenCompra { get; private set; }
        public DateTime FechaPago { get; private set; }
        public decimal MontoAbonadoUSD { get; private set; }
        public decimal TasaCambio { get; private set; }
        public decimal MontoAbonadoBs { get; private set; }
        public string MetodoPago { get; private set; } = string.Empty;
        public string Referencia { get; private set; } = string.Empty;
        public string UsuarioId { get; private set; } = string.Empty;
        public string? Observaciones { get; private set; }

        private PagoProveedor() { }

        public PagoProveedor(
            Guid ordenCompraId, 
            decimal montoAbonadoUSD, 
            decimal tasaCambio, 
            string metodoPago, 
            string referencia, 
            string usuarioId, 
            string? observaciones = null)
        {
            Id = Guid.NewGuid();
            OrdenCompraId = ordenCompraId;
            FechaPago = DateTime.UtcNow;
            MontoAbonadoUSD = Math.Round(montoAbonadoUSD, 2);
            TasaCambio = tasaCambio;
            MontoAbonadoBs = Math.Round(montoAbonadoUSD * tasaCambio, 2);
            MetodoPago = metodoPago ?? "Transferencia";
            Referencia = referencia ?? string.Empty;
            UsuarioId = usuarioId ?? "admin";
            Observaciones = observaciones;
        }
    }
}
