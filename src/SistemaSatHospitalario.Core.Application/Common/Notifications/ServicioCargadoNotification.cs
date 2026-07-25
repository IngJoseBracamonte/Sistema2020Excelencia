using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Common.Notifications
{
    public class ServicioCargadoNotification : INotification
    {
        public string TipoServicio { get; }
        public string OrigenCarga { get; }
        public Guid PacienteId { get; }
        public string NombrePaciente { get; }
        public string AreaOrigen { get; }
        public string ServicioDescripcion { get; }
        public Guid? MedicoId { get; }
        public DateTime? HoraCita { get; }
        public DateTime FechaCarga { get; }

        public ServicioCargadoNotification(
            string tipoServicio,
            string origenCarga,
            Guid pacienteId,
            string nombrePaciente,
            string areaOrigen,
            string servicioDescripcion,
            Guid? medicoId = null,
            DateTime? horaCita = null)
        {
            TipoServicio = tipoServicio ?? string.Empty;
            OrigenCarga = origenCarga ?? string.Empty;
            PacienteId = pacienteId;
            NombrePaciente = nombrePaciente ?? string.Empty;
            AreaOrigen = areaOrigen ?? string.Empty;
            ServicioDescripcion = servicioDescripcion ?? string.Empty;
            MedicoId = medicoId;
            HoraCita = horaCita;
            FechaCarga = DateTime.UtcNow;
        }
    }
}
