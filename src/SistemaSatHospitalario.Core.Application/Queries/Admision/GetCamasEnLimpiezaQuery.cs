using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetCamasEnLimpiezaQuery : IRequest<List<CamaEnLimpiezaDto>>
    {
    }

    public class GetCamasEnLimpiezaQueryHandler : IRequestHandler<GetCamasEnLimpiezaQuery, List<CamaEnLimpiezaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCamasEnLimpiezaQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CamaEnLimpiezaDto>> Handle(GetCamasEnLimpiezaQuery request, CancellationToken cancellationToken)
        {
            return await _context.HistorialesLimpiezasCamas
                .Where(h => h.FechaFin == null)
                .Select(h => new CamaEnLimpiezaDto
                {
                    HistorialId = h.Id,
                    CamaId = h.CamaId,
                    CamaCodigo = h.Cama.Codigo,
                    CamaNombre = h.Cama.Nombre,
                    FechaInicio = h.FechaInicio,
                    UsuarioInicio = h.UsuarioInicio,
                    Observaciones = h.Observaciones
                })
                .ToListAsync(cancellationToken);
        }
    }
}
