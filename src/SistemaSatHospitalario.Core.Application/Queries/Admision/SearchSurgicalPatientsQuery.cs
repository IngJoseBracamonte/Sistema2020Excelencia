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
    public class SearchSurgicalPatientsQuery : IRequest<List<PatientDto>>
    {
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class SearchSurgicalPatientsQueryHandler(IApplicationDbContext context)
        : IRequestHandler<SearchSurgicalPatientsQuery, List<PatientDto>>
    {
        public async Task<List<PatientDto>> Handle(SearchSurgicalPatientsQuery request, CancellationToken cancellationToken)
        {
            var term = request.SearchTerm.Trim();
            if (term.Length < 2)
            {
                return [];
            }

            return await context.PacientesAdmision
                .AsNoTracking()
                .Where(patient => patient.CedulaPasaporte.Contains(term) || patient.NombreCorto.Contains(term))
                .OrderBy(patient => patient.NombreCorto)
                .Take(10)
                .Select(patient => new PatientDto
                {
                    Id = patient.Id,
                    IdPacienteLegacy = patient.IdPacienteLegacy,
                    Cedula = patient.CedulaPasaporte,
                    Nombre = patient.NombreCorto,
                    Celular = patient.TelefonoContact ?? string.Empty,
                    Telefono = patient.TelefonoContact ?? string.Empty,
                    FechaNacimiento = patient.FechaNacimiento.HasValue ? patient.FechaNacimiento.Value.ToString("yyyy-MM-dd") : string.Empty,
                    Direccion = patient.Direccion ?? string.Empty,
                    EsLegacy = patient.IdPacienteLegacy.HasValue,
                    Source = "Nativo"
                })
                .ToListAsync(cancellationToken);
        }
    }
}