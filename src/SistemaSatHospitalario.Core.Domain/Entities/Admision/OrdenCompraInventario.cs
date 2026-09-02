using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class OrdenCompraInventario
    {
        public Guid Id { get; private set; }
        public string NumeroFactura { get; private set; } = string.Empty;
        public Guid? ProveedorId { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre desnormalizado del proveedor. Fuente de verdad:
        /// la navegación <see cref="Proveedor"/> vía <see cref="ProveedorId"/>. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar Proveedor.RazonSocial vía ProveedorId. Columna legacy pendiente de DROP.")]
        public string ProveedorNombre { get; private set; } = string.Empty;
        public virtual Proveedor? Proveedor { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public decimal MontoTotalUSD { get; private set; }

        /// <summary>
        /// LEGACY (3FN): monto en bolívares calculado y persistido. Fuente de verdad:
        /// <see cref="MontoTotalUSD"/> × tasa de cambio del día (no persistida aquí).
        /// Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Calcular como MontoTotalUSD * tasaCambio. Columna legacy pendiente de DROP.")]
        public decimal MontoTotalBs { get; private set; }

        /// <summary>
        /// LEGACY (3FN): total abonado calculado y persistido. Fuente de verdad:
        /// <see cref="Pagos"/>.Sum(p => p.MontoAbonadoUSD). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Calcular como Pagos.Sum(p => p.MontoAbonadoUSD). Columna legacy pendiente de DROP.")]
        public decimal TotalAbonadoUSD { get; private set; }

        /// <summary>
        /// LEGACY (3FN): saldo pendiente calculado y persistido. Fuente de verdad:
        /// <see cref="MontoTotalUSD"/> - <see cref="TotalAbonadoUSD"/>. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Calcular como MontoTotalUSD - TotalAbonadoUSD. Columna legacy pendiente de DROP.")]
        public decimal SaldoPendienteUSD { get; private set; }
        public string Estado { get; private set; } = "PorPagar"; // PorPagar, Pagado
        public string? Observaciones { get; private set; }

        public virtual ICollection<PagoProveedor> Pagos { get; private set; } = new List<PagoProveedor>();

        private OrdenCompraInventario() { }

        public OrdenCompraInventario(
            string numeroFactura, 
            string proveedorNombre, 
            DateTime fechaEmision, 
            decimal montoTotalUSD, 
            decimal tasaCambio, 
            Guid? proveedorId = null, 
            string? observaciones = null)
        {
            if (string.IsNullOrWhiteSpace(numeroFactura)) throw new ArgumentException("El número de factura es requerido.", nameof(numeroFactura));
            if (string.IsNullOrWhiteSpace(proveedorNombre)) throw new ArgumentException("El nombre del proveedor es requerido.", nameof(proveedorNombre));
            if (montoTotalUSD <= 0) throw new ArgumentException("El monto total debe ser mayor a cero.", nameof(montoTotalUSD));
            if (tasaCambio <= 0) throw new ArgumentException("La tasa de cambio debe ser mayor a cero.", nameof(tasaCambio));

            Id = Guid.NewGuid();
            NumeroFactura = numeroFactura.Trim();
            ProveedorId = proveedorId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            ProveedorNombre = proveedorNombre.Trim();
#pragma warning restore CS0618
            FechaEmision = fechaEmision;
            MontoTotalUSD = Math.Round(montoTotalUSD, 2);
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            MontoTotalBs = Math.Round(montoTotalUSD * tasaCambio, 2);
            TotalAbonadoUSD = 0m;
            SaldoPendienteUSD = MontoTotalUSD;
#pragma warning restore CS0618
            Estado = "PorPagar";
            Observaciones = observaciones;
        }

        public void AsignarProveedor(Proveedor proveedor)
        {
            ArgumentNullException.ThrowIfNull(proveedor);

            ProveedorId = proveedor.Id;
            Proveedor = proveedor;
            ProveedorNombre = proveedor.RazonSocial;
        }

        public PagoProveedor RegistrarAbono(decimal montoAbonadoUSD, decimal tasaCambio, string metodoPago, string referencia, string usuarioId, string? observaciones = null)
        {
            if (montoAbonadoUSD <= 0) throw new InvalidOperationException("El monto abonado debe ser mayor a cero.");
            if (montoAbonadoUSD > SaldoPendienteUSD) throw new InvalidOperationException($"El abono de ${montoAbonadoUSD:N2} supera el saldo pendiente de ${SaldoPendienteUSD:N2}.");
            if (tasaCambio <= 0) throw new InvalidOperationException("La tasa de cambio debe ser mayor a cero.");
            if (string.IsNullOrWhiteSpace(metodoPago)) throw new InvalidOperationException("El método de pago es requerido.");

            var pago = new PagoProveedor(
                Id, 
                montoAbonadoUSD, 
                tasaCambio, 
                metodoPago, 
                referencia ?? string.Empty, 
                usuarioId ?? "admin", 
                observaciones);

            Pagos.Add(pago);

#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            TotalAbonadoUSD = Math.Round(TotalAbonadoUSD + montoAbonadoUSD, 2);
            SaldoPendienteUSD = Math.Round(MontoTotalUSD - TotalAbonadoUSD, 2);

            // Regla Automática: Si SaldoPendienteUSD == 0 -> Pasa a Pagado
            if (SaldoPendienteUSD <= 0m)
            {
                SaldoPendienteUSD = 0m;
                Estado = "Pagado";
            }
#pragma warning restore CS0618

            return pago;
        }
    }
}
