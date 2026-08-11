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
    /// <summary>
    /// Comando para cambiar el estado de una orden de cirugía (Iniciar, Completar, Cancelar).
    /// </summary>
    public class CambiarEstadoCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
        public string? MotivoCancelacion { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class CambiarEstadoCirugiaCommandHandler : IRequestHandler<CambiarEstadoCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CambiarEstadoCirugiaCommandHandler> _logger;

        public CambiarEstadoCirugiaCommandHandler(IApplicationDbContext context, ILogger<CambiarEstadoCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CambiarEstadoCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cambiando estado de Orden {OrdenId} a {NuevoEstado}",
                request.OrdenCirugiaId, request.NuevoEstado);

            var orden = await _context.OrdenesCirugia
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken)
                ?? throw new InvalidOperationException("La orden de cirugía no fue encontrada.");

            var targetState = request.NuevoEstado?.Trim() ?? string.Empty;

            if (targetState.Equals(EstadoCirugiaConstants.EnEspera, StringComparison.OrdinalIgnoreCase))
            {
                orden.IniciarEspera(request.UsuarioId);
            }
            else if (targetState.Equals(EstadoCirugiaConstants.EnCirugia, StringComparison.OrdinalIgnoreCase) ||
                     targetState.Equals("EnProceso", StringComparison.OrdinalIgnoreCase))
            {
                orden.IniciarCirugia(request.UsuarioId);
            }
            else if (targetState.Equals(EstadoCirugiaConstants.Finalizado, StringComparison.OrdinalIgnoreCase) ||
                     targetState.Equals("Completada", StringComparison.OrdinalIgnoreCase))
            {
                orden.FinalizarCirugia(request.UsuarioId);
            }
            else if (targetState.Equals(EstadoCirugiaConstants.Cancelada, StringComparison.OrdinalIgnoreCase))
            {
                orden.CancelarCirugia(request.UsuarioId, request.MotivoCancelacion ?? "Sin motivo especificado");
            }
            else
            {
                throw new ArgumentException($"Estado de cirugía no reconocido o desatendido: {request.NuevoEstado}");
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Estado de Orden {OrdenId} cambiado exitosamente a {Estado}.", request.OrdenCirugiaId, orden.Estado);
            return true;
        }
    }
}
