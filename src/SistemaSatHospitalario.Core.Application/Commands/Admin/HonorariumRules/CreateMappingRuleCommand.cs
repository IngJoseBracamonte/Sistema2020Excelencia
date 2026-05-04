using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin.HonorariumRules
{
    public class CreateMappingRuleCommand : IRequest<Guid>
    {
        public string Pattern { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public MappingRuleType MatchType { get; set; } = MappingRuleType.Contains;
        public int Priority { get; set; }
    }

    public class CreateMappingRuleCommandHandler : IRequestHandler<CreateMappingRuleCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IHonorariumMapperService _mapperService;

        public CreateMappingRuleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IHonorariumMapperService mapperService)
        {
            _context = context;
            _currentUser = currentUser;
            _mapperService = mapperService;
        }

        public async Task<Guid> Handle(CreateMappingRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = new HonorariumMappingRule(
                request.Pattern,
                request.Category,
                request.MatchType,
                request.Priority,
                _currentUser.UserName ?? "Sistema"
            );

            _context.HonorariumMappingRules.Add(rule);
            await _context.SaveChangesAsync(cancellationToken);

            _mapperService.InvalidateCache();

            return rule.Id;
        }
    }
}
