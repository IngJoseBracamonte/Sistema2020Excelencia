using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin
{
    public class UpdateRolePermissionsCommand : IRequest<bool>
    {
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public UpdateRolePermissionsCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.UpdateRolePermissionsAsync(request.RoleName, request.Permissions);
        }
    }
}
