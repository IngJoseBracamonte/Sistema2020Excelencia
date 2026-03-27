using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class BloquearHorarioCommand : IRequest<bool>
    {
        public Guid MedicoId { get; set; }
        public DateTime HoraPautada { get; set; }
        public string Motivo { get; set; }
    }

    public class BloquearHorarioCommandHandler : IRequestHandler<BloquearHorarioCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public BloquearHorarioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(BloquearHorarioCommand request, CancellationToken cancellationToken)
        {
            // 1. Verificar si ya está bloqueado
            var existente = _context.BloqueosHorarios.Any(b => 
                b.MedicoId == request.MedicoId && 
                b.HoraPautada == request.HoraPautada);
            
            if (existente) return true; // Ya está bloqueado

            // 2. Verificar si hay una cita (No se puede bloquear si ya hay paciente)
            var conCita = _context.CitasMedicas.Any(c => 
                c.MedicoId == request.MedicoId && 
                c.HoraPautada == request.HoraPautada && 
                c.Estado != "Cancelado");

            if (conCita) return false;

            // 3. Crear el bloqueo
            var bloqueo = new BloqueoHorario(request.MedicoId, request.HoraPautada, request.Motivo);
            _context.BloqueosHorarios.Add(bloqueo);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
