using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ReactivateCatalogItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ReactivateCatalogItemCommandHandler : IRequestHandler<ReactivateCatalogItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ReactivateCatalogItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ReactivateCatalogItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ServiciosClinicos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item == null) return false;

            item.Activar();

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
