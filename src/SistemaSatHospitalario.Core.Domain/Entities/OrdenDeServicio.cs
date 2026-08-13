using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public abstract class OrdenDeServicio
    {
        public Guid Id { get; protected set; }
        public int NumeroLlegadaDiario { get; protected set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; protected set; }
        public string NombrePaciente { get; protected set; }
        public string TipoIngreso { get; protected set; } // Particular, Seguro, Hospitalizacion, Emergencia
        public EstadoFacturacion EstadoFacturacion { get; protected set; }
        public decimal TotalCobrado { get; protected set; }
        public DateTime FechaCreacion { get; protected set; }
        
        // Se cambió de Guid? a int? para sincronización con Legacy
        public int? ConvenioId { get; protected set; } // Opcional, asociado a Seguros/Convenios

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(PacienteId))]
        public virtual Admision.PacienteAdmision? Paciente { get; protected set; }

        protected OrdenDeServicio() { }

        protected OrdenDeServicio(int numeroLlegada, Guid pacienteId, string nombrePaciente, string tipoIngreso, int? convenioId = null)
        {
            Id = Guid.NewGuid();
            NumeroLlegadaDiario = numeroLlegada;
            PacienteId = pacienteId;
            NombrePaciente = nombrePaciente ?? throw new ArgumentNullException(nameof(nombrePaciente));
            TipoIngreso = tipoIngreso ?? throw new ArgumentNullException(nameof(tipoIngreso));
            EstadoFacturacion = tipoIngreso == "Seguro" ? EstadoFacturacion.FacturaFiscal : EstadoFacturacion.SinFactura;
            FechaCreacion = DateTime.UtcNow;
            TotalCobrado = 0;
            ConvenioId = convenioId;
        }

        public void ActualizarTotalCobrado(decimal nuevoTotal)
        {
            if (nuevoTotal < 0) throw new ArgumentException("El total no puede ser negativo.");
            TotalCobrado = nuevoTotal;
        }

        public void AsignarConvenio(int convenioId)
        {
            ConvenioId = convenioId;
        }
    }
}
