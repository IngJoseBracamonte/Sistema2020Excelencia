using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommandHandler : IRequestHandler<AgendarTurnoCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public AgendarTurnoCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(AgendarTurnoCommand request, CancellationToken cancellationToken)
        {
            // Normalización para garantizar paridad con el sistema de Reservas y prevenir errores de índice único
            var targetHora = new DateTime(
                request.FechaHoraToma.Year, request.FechaHoraToma.Month, request.FechaHoraToma.Day, 
                request.FechaHoraToma.Hour, request.FechaHoraToma.Minute, 0, 
                DateTimeKind.Unspecified);

            // 1. Validar disponibilidad (Evitar colisión de horario exacto)
            var colisionCita = await _context.CitasMedicas
                .AnyAsync(c => c.MedicoId == request.MedicoId 
                            && c.HoraPautada == targetHora 
                            && c.EstadoAtencion != "Cancelado", 
                            cancellationToken);

            var colisionBloqueo = await _context.BloqueosHorarios
                .AnyAsync(b => b.MedicoId == request.MedicoId && b.HoraPautada == targetHora, cancellationToken);

            if (colisionCita || colisionBloqueo)
            {
                throw new InvalidOperationException("El horario solicitado ya se encuentra ocupado o bloqueado.");
            }

            // 2. Limpiar reservas temporales para este horario al confirmar la cita real
            var reservasAsociadas = await _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId && r.HoraPautada == targetHora)
                .ToListAsync(cancellationToken);
            
            if (reservasAsociadas.Any())
            {
                _context.ReservasTemporales.RemoveRange(reservasAsociadas);
            }

            // 2. Crear Cita Médica
            var cita = new CitaMedica(
                request.MedicoId,
                request.PacienteId,
                request.CuentaServicioId,
                targetHora,
                request.Comentario
            );

            _context.CitasMedicas.Add(cita);

            // 3. Persistir
            await _context.SaveChangesAsync(cancellationToken);

            return cita.Id;
        }
    }
}
