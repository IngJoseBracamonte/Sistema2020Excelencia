using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision;

public class Proveedor
{
    public Guid Id { get; private set; }
    public string RIF { get; private set; } = string.Empty;
    public string RazonSocial { get; private set; } = string.Empty;
    public string? Direccion { get; private set; }
    public string? Telefono { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    protected Proveedor() { }

    public Proveedor(string rif, string razonSocial, string? direccion = null, string? telefono = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rif, nameof(rif));
        ArgumentException.ThrowIfNullOrWhiteSpace(razonSocial, nameof(razonSocial));

        Id = Guid.NewGuid();
        RIF = rif.Trim().ToUpperInvariant();
        RazonSocial = razonSocial.Trim();
        Direccion = direccion?.Trim();
        Telefono = telefono?.Trim();
        Activo = true;
        FechaRegistro = DateTime.UtcNow;
    }

    public void Actualizar(string rif, string razonSocial, string? direccion, string? telefono)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rif, nameof(rif));
        ArgumentException.ThrowIfNullOrWhiteSpace(razonSocial, nameof(razonSocial));

        RIF = rif.Trim().ToUpperInvariant();
        RazonSocial = razonSocial.Trim();
        Direccion = direccion?.Trim();
        Telefono = telefono?.Trim();
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}