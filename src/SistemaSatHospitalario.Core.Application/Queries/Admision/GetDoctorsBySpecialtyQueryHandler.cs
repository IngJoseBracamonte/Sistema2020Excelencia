using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDoctorsBySpecialtyQueryHandler : IRequestHandler<GetDoctorsBySpecialtyQuery, List<DoctorDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDoctorsBySpecialtyQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorDto>> Handle(GetDoctorsBySpecialtyQuery request, CancellationToken cancellationToken)
        {
            var search = request.Especialidad.ToUpper().Trim();
            return await _context.Medicos
                .Where(m => m.Especialidad.ToUpper().Contains(search) && m.Activo)
                .Select(m => new DoctorDto
                {
                    Id = m.Id,
                    Nombre = m.Nombre,
                    Especialidad = m.Especialidad
                })
                .ToListAsync(cancellationToken);
        }
    }
}
