using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class PaymentMethodDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsUSD { get; set; }
        public bool IsVuelto { get; set; }
    }

    public class GetPaymentMethodsQuery : IRequest<List<PaymentMethodDto>>
    {
    }

    public class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, List<PaymentMethodDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPaymentMethodsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            return await _context.CatalogoMetodosPago
                .Where(x => x.Activo)
                .OrderBy(x => x.Orden)
                .Select(x => new PaymentMethodDto
                {
                    Name = x.Nombre,
                    Value = x.Valor,
                    IsUSD = x.EsUSD,
                    IsVuelto = x.EsVuelto
                })
                .ToListAsync(cancellationToken);
        }
    }
}
