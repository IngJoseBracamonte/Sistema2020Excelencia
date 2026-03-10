using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaServicios
    {
        public Guid Id { get; private set; }
        public Guid PacienteId { get; private set; }
        public string TipoIngreso { get; private set; } // Particular, Seguro, Hospitalizacion, Emergencia
        public Guid? ConvenioId { get; private set; }
        public string Estado { get; private set; } // Abierta, Facturada, Anulada
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaCierre { get; private set; }

        private readonly List<DetalleServicioCuenta> _detalles = new();
        public IReadOnlyCollection<DetalleServicioCuenta> Detalles => _detalles.AsReadOnly();

        protected CuentaServicios() { }

        public CuentaServicios(Guid pacienteId, string tipoIngreso, Guid? convenioId = null)
        {
            Id = Guid.NewGuid();
            PacienteId = pacienteId;
            TipoIngreso = tipoIngreso ?? throw new ArgumentNullException(nameof(tipoIngreso));
            ConvenioId = convenioId;
            Estado = "Abierta";
            FechaCreacion = DateTime.UtcNow;
        }

        public void AgregarServicio(Guid servicioId, string descripcion, decimal precio, int cantidad, string tipoServicio, string usuarioCarga)
        {
            if (Estado != "Abierta")
                throw new InvalidOperationException("No se pueden agregar servicios a una cuenta que no está abierta.");

            var detalle = new DetalleServicioCuenta(Id, servicioId, descripcion, precio, cantidad, tipoServicio, usuarioCarga);
            _detalles.Add(detalle);
        }

        public decimal CalcularTotal() => _detalles.Sum(d => d.Precio * d.Cantidad);

        public void Facturar()
        {
            if (Estado != "Abierta")
                throw new InvalidOperationException("Solo se pueden facturar cuentas abiertas.");
            
            Estado = "Facturada";
            FechaCierre = DateTime.UtcNow;
        }

        public void Anular()
        {
            Estado = "Anulada";
            FechaCierre = DateTime.UtcNow;
        }
    }
}
