using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.DTOs;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, JwtAuthResult>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<JwtAuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // La capa Application no sabe de EF Core ni IdentityUserManager,
            // sólo orquesta a través de IAuthService
            return await _authService.AuthenticateAsync(request.Username, request.Password, cancellationToken);
        }
    }
}
