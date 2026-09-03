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

        /// <summary>
        /// LEGACY (3FN): ID de usuario como texto. Fuente de verdad:
        /// <see cref="UsuarioIdentityId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioIdentityId. Columna legacy pendiente de DROP.")]
        public string UsuarioId { get; private set; } = string.Empty;

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que registró el pago.</summary>
        public Guid? UsuarioIdentityId { get; private set; }
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
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioId = usuarioId ?? "admin";
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
            Observaciones = observaciones;
        }
    }
}
