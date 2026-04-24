using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class ApprovePasswordResetCommandHandler : IRequestHandler<ApprovePasswordResetCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public ApprovePasswordResetCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(ApprovePasswordResetCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.ApprovePasswordResetAsync(request.RequestId, request.AdminUser);
        }
    }
}
