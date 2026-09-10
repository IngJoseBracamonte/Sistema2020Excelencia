using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    public interface IInventoryService
    {
        Task DeductInventoryForServiceDetailAsync(
            Guid detalleId,
            Guid serviceId,
            string serviceCodigo,
            string serviceDescripcion,
            decimal cantidadServicio,
            Guid? usuarioCargaId,   // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Guid cuentaId,
            Guid? sedeId,
            CancellationToken cancellationToken);

        Task RecordMovementAsync(
            Guid insumoId,
            Guid sedeId,
            string tipoMovimiento, // Ingreso, Descarte
            decimal cantidadOriginal,
            UnidadMedidaEnum unidadMedidaOriginal,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string motivo,
            CancellationToken cancellationToken);

        Task RecordDiscardAsync(
            Guid insumoId,
            decimal cantidad,
            string motivo,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Guid? sedeId = null,
            CancellationToken cancellationToken = default);

        Task PerformClosingAsync(
            Guid sedeId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string observaciones,
            List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken);

        Task DispatchPedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Dictionary<Guid, decimal>? cantidadesAprobadas = null,
            Dictionary<Guid, string>? observacionesPorDetalle = null,
            CancellationToken cancellationToken = default);

        Task RejectPedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            string motivo,
            CancellationToken cancellationToken = default);

        Task ReceivePedidoAsync(
            Guid pedidoId,
            Guid? usuarioId,        // V14.3 - Usar Guid (ID) en lugar de string (nombre) por 3FN normalization
            Dictionary<Guid, decimal> discrepancias,
            CancellationToken cancellationToken = default);
    }

    public record CierreDetalleInputDto(Guid InsumoId, decimal StockReal);
}
