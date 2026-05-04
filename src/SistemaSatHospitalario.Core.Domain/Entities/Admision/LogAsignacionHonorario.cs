using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class LogAsignacionHonorario
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public string NombreServicio { get; private set; }
        public string TipoAccion { get; private set; } // ASIGNACION_MANUAL, ASIGNACION_DEFAULT, REASIGNACION, CONFIG_DEFAULT
        public Guid? MedicoAnteriorId { get; private set; }
        public string? MedicoAnteriorNombre { get; private set; }
        public Guid? MedicoNuevoId { get; private set; }
        public string? MedicoNuevoNombre { get; private set; }
        public string UsuarioOperador { get; private set; }
        public DateTime FechaAccion { get; private set; }
        public string? Observaciones { get; private set; }

        protected LogAsignacionHonorario() { }

        public LogAsignacionHonorario(
            Guid detalleServicioId, string nombreServicio, string tipoAccion,
            Guid? medicoAnteriorId, string? medicoAnteriorNombre,
            Guid? medicoNuevoId, string? medicoNuevoNombre,
            string usuarioOperador, string? observaciones = null)
        {
            Id = Guid.NewGuid();
            DetalleServicioId = detalleServicioId;
            NombreServicio = nombreServicio;
            TipoAccion = tipoAccion;
            MedicoAnteriorId = medicoAnteriorId;
            MedicoAnteriorNombre = medicoAnteriorNombre;
            MedicoNuevoId = medicoNuevoId;
            MedicoNuevoNombre = medicoNuevoNombre;
            UsuarioOperador = usuarioOperador;
            FechaAccion = DateTime.UtcNow;
            Observaciones = observaciones;
        }
    }
}
