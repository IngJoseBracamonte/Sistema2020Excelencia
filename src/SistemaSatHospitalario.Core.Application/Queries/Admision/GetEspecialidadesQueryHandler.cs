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
    public class GetEspecialidadesQueryHandler : IRequestHandler<GetEspecialidadesQuery, List<EspecialidadDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEspecialidadesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EspecialidadDto>> Handle(GetEspecialidadesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Especialidades
                .Select(e => new EspecialidadDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Activo = e.Activo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
