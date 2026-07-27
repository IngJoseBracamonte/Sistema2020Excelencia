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
                query = query.Where(o => o.Estado == request.Estado);
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
                    UsuarioCreacion = o.UsuarioCreacion
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
                .FirstOrDefaultAsync(o => o.Id == request.OrdenCirugiaId, cancellationToken);

            if (orden == null) return null;

            // Obtener insumos cargados a la cuenta del paciente
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
