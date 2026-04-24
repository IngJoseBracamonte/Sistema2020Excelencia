using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ValidarCuentaCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public string UsuarioValidador { get; set; } = string.Empty;
    }

    public class ValidarCuentaCommandHandler : IRequestHandler<ValidarCuentaCommand, bool>
    {
        private readonly IBillingRepository _billingRepository;

        public ValidarCuentaCommandHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<bool> Handle(ValidarCuentaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _billingRepository.ObtenerCuentaPorIdAsync(request.CuentaId, cancellationToken);
            if (cuenta == null) return false;

            cuenta.Validar(request.UsuarioValidador);
            
            await _billingRepository.ActualizarCuentaAsync(cuenta, cancellationToken);
            await _billingRepository.GuardarCambiosAsync(cancellationToken);
            
            return true;
        }
    }
}
