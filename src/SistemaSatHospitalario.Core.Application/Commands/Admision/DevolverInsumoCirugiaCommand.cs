using System;
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
    public class DevolverInsumoCirugiaCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal CantidadRestar { get; set; }
        public string Usuario { get; set; } = string.Empty;

        public DevolverInsumoCirugiaCommand(Guid cuentaId, Guid insumoId, decimal cantidadRestar, string usuario)
        {
            CuentaId = cuentaId;
            InsumoId = insumoId;
            CantidadRestar = cantidadRestar;
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }
    }

    public class DevolverInsumoCirugiaCommandHandler : IRequestHandler<DevolverInsumoCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DevolverInsumoCirugiaCommandHandler> _logger;

        public DevolverInsumoCirugiaCommandHandler(IApplicationDbContext context, ILogger<DevolverInsumoCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DevolverInsumoCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Procesando devolución de Quirófano: Insumo {InsumoId}, Cantidad {CantidadRestar} para Cuenta {CuentaId}",
                request.InsumoId, request.CantidadRestar, request.CuentaId);

            // 1. Obtener la asignación del kit de cirugía
            var kitAsignacion = await _context.InsumosCirugiasPacientes
                .Include(k => k.Insumo)
                .FirstOrDefaultAsync(k => k.CuentaServicioId == request.CuentaId && k.InsumoId == request.InsumoId, cancellationToken);

            if (kitAsignacion == null)
            {
                throw new InvalidOperationException("Este insumo no forma parte del kit de cirugía asignado a esta cuenta.");
            }

            // 2. Registrar la devolución en la entidad (valida límites internamente)
            kitAsignacion.RegistrarDevolucion(request.CantidadRestar);

            // 3. Devolver stock físico al inventario de Quirófano (usando SedeId principal u Hospitalización)
            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            Guid targetSedeId = SeedConstants.SedeId_Principal;
            if (cuenta != null && cuenta.AreaClinicaId.HasValue)
            {
                var area = await _context.AreasClinicas.FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken);
                if (area != null)
                {
                    targetSedeId = area.SedeId;
                }
            }

            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == targetSedeId, cancellationToken);

            if (stockSede == null)
            {
                stockSede = new StockSede(request.InsumoId, targetSedeId, 0);
                _context.StocksSedes.Add(stockSede);
            }

            // Incrementar stock físico por retorno
            stockSede.RegistrarMovimientoStock(request.CantidadRestar, kitAsignacion.Insumo.PermiteFraccionamiento);

            // 4. Agregar MovimientoInsumo (Immutable Audit Ledger)
            var movimiento = new MovimientoInsumo(
                request.InsumoId,
                targetSedeId,
                "Devolución",
                request.CantidadRestar,
                kitAsignacion.Insumo.UnidadMedidaBase,
                request.CantidadRestar,
                request.Usuario,
                $"Retorno/Devolución de Quirófano a stock (Cuenta ID: {request.CuentaId})"
            );
            _context.MovimientosInsumo.Add(movimiento);

            // 5. Ajustar el cargo de facturación en caliente si el insumo fue cargado en la cuenta
            var detalleCargo = await _context.DetallesServicioCuenta
                .FirstOrDefaultAsync(d => d.CuentaServicioId == request.CuentaId && d.ServicioId == request.InsumoId, cancellationToken);

            if (detalleCargo != null)
            {
                decimal nuevaCantidad = Math.Max(0, detalleCargo.Cantidad - request.CantidadRestar);
                detalleCargo.ModificarCantidadAdministrativa(nuevaCantidad);
                _logger.LogInformation("Ajustado cargo de facturación en detalle {DetalleId}. Nueva cantidad: {NuevaCantidad}",
                    detalleCargo.Id, nuevaCantidad);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Devolución de Quirófano completada exitosamente.");
            return true;
        }
    }
}
