using System;
using System.Collections.Generic;
using System.Linq;
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
    public class KitInsumoPersonalizadoDto
    {
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class AsignarKitCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid CuentaServicioId { get; set; }
        public Guid SedeDespachoId { get; set; } = SeedConstants.SedeId_Principal;
        public string UsuarioId { get; set; } = string.Empty;
        public List<KitInsumoPersonalizadoDto> InsumosPersonalizados { get; set; } = new();
    }

    public class AsignarKitCirugiaCommandHandler : IRequestHandler<AsignarKitCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AsignarKitCirugiaCommandHandler> _logger;

        public AsignarKitCirugiaCommandHandler(
            IApplicationDbContext context,
            ILogger<AsignarKitCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(AsignarKitCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Asignando kit de insumos a la orden {OrdenId} para la cuenta {CuentaId}",
                request.OrdenCirugiaId, request.CuentaServicioId);

            var orden = await _context.OrdenesCirugia.FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);
            if (orden == null)
            {
                _logger.LogWarning("Orden de cirugía {OrdenId} no encontrada.", request.OrdenCirugiaId);
                return false;
            }

            foreach (var itemDto in request.InsumosPersonalizados.Where(i => i.Cantidad > 0))
            {
                var existente = await _context.InsumosCirugiasPacientes
                    .FirstOrDefaultAsync(i => i.CuentaServicioId == request.CuentaServicioId && i.InsumoId == itemDto.InsumoId, cancellationToken);

                if (existente != null)
                {
                    existente.AdicionarEntregada(itemDto.Cantidad);
                }
                else
                {
                    var nuevoConsumo = new InsumoCirugiaPaciente(request.CuentaServicioId, itemDto.InsumoId, itemDto.Cantidad, request.OrdenCirugiaId);
                    _context.InsumosCirugiasPacientes.Add(nuevoConsumo);
                }

                var stockSede = await _context.StocksSedes
                    .FirstOrDefaultAsync(s => s.InsumoId == itemDto.InsumoId && s.SedeId == request.SedeDespachoId, cancellationToken);

                if (stockSede != null)
                {
                    stockSede.RegistrarSalida(itemDto.Cantidad);
                }
            }

            var log = new CirugiaLog(orden.Id, request.UsuarioId, CirugiaEventoConstants.DespachoInsumos,
                $"Despachados {request.InsumosPersonalizados.Count} insumos del kit desde Sede {request.SedeDespachoId}.");
            _context.CirugiaLogs.Add(log);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Kit de insumos asignado y despachado exitosamente para la orden {OrdenId}.", request.OrdenCirugiaId);
            return true;
        }
    }
}
