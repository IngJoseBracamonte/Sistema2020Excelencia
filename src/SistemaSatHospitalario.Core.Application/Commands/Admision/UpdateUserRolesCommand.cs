using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateUserRolesCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
    }

    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public UpdateUserRolesCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.UpdateUserRolesAsync(request.UserId, request.Roles);
        }
    }
}
