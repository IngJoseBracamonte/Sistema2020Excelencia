using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class SearchPatientQueryHandler : IRequestHandler<SearchPatientQuery, List<PatientDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public SearchPatientQueryHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<List<PatientDto>> Handle(SearchPatientQuery request, CancellationToken cancellationToken)
        {
            var results = new List<PatientDto>();
            var term = request.SearchTerm?.Trim() ?? "";
            
            if (string.IsNullOrEmpty(term)) return results;

            // 1. Buscar en Sistema Nativo
            var nativePatients = await _context.PacientesAdmision
                .Where(p => p.CedulaPasaporte.Contains(term) || p.NombreCorto.Contains(term))
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var p in nativePatients)
            {
                results.Add(new PatientDto
                {
                    Id = p.Id, // ID numérico unificado
                    Cedula = p.CedulaPasaporte,
                    Nombre = p.NombreCorto,
                    Apellidos = "",
                    Celular = p.TelefonoContact,
                    Source = "Nativo",
                    EsLegacy = false
                });
            }

            // 2. Buscar en Sistema Legacy
            // Nota: El repositorio legacy debe devolver DatosPersonalesLegacy u otro DTO con IdPersona
            var legacyPatients = await _legacyRepository.SearchPatientsLimitedAsync(term, cancellationToken);
            foreach (var p in legacyPatients)
            {
                // Evitar duplicados si ya está en el nativo por ID o Cédula
                if (results.Any(r => r.Id == p.IdPersona || r.Cedula == p.Identificacion)) continue;

                results.Add(new PatientDto
                {
                    Id = p.IdPersona, // ID numérico original del legado
                    Cedula = p.Identificacion,
                    Nombre = p.Nombre1,
                    Apellidos = p.Apellido1,
                    Celular = p.Telefono,
                    Source = "Legacy",
                    EsLegacy = true
                });
            }

            return results;
        }
    }
}
