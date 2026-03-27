using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AdminManageScheduleCommand : IRequest<bool>
    {
        public string Action { get; set; } = EstadoConstants.ActionDelete; // "Delete", "Update"
        public string Type { get; set; } = EstadoConstants.TypeReserva; // "Reserva", "Cita"
        public Guid TargetId { get; set; }
        public DateTime? NewTime { get; set; }
    }

    public class AdminManageScheduleCommandHandler : IRequestHandler<AdminManageScheduleCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public AdminManageScheduleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AdminManageScheduleCommand request, CancellationToken cancellationToken)
        {
            if (request.Type == EstadoConstants.TypeReserva)
            {
                var reserva = await _context.ReservasTemporales.FirstOrDefaultAsync(r => r.Id == request.TargetId, cancellationToken);
                if (reserva == null) return false;

                if (request.Action == EstadoConstants.ActionDelete)
                {
                    _context.ReservasTemporales.Remove(reserva);
                }
                else if (request.Action == EstadoConstants.ActionUpdate && request.NewTime.HasValue)
                {
                    var targetHora = new DateTime(request.NewTime.Value.Year, request.NewTime.Value.Month, request.NewTime.Value.Day, 
                                                  request.NewTime.Value.Hour, request.NewTime.Value.Minute, 0, DateTimeKind.Unspecified);
                    reserva.ActualizarHoraPautada(targetHora);
                }
            }
            else if (request.Type == EstadoConstants.TypeCita)
            {
                var cita = await _context.CitasMedicas.FirstOrDefaultAsync(c => c.Id == request.TargetId, cancellationToken);
                if (cita == null) return false;

                if (request.Action == EstadoConstants.ActionDelete)
                {
                    cita.Cancelar();
                }
                else if (request.Action == EstadoConstants.ActionUpdate && request.NewTime.HasValue)
                {
                    var targetHora = new DateTime(request.NewTime.Value.Year, request.NewTime.Value.Month, request.NewTime.Value.Day, 
                                                  request.NewTime.Value.Hour, request.NewTime.Value.Minute, 0, DateTimeKind.Unspecified);
                    cita.ActualizarHoraPautada(targetHora);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
