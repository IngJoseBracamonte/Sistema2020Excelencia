using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetSecurityConfigQuery : IRequest<SecurityConfigDto>
    {
    }

    public class SecurityConfigDto
    {
        public List<RoleDto> Roles { get; set; }
        public List<string> AvailablePermissions { get; set; }
    }

    public class GetSecurityConfigQueryHandler : IRequestHandler<GetSecurityConfigQuery, SecurityConfigDto>
    {
        private readonly IIdentityService _identityService;

        public GetSecurityConfigQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<SecurityConfigDto> Handle(GetSecurityConfigQuery request, CancellationToken cancellationToken)
        {
            var roles = await _identityService.GetRolesListAsync();
            
            // Get all static permissions from constants
            var permissions = typeof(SistemaSatHospitalario.Core.Domain.Constants.PermissionConstants)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string) && f.Name != "Type")
                .Select(f => (string)f.GetValue(null))
                .ToList();

            return new SecurityConfigDto
            {
                Roles = roles,
                AvailablePermissions = permissions
            };
        }
    }
}
