using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    /// <summary>
    /// Servicio central de catálogos cacheados (3FN).
    /// Patrón Cache-Aside con invalidación reactiva por clave.
    /// </summary>
    public interface ICatalogLookupService
    {
        /// <summary>Obtiene las unidades de medida activas (cacheado).</summary>
        Task<IReadOnlyList<UnidadMedidaDto>> GetUnidadesMedidaAsync(CancellationToken ct = default);

        /// <summary>Obtiene los estados de cita médica activos (cacheado).</summary>
        Task<IReadOnlyList<EstadoCitaMedicaDto>> GetEstadosCitaAsync(CancellationToken ct = default);

        /// <summary>Obtiene los estados de caja activos (cacheado).</summary>
        Task<IReadOnlyList<EstadoCajaDto>> GetEstadosCajaAsync(CancellationToken ct = default);

        /// <summary>Obtiene los estados de cuenta activos (cacheado).</summary>
        Task<IReadOnlyList<EstadoCuentaDto>> GetEstadosCuentaAsync(CancellationToken ct = default);

        /// <summary>Obtiene los tipos de ingreso activos (cacheado).</summary>
        Task<IReadOnlyList<TipoIngresoDto>> GetTiposIngresoAsync(CancellationToken ct = default);

        /// <summary>Obtiene los estados fiscales activos (cacheado).</summary>
        Task<IReadOnlyList<EstadoFiscalDto>> GetEstadosFiscalesAsync(CancellationToken ct = default);

        /// <summary>Obtiene los motivos de autorización activos (cacheado).</summary>
        Task<IReadOnlyList<MotivoAutorizacionDto>> GetMotivosAutorizacionAsync(CancellationToken ct = default);

        /// <summary>Obtiene las categorías de insumo activas (cacheado).</summary>
        Task<IReadOnlyList<CategoriaInsumoDto>> GetCategoriasInsumoAsync(CancellationToken ct = default);

        /// <summary>Verifica si una unidad de medida es fraccionable (cacheado).</summary>
        Task<bool> EsUnidadFraccionableAsync(int unidadMedidaId, CancellationToken ct = default);

        /// <summary>Invalida un catálogo específico por clave.</summary>
        void Invalidate(string catalogKey);

        /// <summary>Invalida todos los catálogos.</summary>
        void InvalidateAll();
    }

    // DTOs de catálogo (proyección plana para caché)
    public record UnidadMedidaDto(int Id, string Codigo, string Nombre, string Simbolo, bool EsFraccionable);
    public record EstadoCitaMedicaDto(int Id, string Codigo, string Nombre);
    public record EstadoCajaDto(int Id, string Codigo, string Nombre);
    public record EstadoCuentaDto(int Id, string Codigo, string Nombre);
    public record TipoIngresoDto(int Id, string Codigo, string Nombre);
    public record EstadoFiscalDto(int Id, string Codigo, string Nombre);
    public record MotivoAutorizacionDto(int Id, string Nombre);
    public record CategoriaInsumoDto(Guid Id, string Nombre, string? Codigo);
}
