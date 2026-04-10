using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetDoctorHonorariumSummaryQuery : IRequest<List<DoctorHonorariumSummaryDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetDoctorHonorariumSummaryQueryHandler : IRequestHandler<GetDoctorHonorariumSummaryQuery, List<DoctorHonorariumSummaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetDoctorHonorariumSummaryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorHonorariumSummaryDto>> Handle(GetDoctorHonorariumSummaryQuery request, CancellationToken cancellationToken)
        {
            // Normalizar fechas para abarcar el día completo (00:00:00 a 23:59:59)
            var start = request.StartDate.Date;
            var end = request.EndDate.Date.AddDays(1).AddTicks(-1);

            // Obtenemos las citas facturadas en el rango (V10.1 SQL Strategy)
            // Se asume que el honorario se paga tras el cierre de cuenta (Estado = Facturada)
            var summary = await (from cita in _context.CitasMedicas
                                 join cs in _context.CuentasServicios on cita.CuentaServicioId equals cs.Id
                                 join detail in _context.DetallesServicioCuenta on cs.Id equals detail.CuentaServicioId
                                 where cs.Estado == EstadoConstants.Facturada
                                    && cs.FechaCierre >= start 
                                    && cs.FechaCierre <= end
                                    && detail.TipoServicio == EstadoConstants.Medico
                                 group detail by new { cita.MedicoId, cita.Medico.Nombre } into g
                                 select new DoctorHonorariumSummaryDto
                                 {
                                     MedicoId = g.Key.MedicoId,
                                     MedicoNombre = g.Key.Nombre,
                                     CantidadServicios = g.Count(),
                                     TotalHonorarios = g.Sum(x => x.Honorario * x.Cantidad)
                                 }).ToListAsync(cancellationToken);

            return summary;
        }
    }
}
