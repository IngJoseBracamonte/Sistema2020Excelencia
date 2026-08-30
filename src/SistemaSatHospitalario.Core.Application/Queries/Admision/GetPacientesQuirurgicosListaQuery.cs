using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    /// <summary>
    /// Objeto de valor que modela la ubicación física hospitalaria (Área Clínica y Cama asignada).
    /// </summary>
    public class UbicacionPacienteDto
    {
        public Guid? AreaClinicaId { get; set; }
        public string AreaClinicaNombre { get; set; } = "Sin Asignar";
        public Guid? CamaId { get; set; }
        public string CamaNombre { get; set; } = "Sin Cama";
        public string CamaCodigo { get; set; } = string.Empty;
        public string DescripcionCompleta { get; set; } = "Sin Asignar";
    }

    /// <summary>
    /// Objeto de valor que modela el régimen de admisión y esquema de cobertura/convenio del paciente.
    /// </summary>
    public class TipoIngresoCoberturaDto
    {
        public string Tipo { get; set; } = "Hospitalizacion";
        public int? ConvenioId { get; set; }
        public string? ConvenioNombre { get; set; }
        public bool EsAsegurado { get; set; }
    }

    public class MedicoHonorarioItemDto
    {
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public Guid EspecialidadId { get; set; }
        public string EspecialidadNombre { get; set; } = string.Empty;
        public decimal MontoHonorarioUsd { get; set; }
        public bool EsCirujanoPrincipal { get; set; }
    }

    public class SolicitudInsumoDetalleDto
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string InsumoNombre { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public decimal CantidadSolicitada { get; set; }
        public string EstadoSolicitud { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string UsuarioSolicitud { get; set; } = string.Empty;
    }

    public class PacienteQuirurgicoItemDto
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
        public Guid MedicoPrincipalId { get; set; }
        public string MedicoPrincipalNombre { get; set; } = string.Empty;
        public DateTime FechaHoraProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;

        // Listados contextuales
        public List<OrdenCirugiaRequisitoDto> Requisitos { get; set; } = new();
        public List<MedicoHonorarioItemDto> MedicosHonorarios { get; set; } = new();
        public List<InsumoCirugiaConsumoDto> InsumosAsignados { get; set; } = new();
        public List<SolicitudInsumoDetalleDto> SolicitudesInsumos { get; set; } = new();
        public List<CirugiaLogDto> Logs { get; set; } = new();
    }

    public class GetPacientesQuirurgicosListaQuery : IRequest<List<PacienteQuirurgicoItemDto>>
    {
        public string? Busqueda { get; set; }
        public string? Estado { get; set; }
    }

    public class GetPacientesQuirurgicosListaQueryHandler : IRequestHandler<GetPacientesQuirurgicosListaQuery, List<PacienteQuirurgicoItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPacientesQuirurgicosListaQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<PacienteQuirurgicoItemDto>> Handle(GetPacientesQuirurgicosListaQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrdenesCirugia
                .AsNoTracking()
                .Include(o => o.Paciente)
                .Include(o => o.Medico)
                    .ThenInclude(m => m.Especialidad)
                .Include(o => o.SedeQuirofano)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.AreaClinica)
                        .ThenInclude(a => a.Sede)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.CamaRetenida)
                        .ThenInclude(a => a.Sede)
                .Include(o => o.CuentaServicio)
                    .ThenInclude(c => c.Convenio)
                .Include(o => o.Requisitos)
                    .ThenInclude(r => r.RequisitoCirugia)
                .Include(o => o.MedicosHonorarios)
                    .ThenInclude(m => m.Medico)
                .Include(o => o.MedicosHonorarios)
                    .ThenInclude(m => m.Especialidad)
                .Include(o => o.SolicitudesInsumos)
                    .ThenInclude(s => s.Insumo)
                .Include(o => o.Logs)
                .AsQueryable();

            query = ApplyFilters(query, request);

            var ordenes = await query
                .OrderByDescending(o => o.FechaHoraProgramada)
                .ToListAsync(cancellationToken);

            var cuentasIds = ordenes.Select(o => o.CuentaServicioId).Distinct().ToList();

            var insumosPorCuenta = await _context.InsumosCirugiasPacientes
                .AsNoTracking()
                .Include(i => i.Insumo)
                .Where(i => cuentasIds.Contains(i.CuentaServicioId))
                .ToListAsync(cancellationToken);

            return ordenes.Select(o =>
            {
                var sedeNombre = o.CuentaServicio?.AreaClinica?.Sede?.Nombre ?? (o.SedeQuirofano?.Nombre ?? string.Empty);
                var camaNombre = o.CuentaServicio?.CamaRetenida?.Nombre ?? (o.CuentaServicio?.AreaClinica?.Nombre ?? o.SalaQuirofano);
                var ubicacionFormateada = !string.IsNullOrWhiteSpace(sedeNombre) && !string.IsNullOrWhiteSpace(camaNombre)
                    ? $"{sedeNombre} - {camaNombre}"
                    : (o.CuentaServicio?.SubAreaClinica ?? (!string.IsNullOrWhiteSpace(camaNombre) ? camaNombre : "Sin Asignar"));
                var convenioId = o.CuentaServicio?.ConvenioId;
                var tieneConvenio = convenioId.HasValue && convenioId.Value > 0;

                return new PacienteQuirurgicoItemDto
                {
                    Id = o.Id,
                    CuentaServicioId = o.CuentaServicioId,
                    PacienteId = o.PacienteId,
                    PacienteNombre = o.Paciente.NombreCorto,
                    PacienteCedula = o.Paciente.CedulaPasaporte,
                    Ubicacion = new UbicacionPacienteDto
                    {
                        AreaClinicaId = o.CuentaServicio?.AreaClinicaId,
                        AreaClinicaNombre = o.CuentaServicio?.AreaClinica?.Nombre ?? (!string.IsNullOrWhiteSpace(sedeNombre) ? sedeNombre : "Sin Asignar"),
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
                    MedicoPrincipalId = o.MedicoId,
                    MedicoPrincipalNombre = o.Medico.Nombre,
                    FechaHoraProgramada = o.FechaHoraProgramada,
                    Estado = o.Estado,
                    FechaCreacion = o.FechaCreacion,
                    UsuarioCreacion = o.UsuarioCreacion,
                    Requisitos = o.Requisitos.Select(r => new OrdenCirugiaRequisitoDto
                    {
                        Id = r.Id,
                        RequisitoCirugiaId = r.RequisitoCirugiaId,
                        Nombre = r.RequisitoCirugia?.Nombre ?? "Requisito",
                        Descripcion = r.RequisitoCirugia?.Descripcion ?? "",
                        Cumplido = r.Cumplido,
                        FechaVerificacion = r.FechaVerificacion,
                        VerificadoPor = r.VerificadoPor
                    }).ToList(),
                    MedicosHonorarios = o.MedicosHonorarios.Any()
                        ? o.MedicosHonorarios.Select(m => new MedicoHonorarioItemDto
                        {
                            Id = m.Id,
                            MedicoId = m.MedicoId,
                            MedicoNombre = m.Medico?.Nombre ?? "Desconocido",
                            EspecialidadId = m.EspecialidadId,
                            EspecialidadNombre = m.Especialidad?.Nombre ?? (m.Medico?.Especialidad?.Nombre ?? "General"),
                            MontoHonorarioUsd = m.MontoHonorarioUsd,
                            EsCirujanoPrincipal = m.EsCirujanoPrincipal
                        }).ToList()
                        : (o.Medico != null ? new List<MedicoHonorarioItemDto>
                        {
                            new MedicoHonorarioItemDto
                            {
                                Id = Guid.NewGuid(),
                                MedicoId = o.Medico.Id,
                                MedicoNombre = o.Medico.Nombre,
                                EspecialidadId = o.Medico.EspecialidadId,
                                EspecialidadNombre = o.Medico.Especialidad?.Nombre ?? "Cirugía General",
                                MontoHonorarioUsd = o.PrecioBaseUsd,
                                EsCirujanoPrincipal = true
                            }
                        } : new List<MedicoHonorarioItemDto>()),
                    InsumosAsignados = insumosPorCuenta
                        .Where(i => i.CuentaServicioId == o.CuentaServicioId)
                        .Select(i => new InsumoCirugiaConsumoDto
                        {
                            InsumoId = i.InsumoId,
                            InsumoNombre = i.Insumo?.Nombre ?? "",
                            InsumoCodigo = i.Insumo?.Codigo ?? "",
                            CantidadEntregada = i.CantidadEntregada,
                            CantidadDevuelta = i.CantidadDevuelta,
                            CantidadConsumida = i.CantidadConsumida,
                            PrecioUnitarioUsd = i.Insumo?.CostoUnitarioBaseUSD ?? 0
                        }).ToList(),
                    SolicitudesInsumos = o.SolicitudesInsumos.Select(s => new SolicitudInsumoDetalleDto
                    {
                        Id = s.Id,
                        InsumoId = s.InsumoId,
                        InsumoNombre = s.Insumo?.Nombre ?? "",
                        InsumoCodigo = s.Insumo?.Codigo ?? "",
                        CantidadSolicitada = s.CantidadSolicitada,
                        EstadoSolicitud = s.EstadoSolicitud,
                        FechaSolicitud = s.FechaSolicitud,
                        UsuarioSolicitud = s.UsuarioSolicitud
                    }).ToList(),
                    Logs = o.Logs.Select(l => new CirugiaLogDto
                    {
                        Id = l.Id,
                        OrdenCirugiaId = l.OrdenCirugiaId,
                        UsuarioId = l.UsuarioId,
                        Evento = l.Evento,
                        Detalle = l.Detalle,
                        Timestamp = l.Timestamp
                    }).ToList()
                };
            }).ToList();
        }

        private static IQueryable<OrdenCirugia> ApplyFilters(
            IQueryable<OrdenCirugia> query,
            GetPacientesQuirurgicosListaQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                var estado = request.Estado.Trim();
                query = query.Where(o => o.Estado == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.Busqueda))
            {
                var search = request.Busqueda.Trim().ToLower();
                query = query.Where(o =>
                    o.Paciente.NombreCorto.ToLower().Contains(search) ||
                    o.Paciente.CedulaPasaporte.ToLower().Contains(search) ||
                    o.DescripcionCirugia.ToLower().Contains(search));
            }

            return query;
        }
    }
}
