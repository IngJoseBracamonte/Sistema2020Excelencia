using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetCajaSummariesQuery : IRequest<CajaSummaryDto>
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? UsuarioId { get; set; }
    }

    public class CajaSummaryDto
    {
        public decimal GranTotalDivisa { get; set; }
        public decimal GranTotalBs { get; set; }
        public List<CajaDetailDto> Cierres { get; set; } = new();

        // Métricas de consolidación en tiempo real (Fase 2)
        public int CajasActivas { get; set; }
        public int CierresPendientes { get; set; }
        public int CierresRealizados { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal TotalEsperado { get; set; }
        public decimal DiferenciaNeta { get; set; }
        public decimal EfectivoEnBoveda { get; set; }
    }

    public class CajaDetailDto
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public decimal MontoInicialDivisa { get; set; }
        public decimal MontoInicialBs { get; set; }
        public string Estado { get; set; } = string.Empty;
        
        // Campos de auditoría (V13.0)
        public decimal? TotalIngresado { get; set; }
        public decimal? TotalCobrado { get; set; }
        public decimal? Diferencia { get; set; }
        public string? DeclaracionCierreJson { get; set; }
    }

    public class GetCajaSummariesQueryHandler : IRequestHandler<GetCajaSummariesQuery, CajaSummaryDto>
    {
        private readonly IApplicationDbContext _context;

        public GetCajaSummariesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CajaSummaryDto> Handle(GetCajaSummariesQuery request, CancellationToken cancellationToken)
        {
            var start = request.Desde.Date;
            var end = request.Hasta.Date.AddDays(1).AddTicks(-1);

            // Consultar todas las cajas dentro del rango de fechas
            var query = _context.CajasDiarias
                .Where(c => c.FechaApertura >= start && c.FechaApertura <= end);

            if (!string.IsNullOrEmpty(request.UsuarioId))
            {
                query = query.Where(c => c.UsuarioId == request.UsuarioId);
            }

            var listCajas = await query
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync(cancellationToken);

            var list = listCajas.Select(c => new CajaDetailDto
            {
                Id = c.Id,
                Usuario = c.NombreUsuario,
                Apertura = c.FechaApertura,
                Cierre = c.FechaCierre,
                MontoInicialDivisa = c.MontoInicialDivisa,
                MontoInicialBs = c.MontoInicialBs,
                Estado = c.Estado,
                TotalIngresado = c.TotalIngresado,
                TotalCobrado = c.TotalCobrado,
                Diferencia = c.Diferencia,
                DeclaracionCierreJson = c.DeclaracionCierreJson
            }).ToList();

            // Calcular estadísticas del día de hoy para el panel de consolidación
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var cajasHoy = list.Where(c => c.Apertura >= today && c.Apertura < tomorrow).ToList();

            var cajasActivas = cajasHoy.Count(c => c.Estado == "Abierta");
            var cierresPendientes = cajasHoy.Count(c => c.Estado == "CerradaPorAsistente");
            var cierresRealizados = cajasHoy.Count(c => c.Estado == "Cerrada");

            decimal totalRecaudado = cajasHoy.Sum(c => c.TotalIngresado ?? 0);
            decimal totalEsperado = cajasHoy.Sum(c => c.TotalCobrado ?? 0);
            decimal diferenciaNeta = totalRecaudado - totalEsperado;
            decimal efectivoEnBoveda = totalRecaudado;

            // Histórico general
            decimal granTotalDivisa = list.Sum(x => x.TotalIngresado ?? x.MontoInicialDivisa);
            decimal granTotalBs = list.Sum(x => x.MontoInicialBs);

            return new CajaSummaryDto
            {
                Cierres = list,
                GranTotalDivisa = granTotalDivisa,
                GranTotalBs = granTotalBs,
                CajasActivas = cajasActivas,
                CierresPendientes = cierresPendientes,
                CierresRealizados = cierresRealizados,
                TotalRecaudado = totalRecaudado,
                TotalEsperado = totalEsperado,
                DiferenciaNeta = diferenciaNeta,
                EfectivoEnBoveda = efectivoEnBoveda
            };
        }
    }
}
