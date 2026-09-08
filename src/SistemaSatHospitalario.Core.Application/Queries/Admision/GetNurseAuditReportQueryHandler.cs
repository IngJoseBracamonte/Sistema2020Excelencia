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
        private readonly IUserResolverService _userResolver;

        public GetNurseAuditReportQueryHandler(IApplicationDbContext context, IUserResolverService userResolver)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userResolver = userResolver;
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
                triageQuery = triageQuery.Where(t => t.UsuarioRegistroId != null && t.UsuarioRegistroId.ToString()!.ToLower().Contains(usernameLower));
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
                var resolvedId = await _userResolver.ResolveUserIdByUsernameAsync(request.NurseUsername, cancellationToken);
                detailsQuery = resolvedId.HasValue
                    ? detailsQuery.Where(d => d.UsuarioCargaId == resolvedId.Value)
                    : detailsQuery.Where(d => false);
            }

            var details = await detailsQuery.ToListAsync(cancellationToken);

            var detailUserIds = details
                .Where(d => d.UsuarioCargaId.HasValue)
                .Select(d => d.UsuarioCargaId!.Value)
                .Distinct()
                .ToList();

            var userMap = await _userResolver.GetDisplayNameMapAsync(detailUserIds, cancellationToken);

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
                    Usuario = t.UsuarioRegistroId?.ToString() ?? "",
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
                    Usuario = d.UsuarioCargaId.HasValue && userMap.TryGetValue(d.UsuarioCargaId.Value, out var nombre) ? nombre : "Sistema",
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
