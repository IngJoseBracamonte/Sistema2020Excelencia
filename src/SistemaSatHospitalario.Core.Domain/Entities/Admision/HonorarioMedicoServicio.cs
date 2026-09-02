using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioMedicoServicio
    {
        public Guid Id { get; private set; }
        public Guid ServicioId { get; private set; }
        public virtual ServicioClinico Servicio { get; private set; }
        public Guid MedicoId { get; private set; }
        public virtual Medico Medico { get; private set; }
        public decimal MontoHonorario { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioModificoId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioModificoId. Columna legacy pendiente de DROP.")]
        public string UsuarioModifico { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que modificó el honorario.</summary>
        public Guid? UsuarioModificoId { get; private set; }
        public DateTime FechaModificacion { get; private set; }

        protected HonorarioMedicoServicio() { }

        public HonorarioMedicoServicio(Guid servicioId, Guid medicoId, decimal montoHonorario, string usuario)
        {
            Id = Guid.NewGuid();
            ServicioId = servicioId;
            MedicoId = medicoId;
            MontoHonorario = montoHonorario;
            SetUsuarioModifico(usuario, null);
            FechaModificacion = DateTime.UtcNow;
        }

        public void ActualizarHonorario(decimal nuevoMonto, string usuario)
        {
            MontoHonorario = nuevoMonto;
            SetUsuarioModifico(usuario, null);
            FechaModificacion = DateTime.UtcNow;
        }

        private void SetUsuarioModifico(string usuario, Guid? usuarioId)
        {
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioModifico = usuario;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioModificoId = usuarioId ?? (Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null);
        }
    }
}
