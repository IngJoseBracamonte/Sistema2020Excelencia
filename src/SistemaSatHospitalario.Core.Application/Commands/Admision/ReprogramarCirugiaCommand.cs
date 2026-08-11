using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ReprogramarCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public DateTime NuevaFechaHora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class ReprogramarCirugiaCommandHandler : IRequestHandler<ReprogramarCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ReprogramarCirugiaCommandHandler> _logger;

        public ReprogramarCirugiaCommandHandler(IApplicationDbContext context, ILogger<ReprogramarCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ReprogramarCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando reprogramación de la cirugía {OrdenId} a {NuevaFecha}. Motivo: {Motivo}",
                request.OrdenCirugiaId, request.NuevaFechaHora, request.Motivo);

            var orden = await _context.OrdenesCirugia
                .Include(o => o.HistorialObservaciones)
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null)
            {
                throw new InvalidOperationException($"No se encontró la orden de cirugía con ID: {request.OrdenCirugiaId}");
            }

            // Invocación del Patrón State en el Dominio (Exige motivo y evalúa estado actual)
            orden.Reprogramar(request.NuevaFechaHora, request.Motivo, request.UsuarioId);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cirugía {OrdenId} reprogramada exitosamente.", orden.Id);
            return true;
        }
    }
}
