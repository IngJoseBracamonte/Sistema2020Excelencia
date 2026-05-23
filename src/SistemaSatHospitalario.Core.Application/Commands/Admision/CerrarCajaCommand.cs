using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Text.Json;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class MetodoDeclaradoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public decimal MontoIngreso { get; set; }
        public decimal MontoVueltos { get; set; }
    }

    public class CerrarCajaCommand : IRequest<CerrarCajaResult>
    {
        public string UsuarioId { get; set; } = string.Empty;
        public List<MetodoDeclaradoDto> Declaracion { get; set; } = new();
    }

    public class CerrarCajaCommandHandler : IRequestHandler<CerrarCajaCommand, CerrarCajaResult>
    {
        private readonly ICajaAdministrativaRepository _repository;
        private readonly IApplicationDbContext _context;

        public CerrarCajaCommandHandler(ICajaAdministrativaRepository repository, IApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CerrarCajaResult> Handle(CerrarCajaCommand request, CancellationToken cancellationToken)
        {
            var cajaAbierta = await _repository.ObtenerCajaAbiertaPorUsuarioAsync(request.UsuarioId, cancellationToken);
            if (cajaAbierta == null)
            {
                throw new InvalidOperationException("No se encontró ninguna caja abierta para este usuario.");
            }

            // Cargar catálogo de métodos de pago activos
            var catalogoMetodos = await _context.CatalogoMetodosPago
                .Where(m => m.Activo)
                .ToListAsync(cancellationToken);

            // Cargar recibos y pagos vinculados a esta caja
            var recibos = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .Where(r => r.CajaDiariaId == cajaAbierta.Id)
                .ToListAsync(cancellationToken);

            var allPayments = recibos.SelectMany(r => r.DetallesPago).ToList();

            var listMetodosDesglose = new List<object>();
            decimal totalIngresadoBaseUSD = 0;
            decimal totalCobradoBaseUSD = 0;

            // Agrupar los métodos del catálogo que no son vueltos
            var metodosPrincipales = catalogoMetodos.Where(m => !m.EsVuelto).OrderBy(m => m.Orden).ToList();

            foreach (var metodo in metodosPrincipales)
            {
                var declaracionMetodo = request.Declaracion.FirstOrDefault(d => d.MetodoPago == metodo.Valor) 
                                        ?? new MetodoDeclaradoDto { MetodoPago = metodo.Valor };

                // Encontrar el método de vuelto asociado
                string vueltoMetodoValor = string.Empty;
                if (metodo.Valor == "Dolar Efectivo") vueltoMetodoValor = "Vuelto Efectivo USD";
                else if (metodo.Valor == "Efectivo BS") vueltoMetodoValor = "Vuelto Efectivo BS";
                else if (metodo.Valor == "Pago Movil") vueltoMetodoValor = "Vuelto Pago Movil";

                // Calcular esperados del sistema
                // Ingreso esperado (pagos positivos)
                var pagosMetodo = allPayments.Where(p => p.MetodoPago == metodo.Valor && p.MontoAbonadoMoneda > 0).ToList();
                decimal esperadoIngresoOriginal = pagosMetodo.Sum(p => p.MontoAbonadoMoneda);
                decimal esperadoIngresoBase = pagosMetodo.Sum(p => p.EquivalenteAbonadoBase);

                // Vueltos esperados (pagos negativos del método de vuelto asociado)
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

                // Declarado por el usuario
                decimal declaradoIngresoOriginal = declaracionMetodo.MontoIngreso;
                decimal declaradoVueltosOriginal = declaracionMetodo.MontoVueltos;
                decimal declaradoNetoOriginal = declaradoIngresoOriginal - declaradoVueltosOriginal;

                // Convertir lo declarado a USD base para auditoría consolidada
                // Usar la tasa del día de la caja (si no hay pagos, usar 1 o tasa de la configuración)
                decimal tasaCambioCaja = recibos.FirstOrDefault(r => r.TasaCambioDia > 0)?.TasaCambioDia ?? 1;
                
                decimal declaradoIngresoBase = (metodo.GrupoMoneda == 1) ? declaradoIngresoOriginal : (tasaCambioCaja > 0 ? declaradoIngresoOriginal / tasaCambioCaja : 0);
                decimal declaradoVueltosBase = (metodo.GrupoMoneda == 1) ? declaradoVueltosOriginal : (tasaCambioCaja > 0 ? declaradoVueltosOriginal / tasaCambioCaja : 0);
                decimal declaradoNetoBase = declaradoIngresoBase - declaradoVueltosBase;

                // Diferencia en moneda original
                decimal diferenciaOriginal = declaradoNetoOriginal - esperadoNetoOriginal;
                decimal diferenciaBase = declaradoNetoBase - esperadoNetoBase;

                totalIngresadoBaseUSD += declaradoNetoBase;
                totalCobradoBaseUSD += esperadoNetoBase;

                listMetodosDesglose.Add(new
                {
                    MetodoPago = metodo.Valor,
                    Nombre = metodo.Nombre,
                    EsUSD = metodo.EsUSD,
                    MontoIngreso = declaradoIngresoOriginal,
                    MontoVueltos = declaradoVueltosOriginal,
                    TotalDeclarado = declaradoNetoOriginal,
                    MontoEsperadoIngreso = esperadoIngresoOriginal,
                    MontoEsperadoVueltos = esperadoVueltosOriginal,
                    TotalEsperado = esperadoNetoOriginal,
                    DiferenciaOriginal = diferenciaOriginal,
                    DiferenciaBase = diferenciaBase
                });
            }

            decimal totalDiferenciaUSD = totalIngresadoBaseUSD - totalCobradoBaseUSD;
            string declaracionJson = JsonSerializer.Serialize(listMetodosDesglose);

            // Cambiar estado a CerradaPorAsistente
            cajaAbierta.CerrarPorAsistente(declaracionJson, totalIngresadoBaseUSD, totalCobradoBaseUSD, totalDiferenciaUSD);
            await _repository.GuardarCambiosAsync(cancellationToken);

            return new CerrarCajaResult
            {
                CajaId = cajaAbierta.Id,
                TotalIngresosUSD = totalIngresadoBaseUSD,
                TotalVueltoUSD = request.Declaracion.Sum(d => d.MontoVueltos), // Declarado
                TotalIngresosBS = request.Declaracion.Where(d => d.MetodoPago == "Efectivo BS" || d.MetodoPago == "Pago Movil" || d.MetodoPago == "Punto").Sum(d => d.MontoIngreso),
                ConteoVentas = recibos.Count,
                Usuario = cajaAbierta.NombreUsuario,
                FechaCierre = DateTime.UtcNow
            };
        }
    }
}
