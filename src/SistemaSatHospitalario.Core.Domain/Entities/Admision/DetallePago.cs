using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DetallePago
    {
        public Guid Id { get; protected set; }
        public Guid ReciboFacturaId { get; protected set; }
        public Guid? MetodoPagoId { get; protected set; }
        public string ReferenciaBancaria { get; protected set; }
        public decimal MontoAbonadoMoneda { get; protected set; }
        public decimal EquivalenteAbonadoBase { get; protected set; }
        public decimal TasaCambioAplicada { get; protected set; }
        public DateTime FechaPago { get; protected set; }
        public Guid? UsuarioCargaId { get; protected set; }

        public ReciboFactura ReciboFactura { get; protected set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(MetodoPagoId))]
        public virtual CatalogoMetodoPago? MetodoPagoNav { get; protected set; }

        protected DetallePago() { }

        public DetallePago(Guid reciboFacturaId, string metodoPago, string referenciaBancaria, decimal montoAbonadoMoneda, decimal equivalenteAbonadoBase, decimal tasaCambioAplicada, Guid? usuarioCargaId, Guid? metodoPagoId = null)
        {
            if (montoAbonadoMoneda == 0) throw new ArgumentException("El monto no puede ser 0.");
            Id = Guid.NewGuid();
            ReciboFacturaId = reciboFacturaId;
            MetodoPagoId = metodoPagoId;
            ReferenciaBancaria = referenciaBancaria;
            MontoAbonadoMoneda = montoAbonadoMoneda;
            EquivalenteAbonadoBase = equivalenteAbonadoBase;
            TasaCambioAplicada = tasaCambioAplicada;
            UsuarioCargaId = usuarioCargaId;
            FechaPago = DateTime.UtcNow;
        }
    }
}
