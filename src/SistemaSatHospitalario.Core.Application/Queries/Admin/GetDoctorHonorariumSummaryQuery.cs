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
            // Detección dinámica de zona horaria para servidores locales vs nube
            var serverOffset = TimeZoneInfo.Local.BaseUtcOffset.TotalHours;
            var hoursToAdd = serverOffset == -4 ? 0 : 4;

            var start = request.StartDate.Date.AddHours(hoursToAdd);
            var end = request.EndDate.Date.AddDays(1).AddHours(hoursToAdd).AddTicks(-1);

            // ═══ Paso 1: Consultas Atendidas ═══
            // Fuente primaria de honorarios médicos.
            // Filtro directo por CitasMedicas.Estado + rango de fecha en HoraPautada.
            // Se restringe la unión de detalles para evitar asignación errónea de otros servicios en la misma cuenta.
            var fromCitas = await (
                from cita in _context.CitasMedicas
                join detail in _context.DetallesServicioCuenta
                    on cita.CuentaServicioId equals detail.CuentaServicioId
                join medico in _context.Medicos
                    on cita.MedicoId equals medico.Id
                where cita.Estado == EstadoConstants.Atendida
                   && cita.HoraPautada >= start
                   && cita.HoraPautada <= end
                   && (detail.TipoServicio == "MEDICO" || detail.TipoServicio == "Medico" || detail.TipoServicio.Contains("CONS") || detail.TipoServicio.Contains("MEDI") || detail.CategoriaHonorario == HonorarioConstants.CategoriaConsulta)
                   && (detail.MedicoResponsableId == cita.MedicoId || detail.MedicoResponsableId == null)
                select new
                {
                    MedicoId = (Guid?)(detail.MedicoResponsableId ?? cita.MedicoId),
                    detail.Honorario,
                    detail.Precio,
                    detail.Cantidad,
                    detail.CategoriaHonorario,
                    MedicoHonorarioBase = medico.HonorarioBase
                }
            ).ToListAsync(cancellationToken);

            // ═══ Paso 2: Servicios Técnicos Realizados ═══
            // Ej: RX, Laboratorio marcados como Realizado con médico y honorario asignado.
            var fromServicios = await (
                from detail in _context.DetallesServicioCuenta
                join cs in _context.CuentasServicios on detail.CuentaServicioId equals cs.Id
                where detail.Realizado
                   && detail.Honorario > 0
                   && detail.MedicoResponsableId != null
                   && !(detail.TipoServicio == "MEDICO" || detail.TipoServicio == "Medico" || detail.TipoServicio.Contains("CONS") || detail.TipoServicio.Contains("MEDI") || detail.CategoriaHonorario == HonorarioConstants.CategoriaConsulta)
                   && (cs.FechaCierre ?? cs.FechaCarga) >= start
                   && (cs.FechaCierre ?? cs.FechaCarga) <= end
                select new
                {
                    MedicoId = (Guid?)detail.MedicoResponsableId,
                    detail.Honorario,
                    detail.Precio,
                    detail.Cantidad,
                    detail.CategoriaHonorario
                }
            ).ToListAsync(cancellationToken);

            // ═══ Paso 3: Consolidación en memoria ═══
            var allItems = fromCitas.Select(x => new
                {
                    x.MedicoId,
                    // Cadena de fallback: Honorario explícito en detalle -> Honorario base del médico -> Precio del servicio
                    Honorario = x.Honorario > 0 ? x.Honorario : (x.MedicoHonorarioBase > 0 ? x.MedicoHonorarioBase : x.Precio),
                    x.Cantidad,
                    Categoria = x.CategoriaHonorario ?? HonorarioConstants.CategoriaConsulta
                })
                .Concat(fromServicios.Select(x => new
                {
                    x.MedicoId,
                    Honorario = x.Honorario,
                    x.Cantidad,
                    Categoria = x.CategoriaHonorario ?? HonorarioConstants.CategoriaOtros
                }))
                .Where(x => x.MedicoId.HasValue && x.Honorario > 0)
                .ToList();

            // ═══ Paso 4: Nombres de médicos ═══
            var medicoIds = allItems.Select(x => x.MedicoId!.Value).Distinct().ToList();
            var medicos = await _context.Medicos
                .Where(m => medicoIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Nombre, cancellationToken);

            // ═══ Paso 5: Agrupación y resultado ═══
            return allItems
                .GroupBy(x => x.MedicoId!.Value)
                .Select(g => new DoctorHonorariumSummaryDto
                {
                    MedicoId = g.Key,
                    MedicoNombre = medicos.GetValueOrDefault(g.Key, "Médico Desconocido"),
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
        }
    }
}
