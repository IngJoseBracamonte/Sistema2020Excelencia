using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RemoveServicioDeCuentaCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public Guid DetalleId { get; set; }
        public Guid? MedicoId { get; set; } // V4.6 Precision Metadata
        public DateTime? HoraCita { get; set; } // V4.6 Precision Metadata
    }

    public class RemoveServicioDeCuentaCommandHandler : IRequestHandler<RemoveServicioDeCuentaCommand, bool>
    {
        private readonly IBillingRepository _repository;
        private readonly IApplicationDbContext? _context;

        public RemoveServicioDeCuentaCommandHandler(IBillingRepository repository, IApplicationDbContext? context = null)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<bool> Handle(RemoveServicioDeCuentaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _repository.ObtenerCuentaPorIdAsync(request.CuentaId, cancellationToken);
            
            if (cuenta == null)
                throw new InvalidOperationException("La cuenta especificada no existe.");

            // Buscar el detalle antes de removerlo
            var detalle = cuenta.Detalles.FirstOrDefault(d => d.Id == request.DetalleId);
            if (detalle != null)
            {
                // Guardrail de Anulación: Bloquear si es estudio de Imagenología procesado o con Informe adjunto
                if (_context != null)
                {
                    var orden = await _context.OrdenesImagenes
                        .FirstOrDefaultAsync(o => o.CuentaId == request.CuentaId && o.Estudio == detalle.Descripcion, cancellationToken);

                    if (orden != null && (orden.Estado.Equals("Procesado", StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(orden.LinkInforme)))
                    {
                        throw new InvalidOperationException("No se puede anular un servicio de imagenología que ya ha sido procesado o cuenta con un informe adjunto.");
                    }
                }

                // Liberar citas médicas si aplica
                if (request.MedicoId.HasValue && request.HoraCita.HasValue)
                {
                    await _repository.CancelarCitaMedicaAsync(request.CuentaId, request.MedicoId.Value, request.HoraCita.Value, cancellationToken);
                }

                // Remover el estudio base y sus detalles de informe vinculados via DetallePadreId
                var detallesHijos = cuenta.Detalles.Where(d => d.DetallePadreId == request.DetalleId).ToList();
                foreach (var hijo in detallesHijos)
                {
                    cuenta.RemoverServicioPorDetalleId(hijo.Id);
                }

                cuenta.RemoverServicioPorDetalleId(request.DetalleId);
            }

            await _repository.GuardarCambiosAsync(cancellationToken);
            return true;
        }
    }
}
