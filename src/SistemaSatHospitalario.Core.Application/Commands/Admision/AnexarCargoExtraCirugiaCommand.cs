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
    /// <summary>
    /// Comando para anexar un cargo extra (medicamento, insumo, procedimiento) a la cuenta
    /// durante una cirugía en proceso, descontando el inventario físico y registrando movimientos.
    /// </summary>
    public class AnexarCargoExtraCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid CuentaServicioId { get; set; }
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class AnexarCargoExtraCirugiaCommandHandler : IRequestHandler<AnexarCargoExtraCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AnexarCargoExtraCirugiaCommandHandler> _logger;

        public AnexarCargoExtraCirugiaCommandHandler(IApplicationDbContext context, ILogger<AnexarCargoExtraCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AnexarCargoExtraCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Anexando cargo extra a Orden {OrdenId}: {Descripcion} (x{Cantidad})",
                request.OrdenCirugiaId, request.Descripcion, request.Cantidad);

            var orden = await _context.OrdenesCirugia
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken)
                ?? throw new InvalidOperationException("La orden de cirugía no fue encontrada.");

            var cuenta = await _context.CuentasServicios
                .FirstOrDefaultAsync(c => c.Id == request.CuentaServicioId, cancellationToken)
                ?? throw new InvalidOperationException("La cuenta de servicios no fue encontrada.");

            // 1. Agregar el servicio a la cuenta del paciente (Cobro Financiero)
            cuenta.AgregarServicio(
                request.ServicioId,
                request.Descripcion,
                request.Precio,
                request.Honorario,
                request.Cantidad,
                request.TipoServicio,
                request.UsuarioId);

            // 2. Si el cargo corresponde a un insumo/medicamento físico, realizar descuento de stock y trazabilidad
            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(i => i.Id == request.ServicioId, cancellationToken);

            if (insumo != null || request.TipoServicio.Equals("Insumo", StringComparison.OrdinalIgnoreCase) || request.TipoServicio.Equals("Medicamento", StringComparison.OrdinalIgnoreCase))
            {
                var insumoId = insumo?.Id ?? request.ServicioId;

                if (insumo != null)
                {
                    // Descuento de stock en Sede Principal
                    var stockSede = await _context.StocksSedes
                        .FirstOrDefaultAsync(s => s.InsumoId == insumoId && s.SedeId == SeedConstants.SedeId_Principal, cancellationToken);

                    if (stockSede == null)
                    {
                        stockSede = new StockSede(insumoId, SeedConstants.SedeId_Principal, 0);
                        _context.StocksSedes.Add(stockSede);
                    }

                    stockSede.RegistrarMovimientoStock(-request.Cantidad, insumo.PermiteFraccionamiento);

                    // Movimiento de inventario inmutable
                    var movimiento = new MovimientoInsumo(
                        insumoId,
                        SeedConstants.SedeId_Principal,
                        "Salida",
                        request.Cantidad,
                        insumo.UnidadMedidaBase,
                        -request.Cantidad,
                        request.UsuarioId,
                        $"Consumo extra en Quirófano (Orden: {request.OrdenCirugiaId})");
                    _context.MovimientosInsumo.Add(movimiento);

                    // Registro / Actualización en InsumoCirugiaPaciente para habilitar devolución posterior si aplica
                    var asignacion = await _context.InsumosCirugiasPacientes
                        .FirstOrDefaultAsync(k => k.CuentaServicioId == request.CuentaServicioId && k.InsumoId == insumoId, cancellationToken);

                    if (asignacion != null)
                    {
                        asignacion.AdicionarEntregada(request.Cantidad);
                    }
                    else
                    {
                        var nuevaAsignacion = new InsumoCirugiaPaciente(request.CuentaServicioId, insumoId, request.Cantidad);
                        _context.InsumosCirugiasPacientes.Add(nuevaAsignacion);
                    }
                }
            }

            // 3. Log de auditoría en la orden de cirugía
            orden.AgregarLog(request.UsuarioId, CirugiaEventoConstants.CargoExtra,
                $"Cargo extra imputado e inventariado: {request.Descripcion} x{request.Cantidad} - ${request.Precio:N2} USD");

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cargo extra imputado y stock descontado exitosamente para Orden {OrdenId}.", request.OrdenCirugiaId);
            return true;
        }
    }
}
