using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetAllMedicosQuery : IRequest<List<MedicoDto>>
    {
    }

    public class GetAllMedicosQueryHandler : IRequestHandler<GetAllMedicosQuery, List<MedicoDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllMedicosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicoDto>> Handle(GetAllMedicosQuery request, CancellationToken cancellationToken)
        {
            return await _context.Medicos
                .Select(m => new MedicoDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Especialidad = m.Especialidad.Nombre,
                    Activo = m.Activo
                })
                .ToListAsync(cancellationToken);
        }
    }
}
