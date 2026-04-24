using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DetallePago
    {
        public Guid Id { get; protected set; }
        public Guid ReciboFacturaId { get; protected set; }
        public string MetodoPago { get; protected set; } // Zelle, PagoMovil, EfectivoUSD, PuntoVenta
        public string ReferenciaBancaria { get; protected set; }
        public decimal MontoAbonadoMoneda { get; protected set; }
        public decimal EquivalenteAbonadoBase { get; protected set; }
        public DateTime FechaPago { get; protected set; }
        public string UsuarioCarga { get; protected set; }

        public ReciboFactura ReciboFactura { get; protected set; }

        protected DetallePago() { }

        public DetallePago(Guid reciboFacturaId, string metodoPago, string referenciaBancaria, decimal montoAbonadoMoneda, decimal equivalenteAbonadoBase, string usuarioCarga)
        {
            if (montoAbonadoMoneda == 0) throw new ArgumentException("El monto no puede ser 0.");
            
            Id = Guid.NewGuid();
            ReciboFacturaId = reciboFacturaId;
            MetodoPago = metodoPago ?? throw new ArgumentNullException(nameof(metodoPago));
            ReferenciaBancaria = referenciaBancaria;
            MontoAbonadoMoneda = montoAbonadoMoneda;
            EquivalenteAbonadoBase = equivalenteAbonadoBase;
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
            FechaPago = DateTime.UtcNow;
        }
    }
}
