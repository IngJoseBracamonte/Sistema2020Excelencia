using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetDoctorHonorariaReportQuery : IRequest<List<DoctorHonorariaDto>>
    {
    }

    public class DoctorHonorariaDto
    {
        public Guid MedicoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public decimal HonorarioBase { get; set; }
        public int TotalConsultasMes { get; set; }
        public bool Activo { get; set; }
    }

    public class GetDoctorHonorariaReportQueryHandler : IRequestHandler<GetDoctorHonorariaReportQuery, List<DoctorHonorariaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDoctorHonorariaReportQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorHonorariaDto>> Handle(GetDoctorHonorariaReportQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var report = await _context.Medicos
                .Include(m => m.Especialidad)
                .AsNoTracking()
                .Select(m => new DoctorHonorariaDto
                {
                    MedicoId = m.Id,
                    Nombre = m.Nombre,
                    Especialidad = m.Especialidad.Nombre,
                    HonorarioBase = m.HonorarioBase,
                    Activo = m.Activo,
                    // Conteo de citas atendidas o pendientes en el mes actual
                    TotalConsultasMes = _context.CitasMedicas
                        .Count(c => c.MedicoId == m.Id && c.HoraPautada >= startOfMonth)
                })
                .OrderBy(m => m.Nombre)
                .ToListAsync(cancellationToken);

            return report;
        }
    }
}
