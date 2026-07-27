using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    /// <summary>
    /// Comando para procesar devolución masiva de insumos de cirugía a Sede Principal.
    /// </summary>
    public class ProcesarDevolucionCirugiaMasivaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid CuentaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public List<DevolucionInsumoItem> Items { get; set; } = new();
    }

    public class DevolucionInsumoItem
    {
        public Guid InsumoId { get; set; }
        public decimal CantidadUsada { get; set; }
    }

    public class ProcesarDevolucionCirugiaMasivaCommandHandler : IRequestHandler<ProcesarDevolucionCirugiaMasivaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ProcesarDevolucionCirugiaMasivaCommandHandler> _logger;

        public ProcesarDevolucionCirugiaMasivaCommandHandler(IApplicationDbContext context, ILogger<ProcesarDevolucionCirugiaMasivaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcesarDevolucionCirugiaMasivaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Procesando devolución masiva para Orden {OrdenId}, Cuenta {CuentaId}",
                request.OrdenCirugiaId, request.CuentaId);

            var orden = await _context.OrdenesCirugia
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken)
                ?? throw new InvalidOperationException("La orden de cirugía no fue encontrada.");

            var detalleDevolucion = new List<string>();

            foreach (var item in request.Items)
            {
                var kitAsignacion = await _context.InsumosCirugiasPacientes
                    .Include(k => k.Insumo)
                    .FirstOrDefaultAsync(k => k.CuentaServicioId == request.CuentaId && k.InsumoId == item.InsumoId, cancellationToken);

                if (kitAsignacion == null) continue;

                decimal cantidadADevolver = kitAsignacion.CantidadEntregada - item.CantidadUsada - kitAsignacion.CantidadDevuelta;
                if (cantidadADevolver <= 0) continue;

                // Registrar devolución en el kit
                kitAsignacion.RegistrarDevolucion(cantidadADevolver);

                // Devolver stock a Sede Principal
                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == item.InsumoId && s.SedeId == SeedConstants.SedeId_Principal, cancellationToken);

                if (stockSede == null)
                {
                    stockSede = new StockSede(item.InsumoId, SeedConstants.SedeId_Principal, 0);
                    _context.StocksSedes.Add(stockSede);
                }

                stockSede.RegistrarMovimientoStock(cantidadADevolver, kitAsignacion.Insumo.PermiteFraccionamiento);

                // Movimiento de inventario inmutable
                var movimiento = new MovimientoInsumo(
                    item.InsumoId,
                    SeedConstants.SedeId_Principal,
                    "Devolución",
                    cantidadADevolver,
                    kitAsignacion.Insumo.UnidadMedidaBase,
                    cantidadADevolver,
                    request.UsuarioId,
                    $"Devolución masiva de Quirófano a Sede Principal (Orden: {request.OrdenCirugiaId})");
                _context.MovimientosInsumo.Add(movimiento);

                // Ajustar cargo de facturación
                var detalleCargo = await _context.DetallesServicioCuenta
                    .FirstOrDefaultAsync(d => d.CuentaServicioId == request.CuentaId && d.ServicioId == item.InsumoId, cancellationToken);

                if (detalleCargo != null)
                {
                    decimal nuevaCantidad = Math.Max(0, detalleCargo.Cantidad - cantidadADevolver);
                    detalleCargo.ModificarCantidadAdministrativa(nuevaCantidad);
                }

                detalleDevolucion.Add($"{kitAsignacion.Insumo.Nombre}: {cantidadADevolver} devuelto(s)");
            }

            // Log de auditoría en la orden de cirugía
            if (detalleDevolucion.Count > 0)
            {
                orden.AgregarLog(request.UsuarioId, CirugiaEventoConstants.DevolucionInsumos,
                    $"Devolución masiva a Sede Principal: {string.Join("; ", detalleDevolucion)}");
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Devolución masiva completada: {Count} ítems procesados.", detalleDevolucion.Count);
            return true;
        }
    }
}
