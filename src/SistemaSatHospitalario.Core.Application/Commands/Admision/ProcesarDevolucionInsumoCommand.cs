using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

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

            // 2. Reingresar stock a la sede
            var stockSede = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == request.InsumoId && s.SedeId == request.SedeReingresoId, cancellationToken);

            if (stockSede != null)
            {
                stockSede.RegistrarEntrada(request.CantidadDevuelta);
            }

            // 3. Registrar log en la orden de cirugía si existe
            var orden = await _context.OrdenesCirugia
                .FirstOrDefaultAsync(o => o.CuentaServicioId == request.CuentaServicioId, cancellationToken);

            if (orden != null)
            {
                var log = new CirugiaLog(orden.Id, request.UsuarioId, CirugiaEventoConstants.DevolucionInsumos,
                    $"Devolución de {request.CantidadDevuelta} unidades del insumo {request.InsumoId} a la sede {request.SedeReingresoId}. Motivo: {request.Motivo ?? "Sobrante de cirugía"}");
                _context.CirugiaLogs.Add(log);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Devolución de {Cantidad} procesada para insumo {InsumoId} en cuenta {CuentaId}",
                request.CantidadDevuelta, request.InsumoId, request.CuentaServicioId);
            return true;
        }
    }
}
