using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class WorklistDto
    {
        public Guid CuentaId { get; set; }
        public int LegacyOrderId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public List<WorklistItemDto> Items { get; set; } = new();
        public bool TieneAnulados { get; set; }
    }

    public class WorklistItemDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; } // ACTIVO - Cobrado, ANULADO EN CUENTA
        public bool TieneResultados { get; set; }
        public bool EsAnulado { get; set; }
    }

    public class GetWorklistQuery : IRequest<WorklistDto>
    {
        public Guid CuentaId { get; set; }
    }

    public class GetWorklistQueryHandler : IRequestHandler<GetWorklistQuery, WorklistDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public GetWorklistQueryHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<WorklistDto> Handle(GetWorklistQuery request, CancellationToken cancellationToken)
        {
            var cuenta = await _context.CuentasServicios
                .AsNoTracking()
                .Include(c => c.Paciente)
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == request.CuentaId, cancellationToken);

            if (cuenta == null) return null;

            var dto = new WorklistDto
            {
                CuentaId = cuenta.Id,
                LegacyOrderId = cuenta.LegacyOrderId ?? 0,
                PacienteNombre = cuenta.Paciente?.NombreCompleto ?? "Paciente Desconocido",
                PacienteCedula = cuenta.Paciente?.CedulaPasaporte ?? "Sin Cédula"
            };

            if (cuenta.LegacyOrderId == null || cuenta.LegacyOrderId <= 0)
            {
                return dto;
            }

            int legacyOrderId = cuenta.LegacyOrderId.Value;

            // Obtener perfiles del legado con su estado de resultados
            var legacyProfiles = await _legacyRepository.GetProfilesWithResultStatusAsync(legacyOrderId, cancellationToken);

            if (legacyProfiles != null && legacyProfiles.Any())
            {
                foreach (var lp in legacyProfiles)
                {
                    // Buscar en la cuenta nativa por TipoServicioId (Laboratorio = 2) y mapeo directo de ID legacy
                    var nativeDetail = cuenta.Detalles.FirstOrDefault(d => 
                        d.TipoServicioId == TipoServicioConstants.Laboratorio && 
                        d.LegacyMappingId == lp.IdPerfil.ToString());

                    bool esAnulado = nativeDetail == null || nativeDetail.Cantidad <= 0 || nativeDetail.Precio <= 0;

                    dto.Items.Add(new WorklistItemDto
                    {
                        Codigo = lp.IdPerfil.ToString("D3"),
                        Nombre = lp.Descripcion,
                        Estado = esAnulado ? "ANULADO EN CUENTA" : "ACTIVO - Cobrado",
                        TieneResultados = lp.TieneResultados,
                        EsAnulado = esAnulado
                    });

                    if (esAnulado)
                    {
                        dto.TieneAnulados = true;
                    }
                }
            }
            else
            {
                // Fallback para servicios de laboratorio cargados directamente en la cuenta nativa
                var labDetalles = cuenta.Detalles
                    .Where(d => d.TipoServicioId == TipoServicioConstants.Laboratorio || (d.TipoServicio != null && d.TipoServicio.ToUpper() == "LABORATORIO"))
                    .ToList();

                int idx = 1;
                foreach (var d in labDetalles)
                {
                    bool esAnulado = d.Cantidad <= 0 || d.Precio <= 0;
                    dto.Items.Add(new WorklistItemDto
                    {
                        Codigo = idx.ToString("D3"),
                        Nombre = d.Descripcion,
                        Estado = esAnulado ? "ANULADO EN CUENTA" : "ACTIVO - Cobrado",
                        TieneResultados = false,
                        EsAnulado = esAnulado
                    });
                    idx++;
                }
            }

            return dto;
        }
    }
}
