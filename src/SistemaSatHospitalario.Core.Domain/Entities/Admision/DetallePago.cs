using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DetallePago
    {
        public Guid Id { get; protected set; }
        public Guid ReciboFacturaId { get; protected set; }

        /// <summary>
        /// LEGACY (3FN): texto del método de pago. Fuente de verdad: <see cref="MetodoPagoId"/>
        /// y la navegación <see cref="MetodoPagoNav"/>. Alias de compatibilidad hasta el DROP.
        /// </summary>
        [Obsolete("Usar MetodoPagoId / MetodoPagoNav. Columna legacy pendiente de DROP.")]
        public string MetodoPago { get; protected set; } // Zelle, PagoMovil, EfectivoUSD, PuntoVenta
        public Guid? MetodoPagoId { get; protected set; }
        public string ReferenciaBancaria { get; protected set; }
        public decimal MontoAbonadoMoneda { get; protected set; }
        public decimal EquivalenteAbonadoBase { get; protected set; }
        public decimal TasaCambioAplicada { get; protected set; }
        public DateTime FechaPago { get; protected set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioCargaId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioCargaId. Columna legacy pendiente de DROP.")]
        public string UsuarioCarga { get; protected set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que cargó el pago.</summary>
        public Guid? UsuarioCargaId { get; protected set; }

        public ReciboFactura ReciboFactura { get; protected set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(MetodoPagoId))]
        public virtual CatalogoMetodoPago? MetodoPagoNav { get; protected set; }

        protected DetallePago() { }

        public DetallePago(Guid reciboFacturaId, string metodoPago, string referenciaBancaria, decimal montoAbonadoMoneda, decimal equivalenteAbonadoBase, decimal tasaCambioAplicada, string usuarioCarga, Guid? metodoPagoId = null, Guid? usuarioCargaId = null)
        {
            if (montoAbonadoMoneda == 0) throw new ArgumentException("El monto no puede ser 0.");

            Id = Guid.NewGuid();
            ReciboFacturaId = reciboFacturaId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            MetodoPago = metodoPago ?? throw new ArgumentNullException(nameof(metodoPago));
#pragma warning restore CS0618
            MetodoPagoId = metodoPagoId;
            ReferenciaBancaria = referenciaBancaria;
            MontoAbonadoMoneda = montoAbonadoMoneda;
            EquivalenteAbonadoBase = equivalenteAbonadoBase;
            TasaCambioAplicada = tasaCambioAplicada;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
#pragma warning restore CS0618
            // 3FN: si no se pasa la FK explícita, intentar parsear el texto como GUID
            UsuarioCargaId = usuarioCargaId ?? (Guid.TryParse(usuarioCarga, out var parsed) ? parsed : (Guid?)null);
            FechaPago = DateTime.UtcNow;
        }
    }
}
