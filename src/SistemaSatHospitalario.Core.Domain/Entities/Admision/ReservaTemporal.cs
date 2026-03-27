using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ReservaTemporal
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime HoraPautada { get; private set; }
        public string UsuarioId { get; private set; }
        public string? Comentario { get; private set; }
        public DateTime ExpiracionUtc { get; private set; }

        protected ReservaTemporal() { }

        public ReservaTemporal(Guid medicoId, DateTime horaPautada, string usuarioId, string? comentario = null, int minutosValidez = 15)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            HoraPautada = horaPautada;
            UsuarioId = usuarioId;
            Comentario = comentario;
            ExpiracionUtc = DateTime.UtcNow.AddMinutes(minutosValidez);
        }

        public bool EstaExpirada() => DateTime.UtcNow > ExpiracionUtc;
        public void ActualizarHoraPautada(DateTime nuevaHora) => HoraPautada = nuevaHora;
    }
}
