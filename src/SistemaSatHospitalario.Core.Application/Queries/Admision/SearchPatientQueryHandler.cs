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
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

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
                
                // V11.0: Recuperamos mapeos existents para vinculación inmediata
                var legacyIds = legacyPatients.Select(p => p.IdPersona).ToList();
                var nativeMappings = await _context.PacientesAdmision
                    .AsNoTracking()
                    .Where(p => p.IdPacienteLegacy.HasValue && legacyIds.Contains(p.IdPacienteLegacy.Value))
                    .ToDictionaryAsync(p => p.IdPacienteLegacy!.Value, p => p.Id, cancellationToken);

                bool changes = false;
                foreach (var p in legacyPatients)
                {
                    Guid nativeId = Guid.Empty;
                    
                    if (nativeMappings.ContainsKey(p.IdPersona))
                    {
                        nativeId = nativeMappings[p.IdPersona];
                    }
                    else 
                    {
                        // AUTOMATIC ONBOARDING (V11.8 Requirement)
                        // Si no existe localmente, lo creamos de inmediato para garantizar consistencia GUID
                        var fullName = $"{p.Nombre} {p.Apellidos}".Trim();
                        var mainPhone = !string.IsNullOrEmpty(p.Celular) ? p.Celular : p.Telefono;
                        
                        var newNativePatient = new PacienteAdmision(p.Cedula, fullName, mainPhone ?? "", p.IdPersona);
                        await _context.PacientesAdmision.AddAsync(newNativePatient, cancellationToken);
                        nativeId = newNativePatient.Id;
                        changes = true;
                    }

                    results.Add(new PatientDto
                    {
                        Id = nativeId,
                        IdPacienteLegacy = p.IdPersona,
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

                if (changes) {
                    await _context.SaveChangesAsync(cancellationToken);
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
