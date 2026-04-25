using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Auth
{
    public class GetPendingResetsQuery : IRequest<List<PasswordResetRequestDto>> { }

    public class GetPendingResetsQueryHandler : IRequestHandler<GetPendingResetsQuery, List<PasswordResetRequestDto>>
    {
        private readonly IIdentityService _identityService;
        public GetPendingResetsQueryHandler(IIdentityService identityService) => _identityService = identityService;

        public async Task<List<PasswordResetRequestDto>> Handle(GetPendingResetsQuery request, CancellationToken ct) 
            => await _identityService.GetPendingResetRequestsAsync();
    }
}
