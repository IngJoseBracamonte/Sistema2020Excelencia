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
            var start = request.StartDate.Date;
            var end = request.EndDate.Date.AddDays(1).AddTicks(-1);

            // 1. Fetch all relevant details in the date range
            var rawData = await (from detail in _context.DetallesServicioCuenta
                                 join cs in _context.CuentasServicios on detail.CuentaServicioId equals cs.Id
                                 join cita in _context.CitasMedicas on cs.Id equals cita.CuentaServicioId into citaJoin
                                 from cita in citaJoin.DefaultIfEmpty()
                                 where cs.Estado == EstadoConstants.Facturada
                                    && cs.FechaCierre >= start 
                                    && cs.FechaCierre <= end
                                    && detail.Honorario > 0
                                 select new
                                 {
                                     MedicoId = detail.MedicoResponsableId ?? (cita != null ? cita.MedicoId : (Guid?)null),
                                     detail.Honorario,
                                     detail.Cantidad,
                                     Categoria = detail.CategoriaHonorario ?? (cita != null ? HonorarioConstants.CategoriaConsulta : HonorarioConstants.CategoriaOtros)
                                 }).ToListAsync(cancellationToken);

            // 2. Filter out items without a responsible physician
            var filteredData = rawData.Where(x => x.MedicoId.HasValue).ToList();

            // 3. Get physician names
            var medicoIds = filteredData.Select(x => x.MedicoId!.Value).Distinct().ToList();
            var medicos = await _context.Medicos
                .Where(m => medicoIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Nombre, cancellationToken);

            // 4. Group and aggregate
            var result = filteredData.GroupBy(x => x.MedicoId!.Value)
                .Select(g => new DoctorHonorariumSummaryDto
                {
                    MedicoId = g.Key,
                    MedicoNombre = medicos.ContainsKey(g.Key) ? medicos[g.Key] : "Médico Desconocido",
                    CantidadServicios = g.Count(),
                    TotalHonorarios = g.Sum(x => x.Honorario * x.Cantidad),
                    Desglose = g.GroupBy(x => x.Categoria)
                        .Select(cg => new HonorarioDesgloseCategoriaDto
                        {
                            Categoria = cg.Key,
                            Cantidad = cg.Count(),
                            Total = cg.Sum(x => x.Honorario * x.Cantidad)
                        }).ToList()
                })
                .OrderBy(m => m.MedicoNombre)
                .ToList();

            return result;
        }
    }
}
