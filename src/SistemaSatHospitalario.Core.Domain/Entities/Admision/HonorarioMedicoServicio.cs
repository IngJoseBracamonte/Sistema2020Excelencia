using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioMedicoServicio
    {
        public Guid Id { get; private set; }
        public Guid ServicioId { get; private set; }
        public virtual ServicioClinico Servicio { get; private set; } = null!;
        public Guid MedicoId { get; private set; }
        public virtual Medico Medico { get; private set; } = null!;
        public decimal MontoHonorario { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioModificoId { get; private set; }
        public DateTime FechaModificacion { get; private set; }

        protected HonorarioMedicoServicio() { }

        public HonorarioMedicoServicio(
            Guid servicioId, 
            Guid medicoId, 
            decimal montoHonorario, 
            Guid? usuarioModificoId = null,
            string? usuarioModifico = null)
        {
            if (servicioId == Guid.Empty) throw new ArgumentException("El servicio es obligatorio.", nameof(servicioId));
            if (medicoId == Guid.Empty) throw new ArgumentException("El médico es obligatorio.", nameof(medicoId));

            Id = Guid.NewGuid();
            ServicioId = servicioId;
            MedicoId = medicoId;
            MontoHonorario = montoHonorario;
            SetUsuarioModifico(usuarioModificoId, usuarioModifico);
            FechaModificacion = DateTime.UtcNow;
        }

        public void ActualizarHonorario(decimal nuevoMonto, Guid? usuarioModificoId = null, string? usuarioModifico = null)
        {
            MontoHonorario = nuevoMonto;
            SetUsuarioModifico(usuarioModificoId, usuarioModifico);
            FechaModificacion = DateTime.UtcNow;
        }

        private void SetUsuarioModifico(Guid? usuarioId, string? usuario)
        {
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioModificoId = usuarioId ?? (Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null);
        }
    }
}