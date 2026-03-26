using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateConvenioPerfilPrecioCommand : IRequest<bool>
    {
        public int ConvenioId { get; set; }
        public int PerfilId { get; set; }
        public decimal PrecioHNL { get; set; }
        public decimal PrecioUSD { get; set; }
    }

    public class UpdateConvenioPerfilPrecioCommandHandler : IRequestHandler<UpdateConvenioPerfilPrecioCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateConvenioPerfilPrecioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateConvenioPerfilPrecioCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.ConvenioPerfilPrecios
                .FirstOrDefaultAsync(c => c.SeguroConvenioId == request.ConvenioId && c.PerfilId == request.PerfilId, cancellationToken);

            if (existing != null)
            {
                existing.ActualizarPrecio(request.PrecioHNL, request.PrecioUSD);
            }
            else
            {
                var nuevo = new ConvenioPerfilPrecio(request.ConvenioId, request.PerfilId, request.PrecioHNL, request.PrecioUSD);
                _context.ConvenioPerfilPrecios.Add(nuevo);
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
