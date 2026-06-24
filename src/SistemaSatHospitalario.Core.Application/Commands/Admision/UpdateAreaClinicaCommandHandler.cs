using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateAreaClinicaCommandHandler : IRequestHandler<UpdateAreaClinicaCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAreaClinicaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateAreaClinicaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.AreasClinicas
                .Include(a => a.Sede)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new InvalidOperationException("El área clínica no existe.");
            }

            if (request.Activo && !entity.Activo)
            {
                // If attempting to activate, verify parent Sede is active
                var parentSedeActive = await _context.Sedes
                    .AnyAsync(s => s.Id == entity.SedeId && s.Activo, cancellationToken);
                if (!parentSedeActive)
                {
                    throw new InvalidOperationException("No se puede activar un área clínica si su sede asociada está desactivada.");
                }
            }

            entity.Update(request.Codigo, request.Nombre);
            entity.SetEstado(request.Activo);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
