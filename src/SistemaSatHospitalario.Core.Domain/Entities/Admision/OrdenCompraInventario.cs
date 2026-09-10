using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision;

public class OrdenCompraInventario
{
    public Guid Id { get; private set; }
    public string NumeroFactura { get; private set; } = string.Empty;
    public Guid? ProveedorId { get; private set; }
    public virtual Proveedor? Proveedor { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public decimal MontoTotalUSD { get; private set; }
    public string Estado { get; private set; } = "PorPagar"; // PorPagar, Pagado
    public string? Observaciones { get; private set; }

    private readonly List<PagoProveedor> _pagos = [];
    public virtual IReadOnlyCollection<PagoProveedor> Pagos => _pagos.AsReadOnly();

    // Propiedades calculadas en memoria (3FN Pura - No persistidas)
    public decimal TotalAbonadoUSD => _pagos.Sum(p => p.MontoAbonadoUSD);
    public decimal SaldoPendienteUSD => Math.Max(0m, MontoTotalUSD - TotalAbonadoUSD);

    protected OrdenCompraInventario() { }

    public OrdenCompraInventario(
        string numeroFactura, 
        DateTime fechaEmision, 
        decimal montoTotalUSD, 
        Guid? proveedorId = null, 
        string? observaciones = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(numeroFactura, nameof(numeroFactura));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(montoTotalUSD, nameof(montoTotalUSD));

        Id = Guid.NewGuid();
        NumeroFactura = numeroFactura.Trim();
        ProveedorId = proveedorId;
        FechaEmision = fechaEmision;
        MontoTotalUSD = Math.Round(montoTotalUSD, 2);
        Estado = "PorPagar";
        Observaciones = observaciones?.Trim();
    }

    public void AsignarProveedor(Proveedor proveedor)
    {
        ArgumentNullException.ThrowIfNull(proveedor);

        ProveedorId = proveedor.Id;
        Proveedor = proveedor;
    }

    public PagoProveedor RegistrarAbono(
        decimal montoAbonadoUSD, 
        decimal tasaCambio, 
        string metodoPago, 
        string referencia, 
        string usuarioId, 
        string? observaciones = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(montoAbonadoUSD, nameof(montoAbonadoUSD));
        ArgumentException.ThrowIfNullOrWhiteSpace(metodoPago, nameof(metodoPago));

        if (Estado == "Pagado" || SaldoPendienteUSD == 0)
            throw new InvalidOperationException("La orden de compra ya se encuentra totalmente pagada.");

        var pago = new PagoProveedor(
            Id, 
            montoAbonadoUSD, 
            tasaCambio, 
            metodoPago, 
            referencia ?? string.Empty, 
            usuarioId, 
            observaciones);

        _pagos.Add(pago);

        // Actualización de estado del agregado si se liquida el saldo
        if (SaldoPendienteUSD <= 0)
        {
            Estado = "Pagado";
        }

        return pago;
    }
}