using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteEspecialidadCommandHandler : IRequestHandler<DeleteEspecialidadCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteEspecialidadCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteEspecialidadCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Especialidades.FindAsync(request.Id);
            if (entity == null) return;

            _context.Especialidades.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
