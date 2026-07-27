using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    /// <summary>
    /// Comando para crear y agendar una nueva orden de cirugía.
    /// </summary>
    public class CrearOrdenCirugiaCommand : IRequest<Guid>
    {
        public Guid CuentaServicioId { get; set; }
        public Guid PacienteId { get; set; }
        public string DescripcionCirugia { get; set; } = string.Empty;
        public decimal PrecioBaseUsd { get; set; }
        public Guid MedicoId { get; set; }
        public DateTime FechaHoraProgramada { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
    }

    public class CrearOrdenCirugiaCommandHandler : IRequestHandler<CrearOrdenCirugiaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CrearOrdenCirugiaCommandHandler> _logger;

        public CrearOrdenCirugiaCommandHandler(IApplicationDbContext context, ILogger<CrearOrdenCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CrearOrdenCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creando orden de cirugía: {Descripcion} para Paciente {PacienteId}",
                request.DescripcionCirugia, request.PacienteId);

            var orden = new OrdenCirugia(
                request.CuentaServicioId,
                request.PacienteId,
                request.DescripcionCirugia,
                request.PrecioBaseUsd,
                request.MedicoId,
                request.FechaHoraProgramada,
                request.UsuarioCreacion);

            // Log de auditoría de creación
            orden.AgregarLog(request.UsuarioCreacion, CirugiaEventoConstants.Creacion,
                $"Cirugía '{request.DescripcionCirugia}' programada para {request.FechaHoraProgramada:yyyy-MM-dd HH:mm}. Precio: ${request.PrecioBaseUsd:N2} USD.");

            _context.OrdenesCirugia.Add(orden);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Orden de cirugía {OrdenId} creada exitosamente.", orden.Id);
            return orden.Id;
        }
    }
}
