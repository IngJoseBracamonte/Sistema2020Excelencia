using System;
using System.ComponentModel.DataAnnotations.Schema;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public abstract class OrdenDeServicio
    {
        public Guid Id { get; protected set; }
        public int NumeroLlegadaDiario { get; protected set; }
        public Guid PacienteId { get; protected set; }
        public string TipoIngreso { get; protected set; } // Particular, Seguro, Hospitalizacion, Emergencia
        public EstadoFacturacion EstadoFacturacion { get; protected set; }
        public DateTime FechaCreacion { get; protected set; }
        public int? ConvenioId { get; protected set; }

        [ForeignKey(nameof(PacienteId))]
        public virtual Admision.PacienteAdmision? Paciente { get; protected set; }

        protected OrdenDeServicio() { }

        protected OrdenDeServicio(int numeroLlegada, Guid pacienteId, string tipoIngreso, int? convenioId = null)
        {
            Id = Guid.NewGuid();
            NumeroLlegadaDiario = numeroLlegada;
            PacienteId = pacienteId;
            TipoIngreso = tipoIngreso ?? throw new ArgumentNullException(nameof(tipoIngreso));
            EstadoFacturacion = EstadoFacturacion.FacturaFiscal;
            FechaCreacion = DateTime.UtcNow;
            ConvenioId = convenioId;
        }

        public void AsignarConvenio(int convenioId)
        {
            ConvenioId = convenioId;
        }

        /// <summary>
        /// Estrategia de cálculo en Dominio:
        /// Cada tipo de orden (Rx, Lab, etc.) implementa la suma de sus propios detalles.
        /// </summary>
        public abstract decimal CalcularTotal();
    }
}