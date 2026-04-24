using MediatR;
using System;

namespace SistemaSatHospitalario.Core.Application.Commands.Auth
{
    public class ApprovePasswordResetCommand : IRequest<bool>
    {
        public Guid RequestId { get; set; }
        public string AdminUser { get; set; } = string.Empty;
    }
}
