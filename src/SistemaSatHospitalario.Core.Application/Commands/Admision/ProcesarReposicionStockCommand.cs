using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ProcesarReposicionStockCommand : IRequest<bool>
    {
        public Guid InsumoId { get; set; }
        public Guid SedeOrigenId { get; set; }
        public Guid SedeDestinoId { get; set; }
        public decimal Cantidad { get; set; }
        public string Motivo { get; set; } = "Reposicion";
        public string UsuarioId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class ProcesarReposicionStockCommandHandler : IRequestHandler<ProcesarReposicionStockCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProcesarReposicionStockCommandHandler> _logger;

        public ProcesarReposicionStockCommandHandler(
            IApplicationDbContext context,
            ILogger<ProcesarReposicionStockCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcesarReposicionStockCommand request, CancellationToken cancellationToken)
        {
            if (request.InsumoId == Guid.Empty)
                throw new ArgumentException("El insumo es obligatorio.", nameof(request.InsumoId));
            if (request.SedeOrigenId == Guid.Empty)
                throw new ArgumentException("La sede origen es obligatoria.", nameof(request.SedeOrigenId));
            if (request.SedeDestinoId == Guid.Empty)
                throw new ArgumentException("La sede destino es obligatoria.", nameof(request.SedeDestinoId));
            if (request.SedeOrigenId == request.SedeDestinoId)
                throw new ArgumentException("La sede origen y destino no pueden ser iguales.", nameof(request.SedeDestinoId));
            if (request.Cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(request.Cantidad));

            // 1. Obtener o crear StockSede origen
            var stockOrigen = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == request.SedeOrigenId, cancellationToken);

            if (stockOrigen == null)
            {
                _logger.LogWarning("No existe registro de stock en la sede origen {SedeOrigenId} para el insumo {InsumoId}", request.SedeOrigenId, request.InsumoId);
                return false;
            }

            if (stockOrigen.StockActual < request.Cantidad)
            {
                _logger.LogWarning("Stock insuficiente en la sede origen ({StockActual}) para transferir {Cantidad}", stockOrigen.StockActual, request.Cantidad);
                throw new InvalidOperationException($"Stock insuficiente en sede origen (Disponible: {stockOrigen.StockActual:N2}, Solicitado: {request.Cantidad:N2}).");
            }

            // 2. Obtener o crear StockSede destino
            var stockDestino = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == request.SedeDestinoId, cancellationToken);

            if (stockDestino == null)
            {
                stockDestino = new StockSede(request.InsumoId, request.SedeDestinoId, 0);
                _context.StocksSedes.Add(stockDestino);
            }

            // 3. Ejecutar transferencia atómica de stock
            stockOrigen.RegistrarSalida(request.Cantidad);
            stockDestino.RegistrarEntrada(request.Cantidad);

            // 4. Registrar auditoría de la transferencia/reposición
            var registro = new TransferenciaReposicionStock(
                request.InsumoId,
                request.SedeOrigenId,
                request.SedeDestinoId,
                request.Cantidad,
                request.Motivo,
                request.UsuarioId,
                request.Observaciones);

            _context.TransferenciasReposicionStock.Add(registro);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reposición de {Cantidad} del insumo {InsumoId} completada desde {Origen} a {Destino}",
                request.Cantidad, request.InsumoId, request.SedeOrigenId, request.SedeDestinoId);
            return true;
        }
    }
}
