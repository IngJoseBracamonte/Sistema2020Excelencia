using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, bool>
    {
        private readonly IIdentityService _identityService;
        private readonly INotificationService _notificationService;

        public RequestPasswordResetCommandHandler(IIdentityService identityService, INotificationService notificationService)
        {
            _identityService = identityService;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.RequestPasswordResetAsync(request.Username);
            
            if (result)
            {
                await _notificationService.SendNotificationToGroupAsync("Admin", 
                    "Nueva Solicitud de Reset de Password", 
                    $"El usuario {request.Username} ha solicitado un reset de contraseña.");
            }

            return result;
        }
    }
}
