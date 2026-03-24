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

            var reservas = await _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId && r.HoraPautada.Date == request.Fecha.Date && r.ExpiracionUtc > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            var bloqueos = await _context.BloqueosHorarios
                .Where(b => b.MedicoId == request.MedicoId && b.HoraPautada.Date == request.Fecha.Date)
                .ToListAsync(cancellationToken);

            var start = request.Fecha.Date.AddHours(8); // 8 AM
            var end = request.Fecha.Date.AddHours(18.5); // 6:30 PM (Último turno 6:00 - 6:30)

            for (var current = start; current < end; current = current.AddMinutes(30))
            {
                var citaOcupada = citas.FirstOrDefault(c => c.HoraPautada.TimeOfDay == current.TimeOfDay);
                var reservaVigente = reservas.FirstOrDefault(r => r.HoraPautada.TimeOfDay == current.TimeOfDay);
                var bloqueoAdmin = bloqueos.FirstOrDefault(b => b.HoraPautada.TimeOfDay == current.TimeOfDay);
                
                var comentario = "Disponible";
                if (citaOcupada != null) comentario = citaOcupada.Comentario ?? "Ocupado";
                else if (bloqueoAdmin != null) comentario = bloqueoAdmin.Motivo ?? "Bloqueado Administrativamente";
                else if (reservaVigente != null) comentario = "En proceso de facturación...";

                response.Turnos.Add(new ScheduleEntry
                {
                    Hora = current,
                    Ocupado = citaOcupada != null,
                    Reservado = reservaVigente != null,
                    Bloqueado = bloqueoAdmin != null,
                    Comentario = comentario
                });
            }

            return response;
        }
    }
}
