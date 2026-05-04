using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin.HonorariumRules
{
    public class DeleteMappingRuleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteMappingRuleCommandHandler : IRequestHandler<DeleteMappingRuleCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHonorariumMapperService _mapperService;

        public DeleteMappingRuleCommandHandler(IApplicationDbContext context, IHonorariumMapperService mapperService)
        {
            _context = context;
            _mapperService = mapperService;
        }

        public async Task<Unit> Handle(DeleteMappingRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _context.HonorariumMappingRules.FindAsync(new object[] { request.Id }, cancellationToken);
            if (rule != null)
            {
                _context.HonorariumMappingRules.Remove(rule);
                await _context.SaveChangesAsync(cancellationToken);
                _mapperService.InvalidateCache();
            }

            return Unit.Value;
        }
    }
}
