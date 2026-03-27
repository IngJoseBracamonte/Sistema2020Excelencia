using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class ReservarTurnoTemporalCommand : IRequest<bool>
    {
        public Guid MedicoId { get; set; }
        public DateTime HoraPautada { get; set; }
        public string? UsuarioId { get; set; }
        public string? Comentario { get; set; }
    }

    public class ReservarTurnoTemporalCommandHandler : IRequestHandler<ReservarTurnoTemporalCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ReservarTurnoTemporalCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ReservarTurnoTemporalCommand request, CancellationToken cancellationToken)
        {
            // Normalizar a precisión de minuto para comparativa estable en DB y MySQL
            var targetHora = new DateTime(request.HoraPautada.Year, request.HoraPautada.Month, request.HoraPautada.Day, 
                                          request.HoraPautada.Hour, request.HoraPautada.Minute, 0, DateTimeKind.Unspecified);

            if (request.MedicoId == Guid.Empty)
                throw new InvalidOperationException("El ID del médico es requerido.");

            // 1. Limpiar reservas expiradas
            var expiradas = await _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId && r.ExpiracionUtc < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (expiradas.Any())
            {
                _context.ReservasTemporales.RemoveRange(expiradas);
                // No guardamos cambios aún, esperamos al final del handle
            }

            // 2. Verificar que no haya una cita real ya agendada
            var citaExistente = await _context.CitasMedicas.AnyAsync(c => 
                c.MedicoId == request.MedicoId && 
                c.HoraPautada == targetHora && 
                c.Estado != "Cancelado", cancellationToken);

            if (citaExistente) 
                throw new InvalidOperationException("Ya existe una cita médica(En Espera/Atendida) en este horario exacto.");

            // 3. Verificar si hay reserva vigente (Excluyendo al mismo usuario para permitir re-agendamiento tras refresh)
            var reservaMismaHora = await _context.ReservasTemporales.FirstOrDefaultAsync(r => 
                r.MedicoId == request.MedicoId && 
                r.HoraPautada == targetHora &&
                r.ExpiracionUtc > DateTime.UtcNow, cancellationToken);

            if (reservaMismaHora != null)
            {
                if (reservaMismaHora.UsuarioId != request.UsuarioId)
                {
                    throw new InvalidOperationException("Este turno ya está siendo procesado por otro cajero. Intente en un momento.");
                }
                else
                {
                    // Si es el mismo usuario, eliminar la anterior con DELETE directo
                    await _context.ReservasTemporales
                        .Where(r => r.Id == reservaMismaHora.Id)
                        .ExecuteDeleteAsync(cancellationToken);
                }
            }

            try 
            {
                // 4. Crear nueva reserva
                var nuevaReserva = new ReservaTemporal(request.MedicoId, targetHora, request.UsuarioId ?? "Anonimo", request.Comentario);
                _context.ReservasTemporales.Add(nuevaReserva);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error interno de base de datos al reservar: {ex.Message}");
            }
        }
    }
}


