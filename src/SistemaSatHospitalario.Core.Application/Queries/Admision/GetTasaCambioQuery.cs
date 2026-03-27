using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetTasaCambioQuery : IRequest<decimal> { }

    public class GetTasaCambioQueryHandler : IRequestHandler<GetTasaCambioQuery, decimal>
    {
        private readonly IApplicationDbContext _context;

        public GetTasaCambioQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> Handle(GetTasaCambioQuery request, CancellationToken cancellationToken)
        {
            var tasa = await _context.TasaCambio
                .OrderByDescending(t => t.Fecha)
                .Select(t => t.Monto)
                .FirstOrDefaultAsync(cancellationToken);

            return tasa > 0 ? tasa : 36.5m; // Fallback si no hay tasa registrada
        }
    }
}
