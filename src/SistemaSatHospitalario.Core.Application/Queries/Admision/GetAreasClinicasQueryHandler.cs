using System;
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
            var queryable = _context.AreasClinicas
                .Include(a => a.Sede)
                .Include(a => a.ServicioTarifaBase)
                .AsNoTracking();

            if (request.SoloActivas)
            {
                queryable = queryable.Where(a => a.Activo);
            }

            if (request.SedeId.HasValue && request.SedeId.Value != Guid.Empty)
            {
                queryable = queryable.Where(a => a.SedeId == request.SedeId.Value);
            }

            return await queryable
                .Select(a => new AreaClinicaDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Codigo = a.Codigo,
                    Activo = a.Activo,
                    SedeId = a.SedeId,
                    SedeNombre = a.Sede != null ? a.Sede.Nombre : string.Empty,
                    EsSubAreaAlmacenPrincipal = a.EsSubAreaAlmacenPrincipal,
                    AreaPadreId = a.AreaPadreId,
                    ServicioTarifaBaseId = a.ServicioTarifaBaseId,
                    TarifaBasePrecioUsd = a.ServicioTarifaBase != null ? a.ServicioTarifaBase.PrecioBase : 0m,
                    TarifaBaseHonorarioUsd = a.ServicioTarifaBase != null ? a.ServicioTarifaBase.HonorarioBase : 0m
                })
                .ToListAsync(cancellationToken);
        }
    }
}