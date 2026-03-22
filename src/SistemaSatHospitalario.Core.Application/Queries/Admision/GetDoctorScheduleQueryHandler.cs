using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDoctorScheduleQueryHandler : IRequestHandler<GetDoctorScheduleQuery, DoctorScheduleResponse>
    {
        private readonly IApplicationDbContext _context;

        public GetDoctorScheduleQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorScheduleResponse> Handle(GetDoctorScheduleQuery request, CancellationToken cancellationToken)
        {
            var response = new DoctorScheduleResponse
            {
                MedicoId = request.MedicoId,
                Fecha = request.Fecha.Date,
                Turnos = new List<ScheduleEntry>()
            };

            // 1. Obtener citas existentes para ese doctor y día
            var citas = await _context.CitasMedicas
                .Where(c => c.MedicoId == request.MedicoId && c.HoraPautada.Date == request.Fecha.Date && c.EstadoAtencion != "Cancelado")
                .ToListAsync(cancellationToken);

            // 2. Generar slots de agenda (ej. de 8:00 AM a 4:00 PM cada 30 min)
            var start = request.Fecha.Date.AddHours(8);
            var end = request.Fecha.Date.AddHours(16);

            for (var current = start; current < end; current = current.AddMinutes(30))
            {
                var citaOcupada = citas.FirstOrDefault(c => c.HoraPautada.TimeOfDay == current.TimeOfDay);
                
                response.Turnos.Add(new ScheduleEntry
                {
                    Hora = current,
                    Ocupado = citaOcupada != null,
                    Comentario = citaOcupada != null ? "Ocupado" : "Disponible"
                });
            }

            return response;
        }
    }
}
