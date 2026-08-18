using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ProcesarDevolucionInsumoCommand : IRequest<bool>
    {
        public Guid CuentaServicioId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal CantidadDevuelta { get; set; }
        public Guid SedeReingresoId { get; set; } = SeedConstants.SedeId_Principal;
        public string UsuarioId { get; set; } = string.Empty;
        public string? Motivo { get; set; }
    }

    public class ProcesarDevolucionInsumoCommandHandler : IRequestHandler<ProcesarDevolucionInsumoCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProcesarDevolucionInsumoCommandHandler> _logger;

        public ProcesarDevolucionInsumoCommandHandler(
            IApplicationDbContext context,
            ILogger<ProcesarDevolucionInsumoCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcesarDevolucionInsumoCommand request, CancellationToken cancellationToken)
        {
            if (request.CantidadDevuelta <= 0)
            {
                throw new ArgumentException("La cantidad devuelta debe ser mayor a cero.", nameof(request.CantidadDevuelta));
            }

            var asignado = await _context.InsumosCirugiasPacientes
                .FirstOrDefaultAsync(i => i.CuentaServicioId == request.CuentaServicioId && i.InsumoId == request.InsumoId, cancellationToken);

            if (asignado == null)
            {
                _logger.LogWarning("Insumo {InsumoId} no encontrado en la cuenta {CuentaId}", request.InsumoId, request.CuentaServicioId);
                return false;
            }

            // 1. Registrar devolución en el insumo del paciente
            asignado.RegistrarDevolucion(request.CantidadDevuelta);

            // 2. Reingresar stock a la sede destino (Principal o la indicada)
            var targetSedeId = request.SedeReingresoId != Guid.Empty ? request.SedeReingresoId : SeedConstants.SedeId_Principal;
            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == targetSedeId, cancellationToken);

            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == request.InsumoId, cancellationToken);

            if (stockSede == null)
            {
                stockSede = new StockSede(request.InsumoId, targetSedeId, 0);
                _context.StocksSedes.Add(stockSede);
            }

            stockSede.RegistrarMovimientoStock(request.CantidadDevuelta, insumo?.PermiteFraccionamiento ?? true);

            // 3. Registrar Movimiento Inmutable en Kárdex
            var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
            var movDevolucion = new MovimientoInsumo(
                request.InsumoId,
                targetSedeId,
                TipoMovimientoInsumo.Ingreso,
                request.CantidadDevuelta,
                insumo?.UnidadMedidaBase ?? UnidadMedida.UNIDAD,
                request.CantidadDevuelta,
                usuario,
                $"Devolución de sobrante de cirugía (Cuenta: {request.CuentaServicioId})"
            );
            _context.MovimientosInsumo.Add(movDevolucion);

            // 4. Ajustar el cargo en la cuenta si existiera
            var detalleCargo = await _context.DetallesServicioCuenta
                .FirstOrDefaultAsync(d => d.CuentaServicioId == request.CuentaServicioId && d.ServicioId == request.InsumoId, cancellationToken);

            if (detalleCargo != null)
            {
                decimal nuevaCantidad = Math.Max(0, detalleCargo.Cantidad - request.CantidadDevuelta);
                detalleCargo.ModificarCantidadAdministrativa(nuevaCantidad);
            }

            // 5. Registrar log en la orden de cirugía si existe
            var orden = await _context.OrdenesCirugia
                .FirstOrDefaultAsync(o => o.CuentaServicioId == request.CuentaServicioId, cancellationToken);

            if (orden != null)
            {
                var log = new CirugiaLog(orden.Id, usuario, CirugiaEventoConstants.DevolucionInsumos,
                    $"Devolución de {request.CantidadDevuelta} unidades del insumo {insumo?.Nombre ?? request.InsumoId.ToString()} a la sede {targetSedeId}. Motivo: {request.Motivo ?? "Sobrante de cirugía"}");
                _context.CirugiaLogs.Add(log);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Devolución de {Cantidad} procesada para insumo {InsumoId} en cuenta {CuentaId}",
                request.CantidadDevuelta, request.InsumoId, request.CuentaServicioId);
            return true;
        }
    }
}
