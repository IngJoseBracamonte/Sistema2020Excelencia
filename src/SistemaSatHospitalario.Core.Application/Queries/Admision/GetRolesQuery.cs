using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetRolesQuery : IRequest<List<string>> { }

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<string>>
    {
        private readonly IIdentityService _identityService;

        public GetRolesQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<List<string>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.GetRolesAsync();
        }
    }
}
