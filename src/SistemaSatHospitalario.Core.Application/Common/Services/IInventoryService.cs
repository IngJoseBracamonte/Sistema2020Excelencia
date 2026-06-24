using System;
using System.Threading;
using System.Threading.Tasks;

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
            SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida unidadMedidaOriginal,
            string usuario,
            string motivo,
            CancellationToken cancellationToken);

        Task PerformClosingAsync(
            Guid sedeId,
            string usuario,
            string observaciones,
            System.Collections.Generic.List<CierreDetalleInputDto> detalles,
            CancellationToken cancellationToken);

        Task DispatchPedidoAsync(
            Guid pedidoId,
            string usuario,
            CancellationToken cancellationToken);

        Task ReceivePedidoAsync(
            Guid pedidoId,
            string usuario,
            System.Collections.Generic.Dictionary<Guid, decimal> discrepancias,
            CancellationToken cancellationToken);
    }

    public record CierreDetalleInputDto(Guid InsumoId, decimal StockReal);
}
