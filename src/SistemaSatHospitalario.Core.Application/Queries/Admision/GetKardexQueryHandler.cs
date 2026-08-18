using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetKardexQueryHandler : IRequestHandler<GetKardexQuery, KardexResultDto>
    {
        private readonly IApplicationDbContext _context;

        public GetKardexQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KardexResultDto> Handle(GetKardexQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _context.MovimientosInsumo
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(m => m.Insumo)
                .Include(m => m.Sede)
                .AsQueryable();

            if (request.SedeId.HasValue && request.SedeId.Value != Guid.Empty)
            {
                baseQuery = baseQuery.Where(m => m.SedeId == request.SedeId.Value);
            }

            if (request.InsumoId.HasValue && request.InsumoId.Value != Guid.Empty)
            {
                baseQuery = baseQuery.Where(m => m.InsumoId == request.InsumoId.Value);
            }

            decimal initialBalance = 0;

            if (request.FechaDesde.HasValue)
            {
                var desde = request.FechaDesde.Value.Date;
                var priorMovements = await baseQuery
                    .Where(m => m.Fecha < desde)
                    .ToListAsync(cancellationToken);

                if (priorMovements.Any())
                {
                    initialBalance = priorMovements.Sum(m => IsEntrada(m.TipoMovimiento) 
                        ? Math.Abs(m.CantidadBase) 
                        : -Math.Abs(m.CantidadBase));
                }
            }

            var currentQuery = baseQuery;

            if (request.FechaDesde.HasValue)
            {
                var desde = request.FechaDesde.Value.Date;
                currentQuery = currentQuery.Where(m => m.Fecha >= desde);
            }

            if (request.FechaHasta.HasValue)
            {
                var hastaExclusive = request.FechaHasta.Value.Date.AddDays(1);
                currentQuery = currentQuery.Where(m => m.Fecha < hastaExclusive);
            }

            var movimientosList = await currentQuery
                .OrderByDescending(m => m.Fecha)
                .ToListAsync(cancellationToken);

            // Fallback para insumo específico cuando no hay movimientos registrados
            if (!request.FechaDesde.HasValue && !movimientosList.Any() && request.InsumoId.HasValue && request.InsumoId.Value != Guid.Empty)
            {
                if (request.SedeId.HasValue && request.SedeId.Value != Guid.Empty)
                {
                    var stockSede = await _context.StocksSedes
                        .Where(s => s.InsumoId == request.InsumoId.Value && s.SedeId == request.SedeId.Value)
                        .Select(s => s.StockActual)
                        .FirstOrDefaultAsync(cancellationToken);
                    initialBalance = stockSede;
                }
                else
                {
                    var stockInsumo = await _context.Insumos
                        .Where(i => i.Id == request.InsumoId.Value)
                        .Select(i => i.StockActual)
                        .FirstOrDefaultAsync(cancellationToken);
                    initialBalance = stockInsumo;
                }
            }

            decimal totalEntradas = movimientosList
                .Where(m => IsEntrada(m.TipoMovimiento))
                .Sum(m => Math.Abs(m.CantidadBase));

            decimal totalSalidas = movimientosList
                .Where(m => !IsEntrada(m.TipoMovimiento))
                .Sum(m => Math.Abs(m.CantidadBase));

            decimal finalBalance = initialBalance + totalEntradas - totalSalidas;

            var result = new KardexResultDto
            {
                InitialBalance = initialBalance,
                TotalEntradas = totalEntradas,
                TotalSalidas = totalSalidas,
                FinalBalance = finalBalance,
                Movimientos = movimientosList.Select(m =>
                {
                    var esEntrada = IsEntrada(m.TipoMovimiento);
                    var cantAbs = Math.Abs(m.CantidadOriginal > 0 ? m.CantidadOriginal : m.CantidadBase);
                    var cantFinal = esEntrada ? cantAbs : -cantAbs;
                    var unidadTxt = m.Insumo != null ? m.Insumo.UnidadMedidaBase.ToString() : m.UnidadMedidaOriginal.ToString();

                    return new KardexMovimientoDto
                    {
                        Id = m.Id,
                        Fecha = m.Fecha,
                        InsumoId = m.InsumoId,
                        InsumoCodigo = m.Insumo != null ? m.Insumo.Codigo : "N/A",
                        InsumoNombre = m.Insumo != null ? m.Insumo.Nombre : "Insumo Desconocido",
                        SedeId = m.SedeId,
                        SedeNombre = m.Sede != null ? m.Sede.Nombre : "Almacén Principal",
                        TipoMovimiento = m.TipoMovimiento.ToString(),
                        Cantidad = cantFinal,
                        CantidadBase = cantFinal,
                        UnidadMedida = string.IsNullOrWhiteSpace(unidadTxt) ? "Unidad" : unidadTxt,
                        Usuario = m.Usuario ?? "Sistema",
                        Motivo = m.Motivo ?? string.Empty,
                        EsEntrada = esEntrada
                    };
                }).ToList()
            };

            return result;
        }

        private static bool IsEntrada(TipoMovimientoInsumo tipo)
        {
            return tipo == TipoMovimientoInsumo.Ingreso ||
                   tipo == TipoMovimientoInsumo.TransferenciaEntrada;
        }

        private static bool IsEntrada(string tipoMovimiento)
        {
            if (string.IsNullOrWhiteSpace(tipoMovimiento)) return false;
            var t = tipoMovimiento.Trim().ToLowerInvariant();
            return t.Contains("ingreso") ||
                   t.Contains("entrada") ||
                   t.Contains("devolucion") ||
                   t.Contains("recepcion") ||
                   t.Contains("compra") ||
                   t.Contains("positivo");
        }
    }
}
