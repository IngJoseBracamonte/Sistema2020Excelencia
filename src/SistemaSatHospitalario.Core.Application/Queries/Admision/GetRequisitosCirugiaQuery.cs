using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class RequisitoCirugiaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
    }

    public class GetRequisitosCirugiaQuery : IRequest<List<RequisitoCirugiaDto>>
    {
        public bool SoloActivos { get; set; } = true;
    }

    public class GetRequisitosCirugiaQueryHandler : IRequestHandler<GetRequisitosCirugiaQuery, List<RequisitoCirugiaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetRequisitosCirugiaQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<RequisitoCirugiaDto>> Handle(GetRequisitosCirugiaQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RequisitosCirugia.AsNoTracking();

            if (request.SoloActivos)
            {
                query = query.Where(r => r.EsActivo);
            }

            return await query
                .OrderBy(r => r.Nombre)
                .Select(r => new RequisitoCirugiaDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    EsActivo = r.EsActivo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
