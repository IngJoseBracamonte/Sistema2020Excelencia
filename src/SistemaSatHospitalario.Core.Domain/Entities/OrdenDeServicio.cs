using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public abstract class OrdenDeServicio
    {
        public Guid Id { get; protected set; }
        public int NumeroLlegadaDiario { get; protected set; }
        public Guid PacienteId { get; protected set; }
        public string NombrePaciente { get; protected set; }
        public string TipoIngreso { get; protected set; } // Particular, Seguro, Hospitalizacion, Emergencia
        public string EstadoFacturacion { get; protected set; } // Sin Factura, Factura Fiscal
        public decimal TotalCobrado { get; protected set; }
        public DateTime FechaCreacion { get; protected set; }
        
        public Guid? ConvenioId { get; protected set; } // Opcional, asociado a Seguros/Convenios

        protected OrdenDeServicio() { }

        protected OrdenDeServicio(int numeroLlegada, Guid pacienteId, string nombrePaciente, string tipoIngreso, Guid? convenioId = null)
        {
            Id = Guid.NewGuid();
            NumeroLlegadaDiario = numeroLlegada;
            PacienteId = pacienteId;
            NombrePaciente = nombrePaciente ?? throw new ArgumentNullException(nameof(nombrePaciente));
            TipoIngreso = tipoIngreso ?? throw new ArgumentNullException(nameof(tipoIngreso));
            EstadoFacturacion = tipoIngreso == "Seguro" ? "Factura Fiscal" : "Sin Factura";
            FechaCreacion = DateTime.UtcNow;
            TotalCobrado = 0;
            ConvenioId = convenioId;
        }

        public void ActualizarTotalCobrado(decimal nuevoTotal)
        {
            if (nuevoTotal < 0) throw new ArgumentException("El total no puede ser negativo.");
            TotalCobrado = nuevoTotal;
        }

        public void AsignarConvenio(Guid convenioId)
        {
            ConvenioId = convenioId;
        }
    }
}
