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
    public class ActualizarCortesiaDetalleCommand : IRequest<bool>
    {
        public Guid DetalleId { get; set; }
        public bool IncluidoEnTarifaBase { get; set; }
        public string UsuarioModificacion { get; set; } = string.Empty;

        public ActualizarCortesiaDetalleCommand(Guid detalleId, bool incluidoEnTarifaBase, string usuarioModificacion)
        {
            DetalleId = detalleId;
            IncluidoEnTarifaBase = incluidoEnTarifaBase;
            UsuarioModificacion = usuarioModificacion ?? throw new ArgumentNullException(nameof(usuarioModificacion));
        }
    }

    public class ActualizarCortesiaDetalleCommandHandler : IRequestHandler<ActualizarCortesiaDetalleCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ActualizarCortesiaDetalleCommandHandler> _logger;

        public ActualizarCortesiaDetalleCommandHandler(IApplicationDbContext context, ILogger<ActualizarCortesiaDetalleCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ActualizarCortesiaDetalleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Actualizando estado de cortesía para Detalle {DetalleId}. Incluido en tarifa base: {Incluido}", request.DetalleId, request.IncluidoEnTarifaBase);

            var detalle = await _context.DetallesServicioCuenta
                .FirstOrDefaultAsync(d => d.Id == request.DetalleId, cancellationToken);

            if (detalle == null)
            {
                throw new InvalidOperationException($"No se encontró el detalle de servicio con ID {request.DetalleId}");
            }

            if (request.IncluidoEnTarifaBase)
            {
                // Marcar como cortesía (establece precio a 0)
                detalle.MarcarComoIncluidoEnTarifaBase();
                _logger.LogInformation("Detalle {DetalleId} marcado como incluido en tarifa base (Cortesía). Precio establecido a 0.", request.DetalleId);
            }
            else
            {
                // Restaurar precio original consultando directamente al catálogo para evitar Price Tampering
                var baseService = await _context.ServiciosClinicos
                    .FirstOrDefaultAsync(s => s.Id == detalle.ServicioId, cancellationToken);

                decimal precioRestaurar = baseService?.PrecioBase ?? detalle.PrecioCatalogoHistorico;

                detalle.RemoverDeTarifaBase(precioRestaurar);
                _logger.LogInformation("Detalle {DetalleId} removido de tarifa base. Precio restaurado a: {Precio} (Precio Catálogo: {CatalogPrice})",
                    request.DetalleId, precioRestaurar, baseService?.PrecioBase);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
