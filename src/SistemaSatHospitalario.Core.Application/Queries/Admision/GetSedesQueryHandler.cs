using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetSedesQueryHandler : IRequestHandler<GetSedesQuery, List<SedeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetSedesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SedeDto>> Handle(GetSedesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Sedes
                .Select(s => new SedeDto
                {
                    Id = s.Id,
                    Codigo = s.Codigo,
                    Nombre = s.Nombre,
                    EsPrincipal = s.EsPrincipal,
                    Activo = s.Activo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
