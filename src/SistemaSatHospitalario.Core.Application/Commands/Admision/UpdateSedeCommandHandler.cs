using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateSedeCommandHandler : IRequestHandler<UpdateSedeCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateSedeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateSedeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new InvalidOperationException("La sede no existe.");
            }

            if (request.EsPrincipal && !entity.EsPrincipal)
            {
                var principalExists = await _context.Sedes
                    .AnyAsync(s => s.EsPrincipal && s.Activo && s.Id != request.Id, cancellationToken);
                if (principalExists)
                {
                    throw new InvalidOperationException("Ya existe otra sede principal activa.");
                }
            }

            entity.Update(request.Codigo, request.Nombre, request.EsPrincipal);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
