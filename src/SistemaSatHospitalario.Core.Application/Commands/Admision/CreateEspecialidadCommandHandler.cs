using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateEspecialidadCommandHandler : IRequestHandler<CreateEspecialidadCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateEspecialidadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateEspecialidadCommand request, CancellationToken cancellationToken)
        {
            var entity = new Especialidad(request.Nombre);
            _context.Especialidades.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}
