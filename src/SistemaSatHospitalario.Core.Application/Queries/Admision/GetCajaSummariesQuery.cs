using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
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
        public int EstadoId { get; set; }

        public decimal? TotalIngresado { get; set; }
        public decimal? TotalCobrado { get; set; }
        public decimal? Diferencia { get; set; }
        public string? DeclaracionCierreJson { get; set; }
        public List<CajaDeclaracionMetodoDto> Declaraciones { get; set; } = new();
    }

    public class CajaDeclaracionMetodoDto
    {
        public Guid MetodoPagoId { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string NombreMetodoPago { get; set; } = string.Empty;
        public decimal MontoIngreso { get; set; }
        public decimal MontoVueltos { get; set; }
        public decimal MontoEsperadoIngreso { get; set; }
        public decimal MontoEsperadoVueltos { get; set; }
        public decimal DiferenciaOriginal { get; set; }
        public decimal DiferenciaBase { get; set; }
    }

    public class GetCajaSummariesQueryHandler : IRequestHandler<GetCajaSummariesQuery, CajaSummaryDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserResolverService _userResolver;

        public GetCajaSummariesQueryHandler(IApplicationDbContext context, IUserResolverService userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<CajaSummaryDto> Handle(
    GetCajaSummariesQuery request,
    CancellationToken cancellationToken)
        {
            var start = request.Desde.Date;
            var end = request.Hasta.Date.AddDays(1).AddTicks(-1);

            var query = _context.CajasDiarias
                .AsNoTracking()
                .Include(c => c.DeclaracionesPorMetodo)
                    .ThenInclude(d => d.MetodoPago)
                .Where(c => c.FechaApertura >= start && c.FechaApertura <= end);

            if (!string.IsNullOrEmpty(request.UsuarioId))
            {
                if (Guid.TryParse(request.UsuarioId, out var userIdGuid))
                {
                    query = query.Where(c => c.UsuarioIdentityId == userIdGuid);
                }
                else
                {
                    var resolvedId = _userResolver.GetCurrentUserId();
                    query = query.Where(c => c.UsuarioIdentityId == resolvedId);
                }
            }

            var listCajas = await query
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync(cancellationToken);

            var cajaUserIds = listCajas
                .Where(c => c.UsuarioIdentityId.HasValue)
                .Select(c => c.UsuarioIdentityId!.Value)
                .Distinct()
                .ToList();

            var userMap = await _userResolver.GetDisplayNameMapAsync(
                cajaUserIds,
                cancellationToken); 

            var list = listCajas.Select(c => new CajaDetailDto
            {
                Id = c.Id,
                Usuario = c.UsuarioIdentityId.HasValue &&
                          userMap.TryGetValue(c.UsuarioIdentityId.Value, out var nombre)
                    ? nombre
                    : "Sistema",
                Apertura = c.FechaApertura,
                Cierre = c.FechaCierre,
                MontoInicialDivisa = c.MontoInicialDivisa,
                MontoInicialBs = c.MontoInicialBs,
                Estado = EstadoCajaConstants.ToLegacyString(c.EstadoId),
                EstadoId = c.EstadoId,
                Declaraciones = c.DeclaracionesPorMetodo.Select(d => new CajaDeclaracionMetodoDto
                {
                    MetodoPagoId = d.MetodoPagoId,
                    MetodoPago = d.MetodoPago?.Valor ?? string.Empty,
                    NombreMetodoPago = d.MetodoPago?.Nombre ?? string.Empty,
                    MontoIngreso = d.MontoIngresado,
                    MontoVueltos = d.MontoVueltos,
                    MontoEsperadoIngreso = d.MontoEsperadoIngreso,
                    MontoEsperadoVueltos = d.MontoEsperadoVueltos,
                    DiferenciaOriginal = d.DiferenciaOriginal,
                    DiferenciaBase = d.DiferenciaBase
                }).ToList()
            }).ToList();

            var openCajaIds = list
                .Where(c => c.EstadoId == EstadoCajaConstants.AbiertaId)
                .Select(c => c.Id)
                .ToList();

            if (openCajaIds.Any())
            {
                var catalogoMetodos = await _context.CatalogoMetodosPago
                    .AsNoTracking()
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Orden)
                    .ToListAsync(cancellationToken);

                var openCajaRecibos = await _context.RecibosFactura
                    .AsNoTracking()
                    .Include(r => r.DetallesPago)
                    .Where(r =>
                        r.CajaDiariaId.HasValue &&
                        openCajaIds.Contains(r.CajaDiariaId.Value) &&
                        r.EstadoFiscalNav.Nombre != EstadoConstants.Anulada)
                    .ToListAsync(cancellationToken);

                var receiptsByCaja = openCajaRecibos
                    .GroupBy(r => r.CajaDiariaId!.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in list)
                {
                    if (item.EstadoId != EstadoCajaConstants.AbiertaId)
                        continue;

                    var recibos = receiptsByCaja.TryGetValue(item.Id, out var rList)
                        ? rList
                        : new List<ReciboFactura>();

                    var allPayments = recibos
                        .SelectMany(r => r.DetallesPago)
                        .ToList();

                    var declaraciones = new List<CajaDeclaracionMetodoDto>();
                    decimal totalCobradoBaseUSD = 0;

                    var metodosPrincipales = catalogoMetodos
                        .Where(m => !m.EsVuelto)
                        .ToList();

                    foreach (var metodo in metodosPrincipales)
                    {
                        string vueltoMetodoValor = metodo.Valor switch
                        {
                            "Dolar Efectivo" => "Vuelto Efectivo USD",
                            "Efectivo BS" => "Vuelto Efectivo BS",
                            "Pago Movil" => "Vuelto Pago Movil",
                            _ => string.Empty
                        };

                        var pagosMetodo = allPayments
                            .Where(p =>
                                p.MetodoPagoId == metodo.Id &&
                                p.MontoAbonadoMoneda > 0)
                            .ToList();

                        decimal esperadoIngresoOriginal =
                            pagosMetodo.Sum(p => p.MontoAbonadoMoneda);

                        decimal esperadoIngresoBase =
                            pagosMetodo.Sum(p => p.EquivalenteAbonadoBase);

                        decimal esperadoVueltosOriginal = 0;
                        decimal esperadoVueltosBase = 0;

                        if (!string.IsNullOrEmpty(vueltoMetodoValor))
                        {
                            var metodoVuelto = catalogoMetodos
                                .FirstOrDefault(m => m.Valor == vueltoMetodoValor);

                            if (metodoVuelto != null)
                            {
                                var vueltosMetodo = allPayments
                                    .Where(p => p.MetodoPagoId == metodoVuelto.Id)
                                    .ToList();

                                esperadoVueltosOriginal = Math.Abs(
                                    vueltosMetodo.Sum(p => p.MontoAbonadoMoneda));

                                esperadoVueltosBase = Math.Abs(
                                    vueltosMetodo.Sum(p => p.EquivalenteAbonadoBase));
                            }
                        }

                        decimal esperadoNetoBase =
                            esperadoIngresoBase - esperadoVueltosBase;

                        totalCobradoBaseUSD += esperadoNetoBase;

                        declaraciones.Add(new CajaDeclaracionMetodoDto
                        {
                            MetodoPagoId = metodo.Id,
                            MetodoPago = metodo.Valor,
                            NombreMetodoPago = metodo.Nombre,
                            MontoIngreso = esperadoIngresoOriginal,
                            MontoVueltos = esperadoVueltosOriginal,
                            MontoEsperadoIngreso = esperadoIngresoOriginal,
                            MontoEsperadoVueltos = esperadoVueltosOriginal,
                            DiferenciaOriginal = 0m,
                            DiferenciaBase = 0m
                        });
                    }

                    item.TotalCobrado = totalCobradoBaseUSD;
                    item.TotalIngresado = totalCobradoBaseUSD;
                    item.Diferencia = 0m;
                    item.Declaraciones = declaraciones;
                }
            }

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var cajasHoy = list
                .Where(c => c.Apertura >= today && c.Apertura < tomorrow)
                .ToList();

            var cajasActivas = cajasHoy.Count(
                c => c.EstadoId == EstadoCajaConstants.AbiertaId);

            var cierresPendientes = cajasHoy.Count(
                c => c.EstadoId == EstadoCajaConstants.CerradaPorAsistenteId);

            var cierresRealizados = cajasHoy.Count(
                c => c.EstadoId == EstadoCajaConstants.CerradaId);

            decimal totalRecaudado = cajasHoy
                .Where(c => c.EstadoId != EstadoCajaConstants.AbiertaId)
                .Sum(c => c.TotalIngresado ?? 0);

            decimal totalEsperado = cajasHoy
                .Where(c => c.EstadoId != EstadoCajaConstants.AbiertaId)
                .Sum(c => c.TotalCobrado ?? 0);

            decimal diferenciaNeta = totalRecaudado - totalEsperado;
            decimal efectivoEnBoveda = totalRecaudado;

            decimal granTotalDivisa = list.Sum(
                x => x.TotalIngresado ?? x.MontoInicialDivisa);

            decimal granTotalBs = list.Sum(
                x => x.MontoInicialBs);

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