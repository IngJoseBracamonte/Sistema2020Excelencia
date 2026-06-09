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
    public class GetHistorialModificacionesQuery : IRequest<List<HistorialModificacionCuentaDto>>
    {
        public Guid CuentaServicioId { get; set; }
    }

    public class HistorialModificacionCuentaDto
    {
        public Guid Id { get; set; }
        public Guid CuentaServicioId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string Usuario { get; set; } = null!;
        
        public Guid? PacienteAnteriorId { get; set; }
        public string? PacienteAnteriorNombre { get; set; }
        public Guid? PacienteNuevoId { get; set; }
        public string? PacienteNuevoNombre { get; set; }

        public string? TipoIngresoAnterior { get; set; }
        public string? TipoIngresoNuevo { get; set; }
        public int? ConvenioAnteriorId { get; set; }
        public string? ConvenioAnteriorNombre { get; set; }
        public int? ConvenioNuevoId { get; set; }
        public string? ConvenioNuevoNombre { get; set; }

        public decimal TotalAnteriorUSD { get; set; }
        public decimal TotalNuevoUSD { get; set; }

        public decimal ReciboTotalAnteriorUSD { get; set; }
        public decimal ReciboTotalNuevoUSD { get; set; }
        public decimal ReciboVueltoAnteriorUSD { get; set; }
        public decimal ReciboVueltoNuevoUSD { get; set; }
        public decimal ReciboPagadoUSD { get; set; }

        public decimal CxCSaldoAnteriorUSD { get; set; }
        public decimal CxCSaldoNuevoUSD { get; set; }

        public string? DetalleServiciosCambiosJson { get; set; }
    }

    public class GetHistorialModificacionesQueryHandler : IRequestHandler<GetHistorialModificacionesQuery, List<HistorialModificacionCuentaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetHistorialModificacionesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistorialModificacionCuentaDto>> Handle(GetHistorialModificacionesQuery request, CancellationToken cancellationToken)
        {
            return await _context.HistorialModificacionCuentas
                .AsNoTracking()
                .Where(h => h.CuentaServicioId == request.CuentaServicioId)
                .OrderByDescending(h => h.FechaModificacion)
                .Select(h => new HistorialModificacionCuentaDto
                {
                    Id = h.Id,
                    CuentaServicioId = h.CuentaServicioId,
                    FechaModificacion = h.FechaModificacion,
                    Usuario = h.Usuario,
                    PacienteAnteriorId = h.PacienteAnteriorId,
                    PacienteAnteriorNombre = h.PacienteAnteriorNombre,
                    PacienteNuevoId = h.PacienteNuevoId,
                    PacienteNuevoNombre = h.PacienteNuevoNombre,
                    TipoIngresoAnterior = h.TipoIngresoAnterior,
                    TipoIngresoNuevo = h.TipoIngresoNuevo,
                    ConvenioAnteriorId = h.ConvenioAnteriorId,
                    ConvenioAnteriorNombre = h.ConvenioAnteriorNombre,
                    ConvenioNuevoId = h.ConvenioNuevoId,
                    ConvenioNuevoNombre = h.ConvenioNuevoNombre,
                    TotalAnteriorUSD = h.TotalAnteriorUSD,
                    TotalNuevoUSD = h.TotalNuevoUSD,
                    ReciboTotalAnteriorUSD = h.ReciboTotalAnteriorUSD,
                    ReciboTotalNuevoUSD = h.ReciboTotalNuevoUSD,
                    ReciboVueltoAnteriorUSD = h.ReciboVueltoAnteriorUSD,
                    ReciboVueltoNuevoUSD = h.ReciboVueltoNuevoUSD,
                    ReciboPagadoUSD = h.ReciboPagadoUSD,
                    CxCSaldoAnteriorUSD = h.CxCSaldoAnteriorUSD,
                    CxCSaldoNuevoUSD = h.CxCSaldoNuevoUSD,
                    DetalleServiciosCambiosJson = h.DetalleServiciosCambiosJson
                })
                .ToListAsync(cancellationToken);
        }
    }
}
