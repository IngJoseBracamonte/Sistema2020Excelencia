using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteCatalogItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCatalogItemCommandHandler : IRequestHandler<DeleteCatalogItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCatalogItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCatalogItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ServiciosClinicos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item == null) return false;

            _context.ServiciosClinicos.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
