using System;
using System.Collections.Generic;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDoctorScheduleQuery : IRequest<DoctorScheduleResponse>
    {
        public Guid MedicoId { get; set; }
        public DateTime Fecha { get; set; }

        public GetDoctorScheduleQuery(Guid medicoId, DateTime fecha)
        {
            MedicoId = medicoId;
            Fecha = fecha;
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
        public string Comentario { get; set; }
    }
}
