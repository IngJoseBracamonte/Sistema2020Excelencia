using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AuditarCuentaCommand : IRequest<bool>
    {
        public Guid CuentaId { get; set; }
        public string UsuarioAuditor { get; set; } = string.Empty;
    }

    public class AuditarCuentaCommandHandler : IRequestHandler<AuditarCuentaCommand, bool>
    {
        private readonly IBillingRepository _billingRepository;

        public AuditarCuentaCommandHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<bool> Handle(AuditarCuentaCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _billingRepository.ObtenerCuentaPorIdAsync(request.CuentaId, cancellationToken);
            if (cuenta == null) return false;

            cuenta.Auditar(request.UsuarioAuditor);
            
            await _billingRepository.ActualizarCuentaAsync(cuenta, cancellationToken);
            await _billingRepository.GuardarCambiosAsync(cancellationToken);
            
            return true;
        }
    }
}
