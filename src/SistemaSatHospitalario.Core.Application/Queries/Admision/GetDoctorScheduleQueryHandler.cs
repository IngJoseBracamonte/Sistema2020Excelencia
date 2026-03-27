using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

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
            // V11.1 Identity Alignment: Uso directo de GUID nativo
            Guid? internalPacienteId = request.PacienteId;

            var response = new DoctorScheduleResponse
            {
                MedicoId = request.MedicoId,
                Fecha = request.Fecha.Date,
                Turnos = new List<ScheduleEntry>()
            };

            // 1. Obtener citas, reservas y bloqueos para ese doctor y día (Rango optimizado V11.8)
            var today = request.Fecha.Date;
            var tomorrow = today.AddDays(1);

            var citas = await _context.CitasMedicas
                .Where(c => c.MedicoId == request.MedicoId 
                         && c.HoraPautada >= today && c.HoraPautada < tomorrow 
                         && c.Estado != EstadoConstants.Cancelado)
                .ToListAsync(cancellationToken);

            var reservas = await _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId 
                         && r.HoraPautada >= today && r.HoraPautada < tomorrow 
                         && r.ExpiracionUtc > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            var bloqueos = await _context.BloqueosHorarios
                .Where(b => b.MedicoId == request.MedicoId 
                         && b.HoraPautada >= today && b.HoraPautada < tomorrow)
                .ToListAsync(cancellationToken);

            // Normalizar inicio a precisión estable (Micro-Ciclo 40 Optimization)
            var baseDate = request.Fecha.Date;
            var start = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 8, 0, 0, DateTimeKind.Unspecified); // 8 AM exacto
            var end = start.AddHours(10.5); // 6:30 PM (Último turno 6:00 - 6:30)

            for (var current = start; current < end; current = current.AddMinutes(30))
            {
                // Comparación robusta por DateTime completo (Normalizado a minutos)
                var citaOcupada = citas.FirstOrDefault(c => c.HoraPautada == current);
                var reservaVigente = reservas.FirstOrDefault(r => r.HoraPautada == current);
                var bloqueoAdmin = bloqueos.FirstOrDefault(b => b.HoraPautada == current);
                
                // Determinamos si el slot pertenece al contexto actual (V4.8 Precision)
                bool esCitaPropia = (internalPacienteId.HasValue && citaOcupada?.PacienteId == internalPacienteId.Value);
                                 
                bool esReservaPropia = request.UsuarioId != null && reservaVigente?.UsuarioId == request.UsuarioId;
                
                string comentario = EstadoConstants.LabelLibre;
                Guid? targetId = null;
                string? type = null;

                if (citaOcupada != null)
                {
                    comentario = esCitaPropia ? EstadoConstants.LabelTuCita : (citaOcupada.Comentario ?? EstadoConstants.LabelOcupado);
                    targetId = citaOcupada.Id;
                    type = EstadoConstants.TypeCita;
                }
                else if (bloqueoAdmin != null) 
                {
                    comentario = bloqueoAdmin.Motivo ?? EstadoConstants.LabelBloqueadoAdmin;
                    targetId = bloqueoAdmin.Id;
                    type = EstadoConstants.TypeBloqueo;
                }
                else if (reservaVigente != null) 
                {
                    comentario = esReservaPropia 
                        ? (string.IsNullOrEmpty(reservaVigente.Comentario) ? EstadoConstants.LabelTuReserva : reservaVigente.Comentario) 
                        : EstadoConstants.LabelEnProceso;
                    targetId = reservaVigente.Id;
                    type = EstadoConstants.TypeReserva;
                }

                response.Turnos.Add(new ScheduleEntry
                {
                    Hora = current,
                    Ocupado = citaOcupada != null && !esCitaPropia,
                    Reservado = (reservaVigente != null && !esReservaPropia) || (citaOcupada != null && !esCitaPropia),
                    Bloqueado = bloqueoAdmin != null,
                    Comentario = comentario,
                    TargetId = targetId,
                    Type = type
                });
            }

            return response;
        }
    }
}
