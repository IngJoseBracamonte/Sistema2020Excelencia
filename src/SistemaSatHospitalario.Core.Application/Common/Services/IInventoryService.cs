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
            string usuarioCarga,
            Guid cuentaId,
            Guid? sedeId,
            CancellationToken cancellationToken);

        Task RecordMovementAsync(
            Guid insumoId,
            Guid sedeId,
            string tipoMovimiento, // Ingreso, Descarte
            decimal cantidadOriginal,
            UnidadMedida unidadMedidaOriginal,
            string usuario,
            string motivo,
            CancellationToken cancellationToken);

        Task RecordDiscardAsync(
            Guid insumoId,
            decimal cantidad,
            string motivo,
            string usuario,
            CancellationToken cancellationToken = default);

        Task PerformClosingAsync(
            Guid sedeId,
            string usuario,
            string observaciones,
            List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken);

        Task DispatchPedidoAsync(
            Guid pedidoId,
            string usuario,
            Dictionary<Guid, decimal>? cantidadesAprobadas = null,
            Dictionary<Guid, string>? observacionesPorDetalle = null,
            CancellationToken cancellationToken = default);

        Task RejectPedidoAsync(
            Guid pedidoId,
            string usuario,
            string motivo,
            CancellationToken cancellationToken = default);

        Task ReceivePedidoAsync(
            Guid pedidoId,
            string usuario,
            Dictionary<Guid, decimal> discrepancias,
            CancellationToken cancellationToken = default);
    }

    public record CierreDetalleInputDto(Guid InsumoId, decimal StockReal);
}
