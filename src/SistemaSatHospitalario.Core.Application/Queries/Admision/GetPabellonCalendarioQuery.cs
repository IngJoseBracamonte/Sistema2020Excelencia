using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class CirugiaCalendarioItemDto
    {
        public Guid Id { get; set; }
        public Guid CuentaServicioId { get; set; }
        public Guid PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;

        // Modelos Tipados Fuertemente Normalizados
        public UbicacionPacienteDto Ubicacion { get; set; } = new();
        public TipoIngresoCoberturaDto IngresoCobertura { get; set; } = new();

        public string DescripcionCirugia { get; set; } = string.Empty;
        public string SalaQuirofano { get; set; } = string.Empty;
        public string ModalidadAnestesia { get; set; } = string.Empty;
        public bool EsAlquilado { get; set; }
        public decimal PrecioDerechoSalaUsd { get; set; }
        public decimal PrecioBaseUsd { get; set; }
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public DateTime FechaHoraProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int TotalRequisitos { get; set; }
        public int RequisitosCumplidos { get; set; }
        public bool EstaAptoParaQuirofano => TotalRequisitos > 0 && TotalRequisitos == RequisitosCumplidos;
    }

    public class GetPabellonCalendarioQuery : IRequest<List<CirugiaCalendarioItemDto>>
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? SalaQuirofano { get; set; }
        public string? Estado { get; set; }
    }

    public class GetPabellonCalendarioQueryHandler : IRequestHandler<GetPabellonCalendarioQuery, List<CirugiaCalendarioItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPabellonCalendarioQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<CirugiaCalendarioItemDto>> Handle(GetPabellonCalendarioQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrdenesCirugia
                .AsNoTracking()
                .Include(o => o.Paciente)
                .Include(o => o.Medico)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.AreaClinica)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.CamaRetenida)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.Convenio)
                .Include(o => o.Requisitos)
                .AsQueryable();

            if (request.FechaInicio.HasValue)
            {
                var inicio = request.FechaInicio.Value.Date;
                query = query.Where(o => o.FechaHoraProgramada >= inicio);
            }

            if (request.FechaFin.HasValue)
            {
                var fin = request.FechaFin.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.FechaHoraProgramada <= fin);
            }

            if (!string.IsNullOrWhiteSpace(request.SalaQuirofano))
            {
                query = query.Where(o => o.SalaQuirofano == request.SalaQuirofano.Trim());
            }

            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                var estado = request.Estado.Trim();
                query = query.Where(o => o.Estado == estado);
            }

            var items = await query
                .OrderBy(o => o.FechaHoraProgramada)
                .ToListAsync(cancellationToken);

            return items.Select(o =>
            {
                var areaNombre = o.CuentaServicio?.AreaClinica?.Nombre ?? "Sin Asignar";
                var camaNombre = o.CuentaServicio?.CamaRetenida != null
                    ? o.CuentaServicio.CamaRetenida.Nombre
                    : (o.CuentaServicio?.SubAreaClinica ?? "Sin Cama");
                var ubicacionFormateada = o.CuentaServicio?.CamaRetenida != null
                    ? $"{areaNombre} - {o.CuentaServicio.CamaRetenida.Nombre}"
                    : (o.CuentaServicio?.SubAreaClinica ?? areaNombre);
                var convenioId = o.CuentaServicio?.ConvenioId;
                var tieneConvenio = convenioId.HasValue && convenioId.Value > 0;

                return new CirugiaCalendarioItemDto
                {
                    Id = o.Id,
                    CuentaServicioId = o.CuentaServicioId,
                    PacienteId = o.PacienteId,
                    PacienteNombre = o.Paciente.NombreCorto,
                    PacienteCedula = o.Paciente.CedulaPasaporte,
                    Ubicacion = new UbicacionPacienteDto
                    {
                        AreaClinicaId = o.CuentaServicio?.AreaClinicaId,
                        AreaClinicaNombre = areaNombre,
                        CamaId = o.CuentaServicio?.CamaRetenidaId,
                        CamaNombre = camaNombre,
                        CamaCodigo = o.CuentaServicio?.CamaRetenida?.Codigo ?? string.Empty,
                        DescripcionCompleta = ubicacionFormateada
                    },
                    IngresoCobertura = new TipoIngresoCoberturaDto
                    {
                        Tipo = o.CuentaServicio?.TipoIngreso ?? "Hospitalizacion",
                        ConvenioId = convenioId,
                        ConvenioNombre = o.CuentaServicio?.Convenio?.Nombre,
                        EsAsegurado = tieneConvenio
                    },
                    DescripcionCirugia = o.DescripcionCirugia,
                    SalaQuirofano = o.SalaQuirofano,
                    ModalidadAnestesia = o.ModalidadAnestesia,
                    EsAlquilado = o.EsAlquilado,
                    PrecioDerechoSalaUsd = o.PrecioDerechoSalaUsd,
                    PrecioBaseUsd = o.PrecioBaseUsd,
                    MedicoId = o.MedicoId,
                    MedicoNombre = o.Medico.Nombre,
                    FechaHoraProgramada = o.FechaHoraProgramada,
                    Estado = o.Estado,
                    TotalRequisitos = o.Requisitos.Count,
                    RequisitosCumplidos = o.Requisitos.Count(r => r.Cumplido)
                };
            }).ToList();
        }
    }
}
