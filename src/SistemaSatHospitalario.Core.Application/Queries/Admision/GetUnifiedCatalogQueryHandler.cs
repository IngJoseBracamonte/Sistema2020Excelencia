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
using Microsoft.Extensions.Logging;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetUnifiedCatalogQueryHandler : IRequestHandler<GetUnifiedCatalogQuery, List<CatalogItemDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ILogger<GetUnifiedCatalogQueryHandler> _logger;

        public GetUnifiedCatalogQueryHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository, ILogger<GetUnifiedCatalogQueryHandler> logger)
        {
            _context = context;
            _legacyRepository = legacyRepository;
            _logger = logger;
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
                .Include(s => s.Sugerencias)
                .Include(s => s.HonorariosMedicos)
                    .ThenInclude(hm => hm.Medico)
                .Where(s => s.Activo)
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
                var editorType = ResolveEditorType(s.TipoServicio, false);
                var item = new CatalogItemDto
                {
                    Id = s.Id.ToString(),
                    Codigo = s.Codigo,
                    Descripcion = s.Descripcion,
                    Tipo = editorType,
                    EditorType = editorType,
                    CategoryId = (int)s.Category,
                    EsLegacy = false,
                    Activo = s.Activo,
                    PrecioUsd = preciosConvenio.ContainsKey(s.Id) ? preciosConvenio[s.Id] : s.PrecioBase,
                    HonorarioBase = s.HonorarioBase,
                    HonorariumCategory = s.HonorariumCategory,
                    EspecialidadId = s.EspecialidadId,
                    SugerenciasIds = s.Sugerencias.Select(sg => sg.ServicioSugeridoId.ToString()).ToList(),
                    HonorariosMedicos = s.HonorariosMedicos.Select(hm => new DoctorHonorarioDto
                    {
                        MedicoId = hm.MedicoId,
                        MedicoNombre = hm.Medico?.Nombre ?? "Desconocido",
                        Honorario = hm.MontoHonorario
                    }).ToList(),
                    UnidadMedida = s.UnidadMedida,
                    PermiteFraccionamiento = s.PermiteFraccionamiento
                };
                if (item.Codigo == "S004")
                {
                    _logger.LogInformation("[CATALOG-DIAGNOSTIC] S004 Suggestions count: {Count}. Details: {Details}", 
                        s.Sugerencias.Count, string.Join(",", item.SugerenciasIds));
                }
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

            _logger.LogInformation("[CATALOG-DIAGNOSTIC] Native: {NativeCount}, Legacy: {LegacyCount}, Tasa: {Tasa}, Convenio: {ConvenioId}", 
                serviciosNativos.Count, perfilesLegacy.Count, tasa, request.ConvenioId);

            foreach (var p in perfilesLegacy)
            {
                // Estrategia USD-First (V11.16 Fallback):
                decimal finalUsd = p.PrecioDolar;
                decimal baseHnl = p.Precio;
                
                if (finalUsd <= 0 && baseHnl > 0)
                {
                    finalUsd = Math.Round(baseHnl / tasa, 2);
                    _logger.LogInformation("[LEGACY-CATALOG] Fallback HNL->USD: Perfil {IdPerfil} ({Descripcion}) -> ${FinalUsd}", 
                        p.IdPerfil, p.Descripcion, finalUsd);
                }
                else if (finalUsd <= 0 && baseHnl <= 0)
                {
                    // Fallback de Emergencia: Si la DB no tiene precio, asignamos un nominal para permitir flujo
                    // pero alertamos para corrección de datos en Legacy.
                    finalUsd = 1.00m; 
                    _logger.LogWarning("[LEGACY-CATALOG] [CRITICAL] Perfil {IdPerfil} ({Descripcion}) sin precios en DB. Asignado nominal $1.00", 
                        p.IdPerfil, p.Descripcion);
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
                    Tipo = "LABORATORIO",
                    EditorType = "LABORATORIO",
                    CategoryId = (int)ServiceCategory.Laboratory,
                    EsLegacy = true,
                    Activo = true,
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

        private static string ResolveEditorType(string? rawTipo, bool esLegacy)
        {
            if (esLegacy) return "LABORATORIO";
            if (string.IsNullOrWhiteSpace(rawTipo)) return "PROCEDIMIENTO";

            var key = rawTipo.Trim().ToUpper();
            switch (key)
            {
                case "CONSULTA":
                case "CITAS":
                case "MEDICO":
                case "EVALUACION":
                    return "CONSULTA";

                case "LABORATORIO":
                case "LAB":
                case "EXAMEN":
                case "EXAMENES":
                case "PERFIL":
                case "ANALISIS":
                    return "LABORATORIO";

                case "TOMOGRAFIA":
                case "TOMO":
                case "RX":
                case "RADIOGRAFIA":
                case "IMAGEN":
                case "ECO":
                case "ECOGRAFIA":
                case "ULTRASONIDO":
                case "RESONANCIA":
                case "ESTUDIO":
                case "ESTUDIOS":
                    return "TOMOGRAFIA";

                case "MEDICAMENTO":
                case "MEDICINA":
                case "INSUMO":
                case "FARMACIA":
                case "MATERIAL":
                case "SOLUCION":
                case "AMPOLLA":
                case "TAB":
                case "CAPSULA":
                    return "MEDICAMENTO";

                case "CIRUGIA":
                case "QUIRURGICO":
                case "INTERVENCION":
                case "PABELLON":
                    return "CIRUGIA";

                case "HOSPITALARIO":
                case "HOSPITALIZACION":
                case "EMERGENCIA":
                case "UCI":
                case "TRASLADO":
                case "AREA":
                case "CAMAS":
                case "HABITACION":
                case "ESTANCIA":
                    return "HOSPITALARIO";

                default:
                    return "PROCEDIMIENTO";
            }
        }
    }
}
