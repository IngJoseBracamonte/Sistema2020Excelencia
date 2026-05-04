using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetMappingRulesQuery : IRequest<List<HonorariumMappingRule>>
    {
    }

    public class GetMappingRulesQueryHandler : IRequestHandler<GetMappingRulesQuery, List<HonorariumMappingRule>>
    {
        private readonly IApplicationDbContext _context;

        public GetMappingRulesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HonorariumMappingRule>> Handle(GetMappingRulesQuery request, CancellationToken cancellationToken)
        {
            return await _context.HonorariumMappingRules
                .OrderBy(r => r.Priority)
                .ToListAsync(cancellationToken);
        }
    }
}
