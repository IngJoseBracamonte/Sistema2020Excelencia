using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteSedeCommandHandler : IRequestHandler<DeleteSedeCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSedeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSedeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new InvalidOperationException("La sede no existe.");
            }

            entity.SetEstado(false);

            // Cascade inactivation to all its AreasClinicas in database
            var areas = await _context.AreasClinicas
                .Where(a => a.SedeId == request.Id)
                .ToListAsync(cancellationToken);

            foreach (var area in areas)
            {
                area.SetEstado(false);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
