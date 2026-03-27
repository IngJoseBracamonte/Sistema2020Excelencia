using System;
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

            // EXCLUSIVIDAD LEGACY: Según requerimiento Corporativo Pachón Pro, 
            // no se busca en el sistema nativo para evitar duplicidad o colisión de Cédulas.
            try
            {
                var legacyPatients = await _legacyRepository.SearchPatientsLimitedAsync(term, cancellationToken);
                
                // V11.0: Recuperamos mapeos existentes para vinculación inmediata
                var legacyIds = legacyPatients.Select(p => p.IdPersona).ToList();
                var nativeMappings = await _context.PacientesAdmision
                    .AsNoTracking()
                    .Where(p => p.IdPacienteLegacy.HasValue && legacyIds.Contains(p.IdPacienteLegacy.Value))
                    .ToDictionaryAsync(p => p.IdPacienteLegacy!.Value, p => p.Id, cancellationToken);

                foreach (var p in legacyPatients)
                {
                    results.Add(new PatientDto
                    {
                        Id = p.IdPersona,
                        InternalId = nativeMappings.ContainsKey(p.IdPersona) ? nativeMappings[p.IdPersona] : Guid.Empty,
                        Cedula = p.Cedula,
                        Nombre = p.Nombre,
                        Apellidos = p.Apellidos,
                        Sexo = p.Sexo,
                        Correo = (p.Correo ?? "") + (p.TipoCorreo ?? ""),
                        Celular = (p.CodigoCelular ?? "") + (p.Celular ?? ""),
                        Source = "Legacy",
                        EsLegacy = true
                    });
                }
            }
            catch (global::System.Exception ex)
            {
                global::System.Console.WriteLine($"[LEGACY EXCLUSIVE SEARCH ERROR] {ex.Message}");
            }

            return results;
        }
    }
}
