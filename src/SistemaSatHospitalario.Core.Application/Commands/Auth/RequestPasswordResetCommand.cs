using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class RequestPasswordResetCommand : IRequest<bool>
    {
        public string Username { get; set; } = string.Empty;
    }
}
