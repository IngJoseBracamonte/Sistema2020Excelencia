using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Common;
using SistemaSatHospitalario.Core.Domain.Entities.Admision.Events;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaServicios : BaseEntity
    {
        public Guid Id { get; private set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; private set; }
        public string UsuarioCarga { get; private set; }
        public DateTime FechaCarga { get; private set; }
        public DateTime? FechaCierre { get; private set; }
        public string Estado { get; private set; } // Abierta, Facturada, Anulada
        public string TipoIngreso { get; private set; } // Particular, Seguro, Hospitalizacion, Emergencia
        // Se cambió de Guid? a int? para sincronización con Legacy
        public int? ConvenioId { get; private set; }

        private readonly List<DetalleServicioCuenta> _detalles = new();
        public IReadOnlyCollection<DetalleServicioCuenta> Detalles => _detalles.AsReadOnly();

        protected CuentaServicios() { }

        public CuentaServicios(Guid pacienteId, string usuarioCarga, string tipoIngreso, int? convenioId = null)
        {
            Id = Guid.NewGuid();
            PacienteId = pacienteId;
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
            FechaCarga = DateTime.UtcNow;
            Estado = EstadoConstants.Abierta;
            TipoIngreso = tipoIngreso ?? EstadoConstants.Particular;
            ConvenioId = convenioId;
        }

        public DetalleServicioCuenta AgregarServicio(Guid servicioId, string descripcion, decimal precio, decimal honorario, int cantidad, string tipoServicio, string usuarioCarga, string? legacyMappingId = null)
        {
            if (Estado != EstadoConstants.Abierta)
                throw new InvalidOperationException("No se pueden agregar servicios a una cuenta que no está abierta.");

            var detalle = new DetalleServicioCuenta(Id, servicioId, descripcion, precio, honorario, cantidad, tipoServicio, usuarioCarga, legacyMappingId);
            _detalles.Add(detalle);
            return detalle;
        }

        public void RemoverServicio(Guid servicioId)
        {
            if (Estado != EstadoConstants.Abierta)
                throw new InvalidOperationException("No se pueden remover servicios de una cuenta que no está abierta.");

            var detalle = _detalles.FirstOrDefault(d => d.ServicioId == servicioId);
            if (detalle != null)
            {
                _detalles.Remove(detalle);
            }
        }

        public void RemoverServicioPorDetalleId(Guid detalleId)
        {
            if (Estado != EstadoConstants.Abierta)
                throw new InvalidOperationException("No se pueden remover servicios de una cuenta que no está abierta.");

            var detalle = _detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                _detalles.Remove(detalle);
            }
        }

        public decimal CalcularTotal() => _detalles.Sum(d => d.Precio * d.Cantidad);

        public void Facturar()
        {
            if (Estado != EstadoConstants.Abierta)
                throw new InvalidOperationException("Solo se pueden facturar cuentas abiertas.");
            
            Estado = EstadoConstants.Facturada;
            FechaCierre = DateTime.UtcNow;

            // [PHASE-5] Raise Domain Event for downstream processing
            AddDomainEvent(new CuentaFacturadaEvent(Id, FechaCierre.Value));
        }

        public void Anular()
        {
            Estado = EstadoConstants.Anulada;
            FechaCierre = DateTime.UtcNow;
        }
    }
}
