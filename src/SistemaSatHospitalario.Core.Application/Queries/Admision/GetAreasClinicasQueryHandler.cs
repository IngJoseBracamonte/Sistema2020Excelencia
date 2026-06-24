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
    public class GetAreasClinicasQueryHandler : IRequestHandler<GetAreasClinicasQuery, List<AreaClinicaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAreasClinicasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AreaClinicaDto>> Handle(GetAreasClinicasQuery request, CancellationToken cancellationToken)
        {
            var query = _context.AreasClinicas.AsQueryable();

            if (request.SedeId.HasValue)
            {
                query = query.Where(a => a.SedeId == request.SedeId.Value);
            }

            return await query
                .Select(a => new AreaClinicaDto
                {
                    Id = a.Id,
                    SedeId = a.SedeId,
                    SedeNombre = a.Sede.Nombre,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Activo = a.Activo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
