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
        public Guid ServicioId { get; set; }
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

            cuenta.RemoverServicio(request.ServicioId);

            await _repository.GuardarCambiosAsync(cancellationToken);
            return true;
        }
    }
}
