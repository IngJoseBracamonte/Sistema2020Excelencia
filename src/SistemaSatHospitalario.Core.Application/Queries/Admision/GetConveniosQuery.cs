using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetConveniosQuery : IRequest<List<SeguroConvenioDto>> { }

    public class GetConveniosQueryHandler : IRequestHandler<GetConveniosQuery, List<SeguroConvenioDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetConveniosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SeguroConvenioDto>> Handle(GetConveniosQuery request, CancellationToken cancellationToken)
        {
            return await _context.SegurosConvenios
                .Select(s => new SeguroConvenioDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Rtn = s.Rtn,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    Email = s.Email,
                    Activo = s.Activo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
