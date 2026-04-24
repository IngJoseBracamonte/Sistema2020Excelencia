using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class CompletePasswordResetCommand : IRequest<bool>
    {
        public string Username { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
