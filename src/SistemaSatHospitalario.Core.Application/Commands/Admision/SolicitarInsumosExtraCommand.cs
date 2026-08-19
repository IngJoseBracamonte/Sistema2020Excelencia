using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

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
                .Include(o => o.Paciente)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null)
            {
                throw new InvalidOperationException($"Orden de cirugía {request.OrdenCirugiaId} no encontrada.");
            }

            var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;

            // 1. Registrar Solicitud Ad-Hoc en Pabellón
            var solicitud = new SolicitudInsumoCirugia(
                orden.Id,
                request.InsumoId,
                request.Cantidad,
                request.AlmacenOrigenId != Guid.Empty ? request.AlmacenOrigenId : SeedConstants.SedeId_Principal,
                usuario,
                request.Observaciones);

            _context.SolicitudesInsumosCirugia.Add(solicitud);

            // 2. Generar Correlativo Atómico e Integrar Requisición en PedidosInterSede para el Supervisor Central
            var year = DateTime.UtcNow.Year.ToString();
            var latestCorrelativo = await _context.PedidosInterSede
                .Where(p => p.Correlativo.StartsWith($"PED-{year}-"))
                .OrderByDescending(p => p.Correlativo.Length)
                .ThenByDescending(p => p.Correlativo)
                .Select(p => p.Correlativo)
                .FirstOrDefaultAsync(cancellationToken);

            int nextSeq = 1;
            if (!string.IsNullOrEmpty(latestCorrelativo) && latestCorrelativo.Length >= 13)
            {
                var numPart = latestCorrelativo.Substring(latestCorrelativo.LastIndexOf('-') + 1);
                if (int.TryParse(numPart, out int lastSeq))
                {
                    nextSeq = lastSeq + 1;
                }
            }

            var correlativo = $"PED-{year}-{nextSeq.ToString().PadLeft(4, '0')}";
            var pacienteNombre = orden.Paciente != null ? orden.Paciente.NombreCorto : "Paciente Quirúrgico";
            var obsPedido = $"[CIRUGIA_ADHOC:{solicitud.Id}:{orden.Id}] Paciente: {pacienteNombre} | Cirugía: {orden.DescripcionCirugia} | Urgencia: {request.Observaciones?.Trim()}";

            var pedido = new PedidoInterSede(
                correlativo,
                SeedConstants.SedeId_Cirugia,
                request.AlmacenOrigenId != Guid.Empty ? request.AlmacenOrigenId : SeedConstants.SedeId_Principal,
                usuario,
                obsPedido
            );
            pedido.AgregarDetalle(new PedidoInterSedeDetalle(request.InsumoId, request.Cantidad));
            _context.PedidosInterSede.Add(pedido);

            // 3. Auditoría Inmutable
            var log = new CirugiaLog(orden.Id, usuario, CirugiaEventoConstants.SolicitudInsumoExtra,
                $"Solicitud ad-hoc ({correlativo}): Insumo {request.InsumoId}, Cantidad {request.Cantidad}");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Solicitud ad-hoc {SolicitudId} y Pedido {Correlativo} creados para la orden {OrdenId}", solicitud.Id, correlativo, orden.Id);
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

            var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
            solicitud.Despachar(usuario);

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

            // Sincronizar PedidoInterSede asociado para que no quede pendiente
            var tag = $"[CIRUGIA_ADHOC:{solicitud.Id}:";
            var pedidoAsociado = await _context.PedidosInterSede
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Observaciones != null && p.Observaciones.Contains(tag), cancellationToken);

            if (pedidoAsociado != null && pedidoAsociado.Estado == EstadoPedidoInterSede.Solicitado)
            {
                foreach (var det in pedidoAsociado.Detalles)
                {
                    det.SetDespachado(det.CantidadSolicitada);
                    det.SetRecibido(det.CantidadSolicitada);
                }
                pedidoAsociado.CambiarEstado(EstadoPedidoInterSede.Recibido);
            }

            var log = new CirugiaLog(solicitud.OrdenCirugiaId, usuario, CirugiaEventoConstants.DespachoInsumos,
                $"Despachada solicitud ad-hoc #{solicitud.Id}: {solicitud.CantidadSolicitada} unidades del insumo {solicitud.InsumoId}");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Solicitud extra {SolicitudId} despachada e imputada a la cuenta {CuentaId}", solicitud.Id, cuentaId);
            return true;
        }
    }
}
