using System;
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
    public class GetNurseAuditReportQueryHandler : IRequestHandler<GetNurseAuditReportQuery, List<NurseActivityDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetNurseAuditReportQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<NurseActivityDto>> Handle(GetNurseAuditReportQuery request, CancellationToken cancellationToken)
        {
            var targetStartDate = request.StartDate ?? DateTime.UtcNow.AddDays(-7);
            var targetEndDate = request.EndDate ?? DateTime.UtcNow;

            // Asegurar que el rango sea inclusivo para todo el día final
            targetStartDate = targetStartDate.Date;
            targetEndDate = targetEndDate.Date.AddDays(1).AddTicks(-1);

            // 1. Obtener triages
            var triageQuery = _context.TriagesEnfermeria
                .AsNoTracking()
                .Include(t => t.CuentaServicio)
                    .ThenInclude(c => c.Paciente)
                .Where(t => t.FechaRegistro >= targetStartDate && t.FechaRegistro <= targetEndDate);

            if (!string.IsNullOrEmpty(request.NurseUsername))
            {
                var usernameLower = request.NurseUsername.ToLower();
                triageQuery = triageQuery.Where(t => t.UsuarioRegistro.ToLower().Contains(usernameLower));
            }

            var triages = await triageQuery.ToListAsync(cancellationToken);

            // 2. Obtener cargos de servicios/insumos
            var detailsQuery = _context.DetallesServicioCuenta
                .AsNoTracking()
                .Include(d => d.CuentaServicio)
                    .ThenInclude(c => c.Paciente)
                .Where(d => d.FechaCarga >= targetStartDate && d.FechaCarga <= targetEndDate);

            if (!string.IsNullOrEmpty(request.NurseUsername))
            {
                var usernameLower = request.NurseUsername.ToLower();
                detailsQuery = detailsQuery.Where(d => d.UsuarioCarga.ToLower().Contains(usernameLower));
            }

            var details = await detailsQuery.ToListAsync(cancellationToken);

            // 3. Consolidar y ordenar
            var list = new List<NurseActivityDto>();

            foreach (var t in triages)
            {
                var paciente = t.CuentaServicio?.Paciente;
                string pacienteNombre = paciente != null ? paciente.NombreCompleto : "Desconocido";
                string pacienteCedula = paciente?.CedulaPasaporte ?? "N/A";

                string stateDesc = !string.IsNullOrEmpty(t.DescripcionRapida) ? $" | Estado: {t.DescripcionRapida}" : "";

                list.Add(new NurseActivityDto
                {
                    Fecha = t.FechaRegistro,
                    Usuario = t.UsuarioRegistro,
                    PacienteCedula = pacienteCedula,
                    PacienteNombre = pacienteNombre,
                    TipoActividad = "Triage / Constantes Vitales",
                    Detalle = $"Motivo: {t.MotivoConsulta} | TA: {t.TensionArterial} | FC: {t.FrecuenciaCardiaca} LPM | FR: {t.FrecuenciaRespiratoria} RPM | Temp: {t.Temperatura} °C | SatO2: {t.SaturacionO2}%" + stateDesc
                });
            }

            foreach (var d in details)
            {
                var paciente = d.CuentaServicio?.Paciente;
                string pacienteNombre = paciente != null ? paciente.NombreCompleto : "Desconocido";
                string pacienteCedula = paciente?.CedulaPasaporte ?? "N/A";

                list.Add(new NurseActivityDto
                {
                    Fecha = d.FechaCarga,
                    Usuario = d.UsuarioCarga,
                    PacienteCedula = pacienteCedula,
                    PacienteNombre = pacienteNombre,
                    TipoActividad = $"Carga de {d.TipoServicio}",
                    Detalle = $"Descripción: {d.Descripcion} | Cantidad: {d.Cantidad}"
                });
            }

            return list.OrderByDescending(x => x.Fecha).ToList();
        }
    }
}
