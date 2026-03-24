using MediatR;
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
        public string UsuarioId { get; set; }
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
            // 1. Limpiar reservas expiradas para este médico (mantenimiento preventivo)
            var expiradas = _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId && r.ExpiracionUtc < DateTime.UtcNow)
                .ToList();
            
            if (expiradas.Any())
            {
                _context.ReservasTemporales.RemoveRange(expiradas);
            }

            // 2. Verificar que no haya una cita real ya agendada
            var citaExistente = _context.CitasMedicas.Any(c => 
                c.MedicoId == request.MedicoId && 
                c.HoraPautada == request.HoraPautada && 
                c.EstadoAtencion != "Cancelado");

            if (citaExistente) return false;

            // 3. Verificar que no haya otra reserva temporal vigente
            var reservaExistente = _context.ReservasTemporales.Any(r => 
                r.MedicoId == request.MedicoId && 
                r.HoraPautada == request.HoraPautada &&
                r.ExpiracionUtc > DateTime.UtcNow);

            if (reservaExistente) return false;

            // 4. Crear nueva reserva
            var nuevaReserva = new ReservaTemporal(request.MedicoId, request.HoraPautada, request.UsuarioId);
            _context.ReservasTemporales.Add(nuevaReserva);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
