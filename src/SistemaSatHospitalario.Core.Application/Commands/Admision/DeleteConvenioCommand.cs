using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteConvenioCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteConvenioCommandHandler : IRequestHandler<DeleteConvenioCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteConvenioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteConvenioCommand request, CancellationToken cancellationToken)
        {
            var convenio = await _context.SegurosConvenios.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (convenio == null) return false;

            _context.SegurosConvenios.Remove(convenio);
            
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
