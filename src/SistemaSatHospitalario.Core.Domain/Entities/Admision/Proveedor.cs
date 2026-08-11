using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Proveedor
    {
        public Guid Id { get; private set; }
        public string RIF { get; private set; }
        public string RazonSocial { get; private set; }
        public string? Direccion { get; private set; }
        public string? Telefono { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        protected Proveedor() { }

        public Proveedor(string rif, string razonSocial, string? direccion = null, string? telefono = null)
        {
            if (string.IsNullOrWhiteSpace(rif)) throw new ArgumentException("El RIF del proveedor es requerido.", nameof(rif));
            if (string.IsNullOrWhiteSpace(razonSocial)) throw new ArgumentException("La razón social es requerida.", nameof(razonSocial));

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
            if (string.IsNullOrWhiteSpace(rif)) throw new ArgumentException("El RIF es requerido.", nameof(rif));
            if (string.IsNullOrWhiteSpace(razonSocial)) throw new ArgumentException("La razón social es requerida.", nameof(razonSocial));

            RIF = rif.Trim().ToUpperInvariant();
            RazonSocial = razonSocial.Trim();
            Direccion = direccion?.Trim();
            Telefono = telefono?.Trim();
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
    }
}
