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

            switch (request.NuevoEstado)
            {
                case EstadoCirugiaConstants.EnProceso:
                    orden.IniciarCirugia(request.UsuarioId);
                    break;
                case EstadoCirugiaConstants.Completada:
                    orden.CompletarCirugia(request.UsuarioId);
                    break;
                case EstadoCirugiaConstants.Cancelada:
                    orden.CancelarCirugia(request.UsuarioId, request.MotivoCancelacion ?? "Sin motivo especificado");
                    break;
                default:
                    throw new ArgumentException($"Estado de cirugía no reconocido: {request.NuevoEstado}");
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Estado de Orden {OrdenId} cambiado a {Estado}.", request.OrdenCirugiaId, orden.Estado);
            return true;
        }
    }
}
