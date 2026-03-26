using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CitaMedica
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        // Se cambió de Guid a int para sincronización con Legacy
        public int PacienteId { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public DateTime HoraPautada { get; private set; }
        public string EstadoAtencion { get; private set; } // En Espera, Llamado, Atendido, Cancelado
        public string? Comentario { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        protected CitaMedica() { }

        public CitaMedica(Guid medicoId, int pacienteId, Guid cuentaServicioId, DateTime horaPautada, string? comentario = null)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            PacienteId = pacienteId;
            CuentaServicioId = cuentaServicioId;
            HoraPautada = horaPautada;
            EstadoAtencion = "En Espera";
            Comentario = comentario;
            FechaRegistro = DateTime.UtcNow;
        }

        public void ActualizarComentario(string? comentario) => Comentario = comentario;

        public void MarcarComoLlamado() => EstadoAtencion = "Llamado";
        public void MarcarComoAtendido() => EstadoAtencion = "Atendido";
        public void Cancelar() => EstadoAtencion = "Cancelado";
        public void ActualizarHoraPautada(DateTime nuevaHora) => HoraPautada = nuevaHora;
    }
}
