using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Application.Queries.System
{
    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, List<ErrorTicket>>
    {
        private readonly IApplicationDbContext _context;

        public GetTicketsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ErrorTicket>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ErrorTickets.AsNoTracking();

            if (request.Resueltos.HasValue)
            {
                query = query.Where(t => t.Resuelto == request.Resueltos.Value);
            }

            return await query
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync(cancellationToken);
        }
    }
}
