using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin
{
    public class SyncMedicoSchedulesCommand : IRequest<bool>
    {
        public Guid MedicoId { get; set; }
        public List<HorarioBloqueDto> Horarios { get; set; }
    }

    public class HorarioBloqueDto
    {
        public int DiaSemana { get; set; }
        public string Inicio { get; set; } // HH:mm
        public string Fin { get; set; } // HH:mm
    }

    public class SyncMedicoSchedulesCommandHandler : IRequestHandler<SyncMedicoSchedulesCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public SyncMedicoSchedulesCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SyncMedicoSchedulesCommand request, CancellationToken cancellationToken)
        {
            var medic = await _context.Medicos.FindAsync(new object[] { request.MedicoId }, cancellationToken);
            if (medic == null) return false;

            // Remove existing schedules for this medic
            var existing = await _context.HorariosAtencionMedicos
                .Where(h => h.MedicoId == request.MedicoId)
                .ToListAsync(cancellationToken);

            _context.HorariosAtencionMedicos.RemoveRange(existing);

            // Add new blocks
            foreach (var b in request.Horarios)
            {
                if (TimeSpan.TryParse(b.Inicio, out var inicio) && TimeSpan.TryParse(b.Fin, out var fin))
                {
                    var entity = new HorarioAtencionMedico(request.MedicoId, b.DiaSemana, inicio, fin);
                    _context.HorariosAtencionMedicos.Add(entity);
                }
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0 || !request.Horarios.Any();
        }
    }
}
