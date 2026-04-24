using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class CompletePasswordResetCommandHandler : IRequestHandler<CompletePasswordResetCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public CompletePasswordResetCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(CompletePasswordResetCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new Exception("Las contraseñas no coinciden.");
            }

            return await _identityService.CompletePasswordResetAsync(request.Username, request.NewPassword);
        }
    }
}
