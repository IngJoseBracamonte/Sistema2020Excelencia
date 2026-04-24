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
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

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
            Guid? internalPacienteId = request.PacienteId;

            var medico = await _context.Medicos
                .FirstOrDefaultAsync(m => m.Id == request.MedicoId, cancellationToken);

            if (medico == null) throw new InvalidOperationException("Médico no encontrado.");

            var response = new DoctorScheduleResponse
            {
                MedicoId = request.MedicoId,
                MedicoNombre = medico.Nombre,
                Telefono = medico.Telefono,
                Fecha = request.Fecha.Date,
                Turnos = new List<ScheduleEntry>()
            };

            var today = request.Fecha.Date;
            var tomorrow = today.AddDays(1);

            // 1. Obtener compromisos existentes
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

            var bloqueosIndividuales = await _context.BloqueosHorarios
                .Where(b => b.MedicoId == request.MedicoId 
                         && b.HoraPautada >= today && b.HoraPautada < tomorrow)
                .ToListAsync(cancellationToken);

            // 2. Obtener Indisponibilidades por rango (Vacaciones/Permisos)
            var incidenciasBloqueantes = await _context.IncidenciasHorario
                .Where(i => i.MedicoId == request.MedicoId 
                         && (i.Tipo == TipoComentarioHorario.Bloqueo || i.Tipo == TipoComentarioHorario.Vacaciones)
                         && i.Inicio < tomorrow && i.Fin > today)
                .ToListAsync(cancellationToken);

            // 3. Determinar Rango de Atención (V15.0 Dynamic Logic)
            int diaSemana = (int)today.DayOfWeek;
            var horarios = await _context.HorariosAtencionMedicos
                .Where(h => h.MedicoId == request.MedicoId && h.DiaSemana == diaSemana)
                .ToListAsync(cancellationToken);

            var activeRanges = new List<(TimeSpan Start, TimeSpan End)>();

            if (!horarios.Any())
            {
                // [FALLBACK] Disponibilidad total por defecto (8 AM - 6:30 PM)
                activeRanges.Add((new TimeSpan(8, 0, 0), new TimeSpan(18, 30, 0)));
            }
            else 
            {
                foreach(var h in horarios)
                {
                    activeRanges.Add((h.HoraInicio, h.HoraFin));
                }
            }

            int intervalo = medico.IntervaloTurnoMinutos;

            foreach (var range in activeRanges)
            {
                var start = today.Add(range.Start);
                var end = today.Add(range.End);

                for (var current = start; current < end; current = current.AddMinutes(intervalo))
                {
                    // Verificar si el slot está bloqueado por una incidencia de rango (Vacaciones)
                    var bloqueadoPorRango = incidenciasBloqueantes.Any(i => i.SolapaCon(current));

                    var citaOcupada = citas.FirstOrDefault(c => c.HoraPautada == current);
                    var reservaVigente = reservas.FirstOrDefault(r => r.HoraPautada == current);
                    var bloqueoAdmin = bloqueosIndividuales.FirstOrDefault(b => b.HoraPautada == current);
                    
                    bool esCitaPropia = (internalPacienteId.HasValue && citaOcupada?.PacienteId == internalPacienteId.Value);
                    bool esReservaPropia = request.UsuarioId != null && reservaVigente?.UsuarioId == request.UsuarioId;
                    
                    string comentario = EstadoConstants.LabelLibre;
                    Guid? targetId = null;
                    string? type = null;

                    if (bloqueadoPorRango)
                    {
                        var incidencia = incidenciasBloqueantes.First(i => i.SolapaCon(current));
                        comentario = incidencia.Descripcion ?? "No Disponible (Vacaciones/Permiso)";
                    }
                    else if (citaOcupada != null)
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
                        Ocupado = (citaOcupada != null && !esCitaPropia) || bloqueadoPorRango,
                        Reservado = (reservaVigente != null && !esReservaPropia) || (citaOcupada != null && !esCitaPropia) || bloqueadoPorRango,
                        Bloqueado = bloqueoAdmin != null || bloqueadoPorRango,
                        Comentario = comentario,
                        TargetId = targetId,
                        Type = type
                    });
                }
            }

            // Ordenar por hora (en caso de múltiples rangos)
            response.Turnos = response.Turnos.OrderBy(t => t.Hora).ToList();

            return response;
        }
    }
}
