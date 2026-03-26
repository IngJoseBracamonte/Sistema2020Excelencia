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

            // Obtener la tasa de cambio más reciente
            var tasa = await _context.TasaCambio
                .OrderByDescending(t => t.Fecha)
                .Select(t => t.Monto)
                .FirstOrDefaultAsync(cancellationToken);

            if (tasa <= 0) tasa = 1; // Fallback de seguridad

            // 1. Obtener servicios nativos (RX, Consultas, etc.)
            var serviciosNativos = await _context.ServiciosClinicos
                .Where(s => s.Activo && s.Descripcion != "LABORATORIO")
                .ToListAsync(cancellationToken);

            // 2. Obtener precios por convenio si aplica
            var preciosConvenio = new Dictionary<Guid, decimal>();
            if (request.ConvenioId.HasValue)
            {
                preciosConvenio = await _context.PreciosServicioConvenio
                    .Where(p => p.SeguroConvenioId == request.ConvenioId.Value)
                    .ToDictionaryAsync(p => p.ServicioClinicoId, p => p.PrecioDiferencial, cancellationToken);
            }

            // 3. Mapear servicios nativos (Asumimos PrecioBase en USD)
            foreach (var s in serviciosNativos)
            {
                var item = new CatalogItemDto
                {
                    Id = s.Id.ToString(),
                    Codigo = s.Codigo,
                    Descripcion = s.Descripcion,
                    Tipo = s.TipoServicio,
                    EsLegacy = false,
                    PrecioUsd = preciosConvenio.ContainsKey(s.Id) ? preciosConvenio[s.Id] : s.PrecioBase
                };
                item.CalculatePrices(tasa);
                result.Add(item);
            }

            // 4. Obtener perfiles de Laboratorio del sistema Legacy
            var perfilesLegacy = await _legacyRepository.GetAvailableProfilesAsync(cancellationToken);
            
            // 5. Obtener excepciones de precios para perfiles legacy
            var excepcionesPerfiles = new Dictionary<int, (decimal hnl, decimal usd)>();
            if (request.ConvenioId.HasValue)
            {
                excepcionesPerfiles = await _context.ConvenioPerfilPrecios
                    .Where(x => x.SeguroConvenioId == request.ConvenioId.Value)
                    .ToDictionaryAsync(x => x.PerfilId, x => (x.PrecioHNL, x.PrecioUSD), cancellationToken);
            }

            foreach (var p in perfilesLegacy)
            {
                decimal finalUsd = (decimal)p.PrecioDOlar;
                decimal? overrideHnl = null;

                if (excepcionesPerfiles.TryGetValue(p.IdPerfil, out var exc))
                {
                    if (exc.usd > 0) finalUsd = exc.usd;
                    if (exc.hnl > 0) overrideHnl = exc.hnl;
                }

                var item = new CatalogItemDto
                {
                    Id = p.IdPerfil.ToString(),
                    Codigo = "LAB-" + p.IdPerfil,
                    Descripcion = p.Descripcion,
                    Tipo = "LABORATORIO",
                    EsLegacy = true,
                    PrecioUsd = finalUsd
                };

                // Si hay un precio específico en HNL para este convenio, lo forzamos después del cálculo base
                item.CalculatePrices(tasa);
                if (overrideHnl.HasValue)
                {
                    item.PrecioBs = overrideHnl.Value;
                    item.Precio = item.PrecioBs;
                }

                result.Add(item);
            }

            return result;
        }
    }
}
