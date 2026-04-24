using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AuditARCommand : IRequest<bool>
    {
        public Guid ArId { get; set; }
        public string UsuarioAuditor { get; set; } = string.Empty;
    }
}
