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
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Constants;

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
            // V11.16 Senior Fix: Filtramos por Categoría, no por Descripción (Evita colisiones con stubs de $0.00)
            var serviciosNativos = await _context.ServiciosClinicos
                .Where(s => s.Activo && s.Category != ServiceCategory.Laboratory)
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
                    CategoryId = (int)s.Category,
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

            Console.WriteLine($"[CATALOG-DIAGNOSTIC] Native: {serviciosNativos.Count}, Legacy: {perfilesLegacy.Count}, Tasa: {tasa}, Convenio: {request.ConvenioId}");

            foreach (var p in perfilesLegacy)
            {
                // Estrategia USD-First (V11.16 Fallback):
                decimal finalUsd = p.PrecioDolar;
                decimal baseHnl = p.Precio;
                
                if (finalUsd <= 0 && baseHnl > 0)
                {
                    finalUsd = Math.Round(baseHnl / tasa, 2);
                    Console.WriteLine($"[LEGACY-CATALOG] Fallback HNL->USD: Perfil {p.IdPerfil} ({p.Descripcion}) -> ${finalUsd}");
                }
                else if (finalUsd <= 0 && baseHnl <= 0)
                {
                    // Fallback de Emergencia: Si la DB no tiene precio, asignamos un nominal para permitir flujo
                    // pero alertamos para corrección de datos en Legacy.
                    finalUsd = 1.00m; 
                    Console.WriteLine($"[LEGACY-CATALOG] [CRITICAL] Perfil {p.IdPerfil} ({p.Descripcion}) sin precios en DB. Asignado nominal $1.00");
                }

                decimal? overrideHnl = null;

                // Verificación de Excepciones del Convenio
                if (excepcionesPerfiles.TryGetValue(p.IdPerfil, out var exc))
                {
                    // Si el convenio tiene un valor explícito lo usamos (incluyendo 0 si es intencional del seguro)
                    if (exc.usd > 0) finalUsd = exc.usd;
                    if (exc.hnl > 0) overrideHnl = exc.hnl;
                }

                var item = new CatalogItemDto
                {
                    Id = p.IdPerfil.ToString(),
                    Codigo = EstadoConstants.PrefixLab + p.IdPerfil,
                    Descripcion = p.Descripcion,
                    Tipo = EstadoConstants.Laboratorio,
                    CategoryId = (int)ServiceCategory.Laboratory,
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
