using System;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CitaMedica
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public DateTime HoraPautada { get; private set; }
        public string Estado { get; private set; } // Pendiente, Confirmada, Cancelada, Atendida
        public string? Comentario { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public Guid? AreaClinicaId { get; private set; }

        public Medico Medico { get; private set; }
        public CuentaServicios CuentaServicio { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }

        protected CitaMedica() { }

        public CitaMedica(Guid medicoId, Guid pacienteId, Guid cuentaServicioId, DateTime horaPautada, string? comentario = null, Guid? areaClinicaId = null)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            PacienteId = pacienteId;
            CuentaServicioId = cuentaServicioId;
            HoraPautada = horaPautada;
            Estado = EstadoConstants.Pendiente;
            Comentario = comentario;
            FechaRegistro = DateTime.UtcNow;
            AreaClinicaId = areaClinicaId;
        }

        public void AsignarAreaClinica(Guid areaClinicaId)
        {
            AreaClinicaId = areaClinicaId;
        }

        public void ActualizarComentario(string? comentario) => Comentario = comentario;

        public void Cancelar() => Estado = EstadoConstants.Cancelado;
        public void ActualizarHoraPautada(DateTime nuevaHora) => HoraPautada = nuevaHora;
        
        public void MarcarComoAtendida()
        {
            Estado = EstadoConstants.Atendida;
        }

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty) throw new ArgumentException("El PacienteId no puede ser vacío.");
            PacienteId = nuevoPacienteId;
        }
    }
}
