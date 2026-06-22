using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

            // Calcular el acumulado en tiempo real para las cajas que siguen abiertas
            var openCajaIds = list.Where(c => c.Estado == EstadoConstants.CajaAbierta).Select(c => c.Id).ToList();
            if (openCajaIds.Any())
            {
                var catalogoMetodos = await _context.CatalogoMetodosPago
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Orden)
                    .ToListAsync(cancellationToken);

                var openCajaRecibos = await _context.RecibosFactura
                    .Include(r => r.DetallesPago)
                    .Where(r => r.CajaDiariaId.HasValue && openCajaIds.Contains(r.CajaDiariaId.Value) && r.EstadoFiscal != EstadoConstants.Anulada)
                    .ToListAsync(cancellationToken);

                var receiptsByCaja = openCajaRecibos.GroupBy(r => r.CajaDiariaId!.Value).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in list)
                {
                    if (item.Estado == EstadoConstants.CajaAbierta)
                    {
                        var recibos = receiptsByCaja.TryGetValue(item.Id, out var rList) ? rList : new List<ReciboFactura>();
                        var allPayments = recibos.SelectMany(r => r.DetallesPago).ToList();

                        var listMetodosDesglose = new List<object>();
                        decimal totalCobradoBaseUSD = 0;

                        var metodosPrincipales = catalogoMetodos.Where(m => !m.EsVuelto).ToList();
                        foreach (var metodo in metodosPrincipales)
                        {
                            string vueltoMetodoValor = string.Empty;
                            if (metodo.Valor == "Dolar Efectivo") vueltoMetodoValor = "Vuelto Efectivo USD";
                            else if (metodo.Valor == "Efectivo BS") vueltoMetodoValor = "Vuelto Efectivo BS";
                            else if (metodo.Valor == "Pago Movil") vueltoMetodoValor = "Vuelto Pago Movil";

                            var pagosMetodo = allPayments.Where(p => p.MetodoPago == metodo.Valor && p.MontoAbonadoMoneda > 0).ToList();
                            decimal esperadoIngresoOriginal = pagosMetodo.Sum(p => p.MontoAbonadoMoneda);
                            decimal esperadoIngresoBase = pagosMetodo.Sum(p => p.EquivalenteAbonadoBase);

                            decimal esperadoVueltosOriginal = 0;
                            decimal esperadoVueltosBase = 0;
                            if (!string.IsNullOrEmpty(vueltoMetodoValor))
                            {
                                var vueltosMetodo = allPayments.Where(p => p.MetodoPago == vueltoMetodoValor).ToList();
                                esperadoVueltosOriginal = Math.Abs(vueltosMetodo.Sum(p => p.MontoAbonadoMoneda));
                                esperadoVueltosBase = Math.Abs(vueltosMetodo.Sum(p => p.EquivalenteAbonadoBase));
                            }

                            decimal esperadoNetoOriginal = esperadoIngresoOriginal - esperadoVueltosOriginal;
                            decimal esperadoNetoBase = esperadoIngresoBase - esperadoVueltosBase;

                            totalCobradoBaseUSD += esperadoNetoBase;

                            listMetodosDesglose.Add(new
                            {
                                MetodoPago = metodo.Valor,
                                Nombre = metodo.Nombre,
                                EsUSD = metodo.EsUSD,
                                MontoIngreso = esperadoIngresoOriginal,
                                MontoVueltos = esperadoVueltosOriginal,
                                TotalDeclarado = esperadoNetoOriginal,
                                MontoEsperadoIngreso = esperadoIngresoOriginal,
                                MontoEsperadoVueltos = esperadoVueltosOriginal,
                                TotalEsperado = esperadoNetoOriginal,
                                DiferenciaOriginal = 0m,
                                DiferenciaBase = 0m
                            });
                        }

                        item.TotalCobrado = totalCobradoBaseUSD;
                        item.TotalIngresado = totalCobradoBaseUSD;
                        item.Diferencia = 0m;
                        item.DeclaracionCierreJson = JsonSerializer.Serialize(listMetodosDesglose);
                    }
                }
            }

            // Calcular estadísticas del día de hoy para el panel de consolidación
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var cajasHoy = list.Where(c => c.Apertura >= today && c.Apertura < tomorrow).ToList();

            var cajasActivas = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaAbierta);
            var cierresPendientes = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaCerradaPorAsistente);
            var cierresRealizados = cajasHoy.Count(c => c.Estado == EstadoConstants.CajaCerrada);

            // Excluir cajas abiertas de la suma de recaudado/esperado diario para no alterar la diferencia neta diaria
            decimal totalRecaudado = cajasHoy.Where(c => c.Estado != EstadoConstants.CajaAbierta).Sum(c => c.TotalIngresado ?? 0);
            decimal totalEsperado = cajasHoy.Where(c => c.Estado != EstadoConstants.CajaAbierta).Sum(c => c.TotalCobrado ?? 0);
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
