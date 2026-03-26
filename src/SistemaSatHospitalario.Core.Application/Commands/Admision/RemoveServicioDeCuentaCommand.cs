using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

        public RemoveServicioDeCuentaCommandHandler(IBillingRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoveServicioDeCuentaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _repository.ObtenerCuentaPorIdAsync(request.CuentaId, cancellationToken);
            
            if (cuenta == null)
                throw new InvalidOperationException("La cuenta especificada no existe.");

            // Buscar el detalle antes de removerlo (V4.6 Logic)
            var detalle = cuenta.Detalles.FirstOrDefault(d => d.Id == request.DetalleId);
            if (detalle != null)
            {
                // Si es consulta y tenemos metadata, liberar el slot
                if (request.MedicoId.HasValue && request.HoraCita.HasValue)
                {
                    await _repository.CancelarCitaMedicaAsync(request.CuentaId, request.MedicoId.Value, request.HoraCita.Value, cancellationToken);
                }

                cuenta.RemoverServicioPorDetalleId(request.DetalleId);
            }

            await _repository.GuardarCambiosAsync(cancellationToken);
            return true;
        }
    }
}
