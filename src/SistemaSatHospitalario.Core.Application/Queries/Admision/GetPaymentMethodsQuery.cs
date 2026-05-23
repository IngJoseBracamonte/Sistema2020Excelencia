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
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsUSD { get; set; }
        public bool IsVuelto { get; set; }
        public int GrupoMoneda { get; set; }
        public bool Activo { get; set; }
        public int Orden { get; set; }
    }

    public class GetPaymentMethodsQuery : IRequest<List<PaymentMethodDto>>
    {
        public bool SoloActivos { get; set; } = true;
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
            var query = _context.CatalogoMetodosPago.AsQueryable();

            if (request.SoloActivos)
            {
                query = query.Where(x => x.Activo);
            }

            return await query
                .OrderBy(x => x.Orden)
                .Select(x => new PaymentMethodDto
                {
                    Id = x.Id,
                    Name = x.Nombre,
                    Value = x.Valor,
                    IsUSD = x.EsUSD,
                    IsVuelto = x.EsVuelto,
                    GrupoMoneda = x.GrupoMoneda,
                    Activo = x.Activo,
                    Orden = x.Orden
                })
                .ToListAsync(cancellationToken);
        }
    }
}
