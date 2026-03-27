using System;
using System.Collections.Generic;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDoctorScheduleQuery : IRequest<DoctorScheduleResponse>
    {
        public Guid MedicoId { get; set; }
        public DateTime Fecha { get; set; }

        public string? UsuarioId { get; set; }
        public Guid? PacienteId { get; set; } // Identidad Nativa GUID (V11.1)

        public GetDoctorScheduleQuery(Guid medicoId, DateTime fecha, string? usuarioId = null, Guid? pacienteId = null)
        {
            MedicoId = medicoId;
            Fecha = fecha;
            UsuarioId = usuarioId;
            PacienteId = pacienteId;
        }
    }

    public class DoctorScheduleResponse
    {
        public Guid MedicoId { get; set; }
        public DateTime Fecha { get; set; }
        public List<ScheduleEntry> Turnos { get; set; } = new List<ScheduleEntry>();
    }

    public class ScheduleEntry
    {
        public DateTime Hora { get; set; }
        public bool Ocupado { get; set; }
        public bool Reservado { get; set; }
        public bool Bloqueado { get; set; }
        public string Comentario { get; set; }
        public Guid? TargetId { get; set; }
        public string? Type { get; set; }
    }
}
