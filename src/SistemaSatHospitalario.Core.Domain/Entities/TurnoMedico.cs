using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class TurnoMedico
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        public Guid PacienteId { get; private set; }
        public DateTime FechaHoraToma { get; private set; }
        
        // Relación con Incidencias
        public bool IgnorandoIncidencia { get; private set; }
        public Guid? IncidenciaIgnoradaId { get; private set; }

        private TurnoMedico() { }

        public TurnoMedico(Guid medicoId, Guid pacienteId, DateTime fechaHora, bool ignorandoIncidencia = false, Guid? incidenciaId = null)
        {
            if (ignorandoIncidencia && !incidenciaId.HasValue)
                throw new ArgumentException("Debe proveer el Id de la incidencia que está ignorando.");

            Id = Guid.NewGuid();
            MedicoId = medicoId;
            PacienteId = pacienteId;
            FechaHoraToma = fechaHora;
            IgnorandoIncidencia = ignorandoIncidencia;
            IncidenciaIgnoradaId = incidenciaId;
        }
    }
}
