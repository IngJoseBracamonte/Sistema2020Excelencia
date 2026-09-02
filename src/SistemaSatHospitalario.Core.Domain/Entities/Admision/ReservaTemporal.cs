using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ReservaTemporal
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime HoraPautada { get; private set; }

        /// <summary>
        /// LEGACY (3FN): ID de usuario como texto. Fuente de verdad:
        /// <see cref="UsuarioIdentityId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioIdentityId. Columna legacy pendiente de DROP.")]
        public string UsuarioId { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que hizo la reserva.</summary>
        public Guid? UsuarioIdentityId { get; private set; }
        public string? Comentario { get; private set; }
        public DateTime ExpiracionUtc { get; private set; }

        protected ReservaTemporal() { }

        public ReservaTemporal(Guid medicoId, DateTime horaPautada, string usuarioId, string? comentario = null, int minutosValidez = 15)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            HoraPautada = horaPautada;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioId = usuarioId;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
            Comentario = comentario;
            ExpiracionUtc = DateTime.UtcNow.AddMinutes(minutosValidez);
        }

        public bool EstaExpirada() => DateTime.UtcNow > ExpiracionUtc;
        public void ActualizarHoraPautada(DateTime nuevaHora) => HoraPautada = nuevaHora;
    }
}
