using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class RegistroAuditoriaIncidencia
    {
        public Guid Id { get; private set; }
        public Guid TurnoMedicoId { get; private set; }
        public Guid IncidenciaIgnoradaId { get; private set; }
        public Guid OperadorId { get; private set; }
        public DateTime FechaTraza { get; private set; }
        public string Motivo { get; private set; }

        private RegistroAuditoriaIncidencia() { }

        public RegistroAuditoriaIncidencia(Guid turnoId, Guid incidenciaId, Guid operadorId, string motivo = "Sobreescritura manual de agenda")
        {
            Id = Guid.NewGuid();
            TurnoMedicoId = turnoId;
            IncidenciaIgnoradaId = incidenciaId;
            OperadorId = operadorId;
            FechaTraza = DateTime.UtcNow;
            Motivo = motivo;
        }
    }
}
