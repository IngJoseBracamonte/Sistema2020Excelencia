using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateSedeCommandHandler : IRequestHandler<CreateSedeCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateSedeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateSedeCommand request, CancellationToken cancellationToken)
        {
            if (request.EsPrincipal)
            {
                var principalExists = await _context.Sedes
                    .AnyAsync(s => s.EsPrincipal && s.Activo, cancellationToken);
                if (principalExists)
                {
                    throw new InvalidOperationException("Ya existe una sede principal activa.");
                }
            }

            var codeExists = await _context.Sedes
                .AnyAsync(s => s.Codigo == request.Codigo, cancellationToken);
            if (codeExists)
            {
                throw new InvalidOperationException($"Ya existe una sede con el código '{request.Codigo}'.");
            }

            var entity = new Sede(request.Codigo, request.Nombre, request.EsPrincipal);
            _context.Sedes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}
