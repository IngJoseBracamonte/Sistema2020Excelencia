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
        public string UsuarioModifico { get; private set; }
        public DateTime FechaModificacion { get; private set; }

        protected HonorarioMedicoServicio() { }

        public HonorarioMedicoServicio(Guid servicioId, Guid medicoId, decimal montoHonorario, string usuario)
        {
            Id = Guid.NewGuid();
            ServicioId = servicioId;
            MedicoId = medicoId;
            MontoHonorario = montoHonorario;
            UsuarioModifico = usuario;
            FechaModificacion = DateTime.UtcNow;
        }

        public void ActualizarHonorario(decimal nuevoMonto, string usuario)
        {
            MontoHonorario = nuevoMonto;
            UsuarioModifico = usuario;
            FechaModificacion = DateTime.UtcNow;
        }
    }
}
