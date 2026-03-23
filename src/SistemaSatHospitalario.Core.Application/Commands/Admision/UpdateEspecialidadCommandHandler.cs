using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateEspecialidadCommandHandler : IRequestHandler<UpdateEspecialidadCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateEspecialidadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateEspecialidadCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Especialidades.FindAsync(request.Id);
            if (entity == null) return Unit.Value;

            entity.Update(request.Nombre);
            entity.SetEstado(request.Activo);

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
