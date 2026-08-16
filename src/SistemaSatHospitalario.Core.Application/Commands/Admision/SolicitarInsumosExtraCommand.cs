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
    public class SolicitarInsumosExtraCommand : IRequest<Guid>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public Guid AlmacenOrigenId { get; set; } = SeedConstants.SedeId_Principal;
        public string UsuarioId { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class SolicitarInsumosExtraCommandHandler : IRequestHandler<SolicitarInsumosExtraCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<SolicitarInsumosExtraCommandHandler> _logger;

        public SolicitarInsumosExtraCommandHandler(
            IApplicationDbContext context,
            ILogger<SolicitarInsumosExtraCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(SolicitarInsumosExtraCommand request, CancellationToken cancellationToken)
        {
            var orden = await _context.OrdenesCirugia
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null)
            {
                throw new InvalidOperationException($"Orden de cirugía {request.OrdenCirugiaId} no encontrada.");
            }

            var solicitud = new SolicitudInsumoCirugia(
                orden.Id,
                request.InsumoId,
                request.Cantidad,
                request.AlmacenOrigenId,
                request.UsuarioId,
                request.Observaciones);

            _context.SolicitudesInsumosCirugia.Add(solicitud);

            var log = new CirugiaLog(orden.Id, request.UsuarioId, CirugiaEventoConstants.SolicitudInsumoExtra,
                $"Solicitud ad-hoc: Insumo {request.InsumoId}, Cantidad {request.Cantidad}");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Solicitud ad-hoc {SolicitudId} creada para la orden {OrdenId}", solicitud.Id, orden.Id);
            return solicitud.Id;
        }
    }

    public class DespacharSolicitudExtraCommand : IRequest<bool>
    {
        public Guid SolicitudId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class DespacharSolicitudExtraCommandHandler : IRequestHandler<DespacharSolicitudExtraCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DespacharSolicitudExtraCommandHandler> _logger;

        public DespacharSolicitudExtraCommandHandler(
            IApplicationDbContext context,
            ILogger<DespacharSolicitudExtraCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DespacharSolicitudExtraCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.SolicitudesInsumosCirugia
                .Include(s => s.OrdenCirugia)
                .FirstOrDefaultAsync(s => s.Id == request.SolicitudId, cancellationToken);

            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud extra {SolicitudId} no encontrada.", request.SolicitudId);
                return false;
            }

            solicitud.Despachar(request.UsuarioId);

            // Descontar stock del almacén de origen
            var stock = await _context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == solicitud.InsumoId && s.SedeId == solicitud.AlmacenOrigenId, cancellationToken);

            if (stock != null)
            {
                stock.RegistrarSalida(solicitud.CantidadSolicitada);
            }

            // Cargar directamente a la cuenta del paciente
            var cuentaId = solicitud.OrdenCirugia.CuentaServicioId;
            var existente = await _context.InsumosCirugiasPacientes
                .FirstOrDefaultAsync(i => i.CuentaServicioId == cuentaId && i.InsumoId == solicitud.InsumoId, cancellationToken);

            if (existente != null)
            {
                existente.AdicionarEntregada(solicitud.CantidadSolicitada);
            }
            else
            {
                var nuevoConsumo = new InsumoCirugiaPaciente(cuentaId, solicitud.InsumoId, solicitud.CantidadSolicitada, solicitud.OrdenCirugiaId);
                _context.InsumosCirugiasPacientes.Add(nuevoConsumo);
            }

            var log = new CirugiaLog(solicitud.OrdenCirugiaId, request.UsuarioId, CirugiaEventoConstants.DespachoInsumos,
                $"Despachada solicitud ad-hoc #{solicitud.Id}: {solicitud.CantidadSolicitada} unidades del insumo {solicitud.InsumoId}");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Solicitud extra {SolicitudId} despachada e imputada a la cuenta {CuentaId}", solicitud.Id, cuentaId);
            return true;
        }
    }
}
