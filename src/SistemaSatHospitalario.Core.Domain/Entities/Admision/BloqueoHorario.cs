using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class BloqueoHorario
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime HoraPautada { get; private set; }
        public string Motivo { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        protected BloqueoHorario() { }

        public BloqueoHorario(Guid medicoId, DateTime horaPautada, string motivo)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            HoraPautada = horaPautada;
            Motivo = motivo ?? "Bloqueo Administrativo";
            FechaRegistro = DateTime.UtcNow;
        }
    }
}
