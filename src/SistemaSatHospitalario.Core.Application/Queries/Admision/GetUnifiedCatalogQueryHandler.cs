using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetUnifiedCatalogQueryHandler : IRequestHandler<GetUnifiedCatalogQuery, List<CatalogItemDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public GetUnifiedCatalogQueryHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<List<CatalogItemDto>> Handle(GetUnifiedCatalogQuery request, CancellationToken cancellationToken)
        {
            var result = new List<CatalogItemDto>();

            // 1. Obtener servicios nativos (RX, Consultas, etc.)
            var serviciosNativos = await _context.ServiciosClinicos
                .Where(s => s.Activo)
                .ToListAsync(cancellationToken);

            // 2. Obtener precios por convenio si aplica
            var preciosConvenio = new Dictionary<Guid, decimal>();
            if (request.ConvenioId.HasValue)
            {
                // SeguroConvenioId ahora es int
                preciosConvenio = await _context.PreciosServicioConvenio
                    .Where(p => p.SeguroConvenioId == request.ConvenioId.Value)
                    .ToDictionaryAsync(p => p.ServicioClinicoId, p => p.PrecioDiferencial, cancellationToken);
            }

            // 3. Mapear servicios nativos aplicando precio de convenio si existe
            foreach (var s in serviciosNativos)
            {
                result.Add(new CatalogItemDto
                {
                    Id = s.Id.ToString(),
                    Codigo = s.Codigo,
                    Descripcion = s.Descripcion,
                    Tipo = s.TipoServicio,
                    EsLegacy = false,
                    Precio = preciosConvenio.ContainsKey(s.Id) ? preciosConvenio[s.Id] : s.PrecioBase
                });
            }

            // 4. Obtener perfiles de Laboratorio del sistema Legacy
            var perfilesLegacy = await _legacyRepository.GetAvailableProfilesAsync(cancellationToken);
            foreach (var p in perfilesLegacy)
            {
                result.Add(new CatalogItemDto
                {
                    Id = p.IdPerfil.ToString(),
                    Codigo = "LAB-" + p.IdPerfil,
                    Descripcion = p.Descripcion,
                    Tipo = "LABORATORIO",
                    EsLegacy = true,
                    Precio = p.Precio // Precio is already decimal and non-nullable
                });
            }

            return result;
        }
    }
}
