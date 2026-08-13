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
    public class OrdenCirugiaRequisitoDto
    {
        public Guid Id { get; set; }
        public Guid RequisitoCirugiaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Cumplido { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string? VerificadoPor { get; set; }
    }

    public class CirugiaObservacionHistorialDto
    {
        public Guid Id { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string UsuarioRegistro { get; set; } = string.Empty;
    }

    public class OrdenCirugiaDto
    {
        public Guid Id { get; set; }
        public Guid CuentaServicioId { get; set; }
        public Guid PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;
        public string DescripcionCirugia { get; set; } = string.Empty;
        public decimal PrecioBaseUsd { get; set; }
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public DateTime FechaHoraProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? MotivoCancelacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public List<OrdenCirugiaRequisitoDto> Requisitos { get; set; } = new();
        public List<CirugiaObservacionHistorialDto> HistorialObservaciones { get; set; } = new();
    }

    public class CirugiaLogDto
    {
        public Guid Id { get; set; }
        public Guid OrdenCirugiaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class OrdenCirugiaDetalleDto : OrdenCirugiaDto
    {
        public List<CirugiaLogDto> Logs { get; set; } = new();
        public List<InsumoCirugiaConsumoDto> InsumosAsignados { get; set; } = new();
    }

    public class InsumoCirugiaConsumoDto
    {
        public Guid InsumoId { get; set; }
        public string InsumoNombre { get; set; } = string.Empty;
        public string InsumoCodigo { get; set; } = string.Empty;
        public decimal CantidadEntregada { get; set; }
        public decimal CantidadDevuelta { get; set; }
        public decimal CantidadConsumida { get; set; }
        public decimal PrecioUnitarioUsd { get; set; }
    }

    public class GetOrdenesCirugiaQuery : IRequest<List<OrdenCirugiaDto>>
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Estado { get; set; }
    }

    public class GetOrdenesCirugiaQueryHandler : IRequestHandler<GetOrdenesCirugiaQuery, List<OrdenCirugiaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdenesCirugiaQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<OrdenCirugiaDto>> Handle(GetOrdenesCirugiaQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrdenesCirugia
                .AsNoTracking()
                .Include(o => o.Paciente)
                .Include(o => o.Medico)
                .Include(o => o.Requisitos)
                    .ThenInclude(r => r.RequisitoCirugia)
                .Include(o => o.HistorialObservaciones)
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

            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                var targetState = request.Estado.Trim();
                query = query.Where(o => o.Estado == targetState ||
                    (targetState == "Programada" && o.Estado == "PendienteEjecucion") ||
                    (targetState == "EnCirugia" && o.Estado == "EnProceso") ||
                    (targetState == "Finalizado" && o.Estado == "Completada"));
            }

            return await query
                .OrderBy(o => o.FechaHoraProgramada)
                .Select(o => new OrdenCirugiaDto
                {
                    Id = o.Id,
                    CuentaServicioId = o.CuentaServicioId,
                    PacienteId = o.PacienteId,
                    PacienteNombre = o.Paciente.NombreCorto,
                    PacienteCedula = o.Paciente.CedulaPasaporte,
                    DescripcionCirugia = o.DescripcionCirugia,
                    PrecioBaseUsd = o.PrecioBaseUsd,
                    MedicoId = o.MedicoId,
                    MedicoNombre = o.Medico.Nombre,
                    FechaHoraProgramada = o.FechaHoraProgramada,
                    Estado = o.Estado,
                    MotivoCancelacion = o.MotivoCancelacion,
                    FechaCreacion = o.FechaCreacion,
                    UsuarioCreacion = o.UsuarioCreacion,
                    Requisitos = o.Requisitos.Select(r => new OrdenCirugiaRequisitoDto
                    {
                        Id = r.Id,
                        RequisitoCirugiaId = r.RequisitoCirugiaId,
                        Nombre = r.RequisitoCirugia.Nombre,
                        Descripcion = r.RequisitoCirugia.Descripcion,
                        Cumplido = r.Cumplido,
                        FechaVerificacion = r.FechaVerificacion,
                        VerificadoPor = r.VerificadoPor
                    }).ToList(),
                    HistorialObservaciones = o.HistorialObservaciones.Select(h => new CirugiaObservacionHistorialDto
                    {
                        Id = h.Id,
                        Observacion = h.Observacion,
                        Tipo = h.Tipo.ToString(),
                        FechaRegistro = h.FechaRegistro,
                        UsuarioRegistro = h.UsuarioRegistro
                    }).OrderByDescending(h => h.FechaRegistro).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }

    public class GetOrdenCirugiaDetalleQuery : IRequest<OrdenCirugiaDetalleDto?>
    {
        public Guid OrdenCirugiaId { get; set; }
    }

    public class GetOrdenCirugiaDetalleQueryHandler : IRequestHandler<GetOrdenCirugiaDetalleQuery, OrdenCirugiaDetalleDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdenCirugiaDetalleQueryHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<OrdenCirugiaDetalleDto?> Handle(GetOrdenCirugiaDetalleQuery request, CancellationToken cancellationToken)
        {
            var orden = await _context.OrdenesCirugia
                .AsNoTracking()
                .Include(o => o.Paciente)
                .Include(o => o.Medico)
                .Include(o => o.Logs)
                .Include(o => o.Requisitos)
                    .ThenInclude(r => r.RequisitoCirugia)
                .Include(o => o.HistorialObservaciones)
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null) return null;

            var insumosAsignados = await _context.InsumosCirugiasPacientes
                .AsNoTracking()
                .Include(i => i.Insumo)
                .Where(i => i.CuentaServicioId == orden.CuentaServicioId)
                .Select(i => new InsumoCirugiaConsumoDto
                {
                    InsumoId = i.InsumoId,
                    InsumoNombre = i.Insumo.Nombre,
                    InsumoCodigo = i.Insumo.Codigo,
                    CantidadEntregada = i.CantidadEntregada,
                    CantidadDevuelta = i.CantidadDevuelta,
                    CantidadConsumida = i.CantidadConsumida,
                    PrecioUnitarioUsd = i.Insumo.CostoUnitarioBaseUSD
                })
                .ToListAsync(cancellationToken);

            return new OrdenCirugiaDetalleDto
            {
                Id = orden.Id,
                CuentaServicioId = orden.CuentaServicioId,
                PacienteId = orden.PacienteId,
                PacienteNombre = orden.Paciente.NombreCorto,
                PacienteCedula = orden.Paciente.CedulaPasaporte,
                DescripcionCirugia = orden.DescripcionCirugia,
                PrecioBaseUsd = orden.PrecioBaseUsd,
                MedicoId = orden.MedicoId,
                MedicoNombre = orden.Medico.Nombre,
                FechaHoraProgramada = orden.FechaHoraProgramada,
                Estado = orden.Estado,
                MotivoCancelacion = orden.MotivoCancelacion,
                FechaCreacion = orden.FechaCreacion,
                UsuarioCreacion = orden.UsuarioCreacion,
                Requisitos = orden.Requisitos.Select(r => new OrdenCirugiaRequisitoDto
                {
                    Id = r.Id,
                    RequisitoCirugiaId = r.RequisitoCirugiaId,
                    Nombre = r.RequisitoCirugia.Nombre,
                    Descripcion = r.RequisitoCirugia.Descripcion,
                    Cumplido = r.Cumplido,
                    FechaVerificacion = r.FechaVerificacion,
                    VerificadoPor = r.VerificadoPor
                }).ToList(),
                HistorialObservaciones = orden.HistorialObservaciones.Select(h => new CirugiaObservacionHistorialDto
                {
                    Id = h.Id,
                    Observacion = h.Observacion,
                    Tipo = h.Tipo.ToString(),
                    FechaRegistro = h.FechaRegistro,
                    UsuarioRegistro = h.UsuarioRegistro
                }).OrderByDescending(h => h.FechaRegistro).ToList(),
                Logs = orden.Logs.Select(l => new CirugiaLogDto
                {
                    Id = l.Id,
                    OrdenCirugiaId = l.OrdenCirugiaId,
                    UsuarioId = l.UsuarioId,
                    Evento = l.Evento,
                    Detalle = l.Detalle,
                    Timestamp = l.Timestamp
                }).OrderByDescending(l => l.Timestamp).ToList(),
                InsumosAsignados = insumosAsignados
            };
        }
    }
}
