using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ConsolidarCajasResult
    {
        public decimal TotalRecaudado { get; set; }
        public decimal TotalEsperado { get; set; }
        public decimal DiferenciaNeta { get; set; }
        public int CajasActivas { get; set; }
        public int CierresPendientes { get; set; }
        public int CierresRealizados { get; set; }
        public decimal EfectivoEnBoveda { get; set; }
    }

    public class ConsolidarCajasCommand : IRequest<ConsolidarCajasResult>
    {
    }

    public class ConsolidarCajasCommandHandler : IRequestHandler<ConsolidarCajasCommand, ConsolidarCajasResult>
    {
        private readonly IApplicationDbContext _context;

        public ConsolidarCajasCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ConsolidarCajasResult> Handle(ConsolidarCajasCommand request, CancellationToken cancellationToken)
        {
            // Obtener todas las cajas diarias del día de hoy
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var cajasHoy = await _context.CajasDiarias
                .Where(c => c.FechaApertura >= today && c.FechaApertura < tomorrow)
                .ToListAsync(cancellationToken);

            // Consolidar todas las cajas que estén "CerradaPorAsistente"
            var cajasPorConsolidar = cajasHoy.Where(c => c.Estado == EstadoConstants.CajaCerradaPorAsistente).ToList();
            foreach (var caja in cajasPorConsolidar)
            {
                caja.ConsolidarCaja();
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Recalcular métricas para el resultado
            var cajasActivas = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaAbierta);
            var cierresPendientes = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaCerradaPorAsistente);
            var cierresRealizados = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaCerrada);

            decimal totalRecaudado = cajasHoy.Where(c => c.Estado == EstadoConstants.CajaCerrada).Sum(c => c.TotalIngresado ?? 0);
            decimal totalEsperado = cajasHoy.Where(c => c.Estado == EstadoConstants.CajaCerrada).Sum(c => c.TotalCobrado ?? 0);
            decimal diferenciaNeta = totalRecaudado - totalEsperado;

            // Efectivo en Bóveda: sumar lo ingresado en Efectivo de las cajas cerradas/consolidadas
            // Para esto, sumamos el "TotalIngresado" de las cajas consolidadas (que ya está expresado en USD base)
            decimal efectivoEnBoveda = totalRecaudado;

            return new ConsolidarCajasResult
            {
                TotalRecaudado = totalRecaudado,
                TotalEsperado = totalEsperado,
                DiferenciaNeta = diferenciaNeta,
                CajasActivas = cajasActivas,
                CierresPendientes = cierresPendientes,
                CierresRealizados = cierresRealizados,
                EfectivoEnBoveda = efectivoEnBoveda
            };
        }
    }
}
