using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ForzarCierreCajaCommand : IRequest<Guid>
    {
        public Guid CajaId { get; set; }
    }

    public class ForzarCierreCajaCommandHandler : IRequestHandler<ForzarCierreCajaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public ForzarCierreCajaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(ForzarCierreCajaCommand request, CancellationToken cancellationToken)
        {
            // Buscar la caja por Id
            var caja = await _context.CajasDiarias
                .FirstOrDefaultAsync(c => c.Id == request.CajaId, cancellationToken);

            if (caja == null)
            {
                throw new InvalidOperationException("No se encontró la caja especificada.");
            }

            if (caja.Estado != EstadoConstants.CajaAbierta)
            {
                throw new InvalidOperationException("La caja ya no se encuentra abierta.");
            }

            // Cargar catálogo de métodos de pago activos
            var catalogoMetodos = await _context.CatalogoMetodosPago
                .Where(m => m.Activo)
                .ToListAsync(cancellationToken);

            // Cargar recibos y pagos vinculados a esta caja
            var recibos = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .Where(r => r.CajaDiariaId == caja.Id)
                .ToListAsync(cancellationToken);

            var allPayments = recibos.SelectMany(r => r.DetallesPago).ToList();

            var listMetodosDesglose = new List<object>();
            decimal totalCobradoBaseUSD = 0;

            // Agrupar los métodos del catálogo que no son vueltos
            var metodosPrincipales = catalogoMetodos.Where(m => !m.EsVuelto).OrderBy(m => m.Orden).ToList();

            foreach (var metodo in metodosPrincipales)
            {
                // Encontrar el método de vuelto asociado
                string vueltoMetodoValor = string.Empty;
                if (metodo.Valor == "Dolar Efectivo") vueltoMetodoValor = "Vuelto Efectivo USD";
                else if (metodo.Valor == "Efectivo BS") vueltoMetodoValor = "Vuelto Efectivo BS";
                else if (metodo.Valor == "Pago Movil") vueltoMetodoValor = "Vuelto Pago Movil";

                // Calcular esperados del sistema
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

                // Forzar cierre: el monto ingresado físico declarado coincide exactamente con lo esperado del sistema
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
                    DiferenciaBase = 0m,
                    Forzado = true
                });
            }

            string declaracionJson = JsonSerializer.Serialize(listMetodosDesglose);

            // Transición a CerradaPorAsistente de forma automatizada por el Administrador (Diferencia = 0)
            caja.CerrarPorAsistente(declaracionJson, totalCobradoBaseUSD, totalCobradoBaseUSD, 0m);

            await _context.SaveChangesAsync(cancellationToken);

            return caja.Id;
        }
    }
}
