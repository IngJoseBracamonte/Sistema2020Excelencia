using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    /// <summary>
    /// Implementación de <see cref="ICatalogLookupService"/> con IMemoryCache.
    /// Patrón Cache-Aside: lee de BD solo si no está en caché; invalidación por clave.
    /// </summary>
    public class CatalogLookupService : ICatalogLookupService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        // Claves de caché centralizadas
        private const string KeyUnidadesMedida = "CATALOG_UNIDADES_MEDIDA";
        private const string KeyEstadosCita = "CATALOG_ESTADOS_CITA";
        private const string KeyEstadosCaja = "CATALOG_ESTADOS_CAJA";
        private const string KeyEstadosCuenta = "CATALOG_ESTADOS_CUENTA";
        private const string KeyTiposIngreso = "CATALOG_TIPOS_INGRESO";
        private const string KeyEstadosFiscales = "CATALOG_ESTADOS_FISCALES";
        private const string KeyMotivosAutorizacion = "CATALOG_MOTIVOS_AUTORIZACION";
        private const string KeyCategoriasInsumo = "CATALOG_CATEGORIAS_INSUMO";

        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromHours(4);

        public CatalogLookupService(IApplicationDbContext context, IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IReadOnlyList<UnidadMedidaDto>> GetUnidadesMedidaAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyUnidadesMedida, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<UnidadMedidaDto>)(await _context.UnidadesMedida
                    .AsNoTracking()
                    .Where(u => u.Activo)
                    .OrderBy(u => u.Id)
                    .Select(u => new UnidadMedidaDto(u.Id, u.Codigo, u.Nombre, u.Simbolo, u.EsFraccionable))
                    .ToListAsync(ct));
            }) ?? Array.Empty<UnidadMedidaDto>();
        }

        public async Task<IReadOnlyList<EstadoCitaMedicaDto>> GetEstadosCitaAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyEstadosCita, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<EstadoCitaMedicaDto>)(await _context.EstadosCitaMedica
                    .AsNoTracking()
                    .Where(e => e.Activo)
                    .OrderBy(e => e.Id)
                    .Select(e => new EstadoCitaMedicaDto(e.Id, e.Codigo, e.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<EstadoCitaMedicaDto>();
        }

        public async Task<IReadOnlyList<EstadoCajaDto>> GetEstadosCajaAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyEstadosCaja, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<EstadoCajaDto>)(await _context.EstadosCaja
                    .AsNoTracking()
                    .Where(e => e.Activo)
                    .OrderBy(e => e.Id)
                    .Select(e => new EstadoCajaDto(e.Id, e.Codigo, e.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<EstadoCajaDto>();
        }

        public async Task<IReadOnlyList<EstadoCuentaDto>> GetEstadosCuentaAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyEstadosCuenta, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<EstadoCuentaDto>)(await _context.EstadosCuenta
                    .AsNoTracking()
                    .Where(e => e.Activo)
                    .OrderBy(e => e.Id)
                    .Select(e => new EstadoCuentaDto(e.Id, e.Codigo, e.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<EstadoCuentaDto>();
        }

        public async Task<IReadOnlyList<TipoIngresoDto>> GetTiposIngresoAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyTiposIngreso, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<TipoIngresoDto>)(await _context.TiposIngreso
                    .AsNoTracking()
                    .Where(t => t.Activo)
                    .OrderBy(t => t.Id)
                    .Select(t => new TipoIngresoDto(t.Id, t.Codigo, t.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<TipoIngresoDto>();
        }

        public async Task<IReadOnlyList<EstadoFiscalDto>> GetEstadosFiscalesAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyEstadosFiscales, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<EstadoFiscalDto>)(await _context.EstadosFiscales
                    .AsNoTracking()
                    .Where(e => e.Activo)
                    .OrderBy(e => e.Id)
                    .Select(e => new EstadoFiscalDto(e.Id, e.Codigo, e.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<EstadoFiscalDto>();
        }

        public async Task<IReadOnlyList<MotivoAutorizacionDto>> GetMotivosAutorizacionAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyMotivosAutorizacion, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<MotivoAutorizacionDto>)(await _context.MotivosAutorizacion
                    .AsNoTracking()
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Id)
                    .Select(m => new MotivoAutorizacionDto(m.Id, m.Nombre))
                    .ToListAsync(ct));
            }) ?? Array.Empty<MotivoAutorizacionDto>();
        }

        public async Task<IReadOnlyList<CategoriaInsumoDto>> GetCategoriasInsumoAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrCreateAsync(KeyCategoriasInsumo, async entry =>
            {
                entry.SlidingExpiration = SlidingExpiration;
                entry.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;

                return (IReadOnlyList<CategoriaInsumoDto>)(await _context.CategoriasInsumo
                    .AsNoTracking()
                    .Where(c => c.Activo)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new CategoriaInsumoDto(c.Id, c.Nombre, c.Codigo))
                    .ToListAsync(ct));
            }) ?? Array.Empty<CategoriaInsumoDto>();
        }

        public async Task<bool> EsUnidadFraccionableAsync(int unidadMedidaId, CancellationToken ct = default)
        {
            var unidades = await GetUnidadesMedidaAsync(ct);
            return unidades.FirstOrDefault(u => u.Id == unidadMedidaId)?.EsFraccionable ?? true;
        }

        public void Invalidate(string catalogKey)
        {
            if (string.IsNullOrWhiteSpace(catalogKey)) return;
            _cache.Remove(catalogKey);
        }

        public void InvalidateAll()
        {
            _cache.Remove(KeyUnidadesMedida);
            _cache.Remove(KeyEstadosCita);
            _cache.Remove(KeyEstadosCaja);
            _cache.Remove(KeyEstadosCuenta);
            _cache.Remove(KeyTiposIngreso);
            _cache.Remove(KeyEstadosFiscales);
            _cache.Remove(KeyMotivosAutorizacion);
            _cache.Remove(KeyCategoriasInsumo);
        }
    }
}
