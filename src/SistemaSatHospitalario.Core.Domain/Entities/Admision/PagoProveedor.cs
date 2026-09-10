using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision;

public class PagoProveedor
{
    public Guid Id { get; private set; }
    public Guid OrdenCompraId { get; private set; }
    public virtual OrdenCompraInventario OrdenCompra { get; private set; } = null!;
    public DateTime FechaPago { get; private set; }
    public decimal MontoAbonadoUSD { get; private set; }
    public decimal TasaCambio { get; private set; }
    
    // Propiedad calculada en memoria (3FN Pura - No persistida)
    public decimal MontoAbonadoBs => Math.Round(MontoAbonadoUSD * TasaCambio, 2);

    public string MetodoPago { get; private set; } = string.Empty;
    public string Referencia { get; private set; } = string.Empty;

    /// <summary>
    /// FK lógica a Usuarios (Identity, PK Guid) del usuario que registró el pago.
    /// </summary>
    public Guid? UsuarioIdentityId { get; private set; }
    public string? Observaciones { get; private set; }

    protected PagoProveedor() { }

    public PagoProveedor(
        Guid ordenCompraId, 
        decimal montoAbonadoUSD, 
        decimal tasaCambio, 
        string metodoPago, 
        string referencia, 
        Guid? usuarioIdentityId = null, 
        string? observaciones = null)
    {
        if (ordenCompraId == Guid.Empty)
            throw new ArgumentException("El ID de la orden de compra es obligatorio.", nameof(ordenCompraId));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(montoAbonadoUSD, nameof(montoAbonadoUSD));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tasaCambio, nameof(tasaCambio));
        ArgumentException.ThrowIfNullOrWhiteSpace(metodoPago, nameof(metodoPago));

        Id = Guid.NewGuid();
        OrdenCompraId = ordenCompraId;
        FechaPago = DateTime.UtcNow;
        MontoAbonadoUSD = Math.Round(montoAbonadoUSD, 2);
        TasaCambio = tasaCambio;
        MetodoPago = metodoPago.Trim();
        Referencia = referencia?.Trim() ?? string.Empty;
        UsuarioIdentityId = usuarioIdentityId;
        Observaciones = observaciones?.Trim();
    }

    /// <summary>
    /// Sobrecarga de conveniencia para recibir usuarioId en formato string (GUID).
    /// </summary>
    public PagoProveedor(
        Guid ordenCompraId, 
        decimal montoAbonadoUSD, 
        decimal tasaCambio, 
        string metodoPago, 
        string referencia, 
        string usuarioId, 
        string? observaciones = null)
        : this(
            ordenCompraId, 
            montoAbonadoUSD, 
            tasaCambio, 
            metodoPago, 
            referencia, 
            Guid.TryParse(usuarioId, out var parsed) ? parsed : null, 
            observaciones)
    {
    }
}