using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DetallesPagoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public string ReferenciaBancaria { get; set; } = string.Empty;
        public decimal MontoAbonadoMoneda { get; set; }
        public decimal EquivalenteAbonadoBase { get; set; }
    }

    public class RegistrarReciboFacturaCommand : IRequest<Guid>
    {
        public Guid CuentaServicioId { get; set; }
        public string CajeroUserId { get; set; } = string.Empty;
        public decimal TasaCambioDia { get; set; }
        public List<DetallesPagoDto> PagosMultidivisa { get; set; } = new();
    }

    public class RegistrarReciboFacturaCommandHandler : IRequestHandler<RegistrarReciboFacturaCommand, Guid>
    {
        private readonly ICajaAdministrativaRepository _cajaRepository;
        private readonly IBillingRepository _billingRepository;
        private readonly IApplicationDbContext _context;

        public RegistrarReciboFacturaCommandHandler(
            ICajaAdministrativaRepository cajaRepository, 
            IBillingRepository billingRepository,
            IApplicationDbContext context)
        {
            _cajaRepository = cajaRepository;
            _billingRepository = billingRepository;
            _context = context;
        }

        public async Task<Guid> Handle(RegistrarReciboFacturaCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar Caja Abierta (NoTracking para evitar re-inserción fantasmal)
            var cajaAbierta = await _cajaRepository.ObtenerCajaAbiertaNoTrackingAsync(cancellationToken);
            if (cajaAbierta == null) throw new InvalidOperationException("No se pueden registrar pagos si no hay una Caja Abierta.");
            
            // 2. Obtener Cuenta de Servicios
            var cuenta = await _billingRepository.ObtenerCuentaPorIdAsync(request.CuentaServicioId, cancellationToken);
            if (cuenta == null) throw new InvalidOperationException("La cuenta de servicio referenciada no existe.");
            if (cuenta.Estado != EstadoConstants.Abierta) throw new InvalidOperationException("La cuenta ya ha sido procesada.");

            // 3. Crear Recibo (V11.2 Change Support / Vuelto)
            var metodosPagoCatalog = await _context.CatalogoMetodosPago
                .Where(m => m.Activo)
                .ToListAsync(cancellationToken);

            decimal totalCuenta = cuenta.CalcularTotal();
            decimal totalPagado = 0;

            var listaPagosValidados = new List<(DetallesPagoDto Pago, decimal Equivalente, decimal Tasa)>();

            foreach (var p in request.PagosMultidivisa)
            {
                var metodoPagoEntidad = metodosPagoCatalog.FirstOrDefault(m => m.Valor == p.MetodoPago || m.Nombre == p.MetodoPago);
                decimal equivalenteBase = p.EquivalenteAbonadoBase;
                decimal tasaAplicada = 1m;

                if (metodoPagoEntidad != null)
                {
                    if (metodoPagoEntidad.GrupoMoneda == 1)
                    {
                        tasaAplicada = 1m;
                        equivalenteBase = p.MontoAbonadoMoneda;
                    }
                    else if (metodoPagoEntidad.GrupoMoneda == 2)
                    {
                        tasaAplicada = request.TasaCambioDia;
                        if (tasaAplicada <= 0) throw new InvalidOperationException("La tasa de cambio del día debe ser mayor a cero para pagos en Bolívares.");
                        equivalenteBase = Math.Round(p.MontoAbonadoMoneda / tasaAplicada, 2);
                    }
                }
                else
                {
                    // Fallback
                    var lower = p.MetodoPago.ToLower();
                    if (lower.Contains("bs") || lower.Contains("móvil") || lower.Contains("punto"))
                    {
                        tasaAplicada = request.TasaCambioDia;
                        equivalenteBase = tasaAplicada > 0 ? Math.Round(p.MontoAbonadoMoneda / tasaAplicada, 2) : 0;
                    }
                    else
                    {
                        tasaAplicada = 1m;
                        equivalenteBase = p.MontoAbonadoMoneda;
                    }
                }

                totalPagado += equivalenteBase;
                listaPagosValidados.Add((p, equivalenteBase, tasaAplicada));
            }

            decimal montoVueltoUSD = Math.Max(0, totalPagado - totalCuenta);

            var recibo = new ReciboFactura(request.CuentaServicioId, cuenta.PacienteId, cajaAbierta.Id, request.TasaCambioDia, totalCuenta, montoVueltoUSD, EstadoConstants.Borrador);

            foreach (var item in listaPagosValidados)
            {
                recibo.AgregarDetallePago(item.Pago.MetodoPago, item.Pago.ReferenciaBancaria, item.Pago.MontoAbonadoMoneda, item.Equivalente, item.Tasa, request.CajeroUserId);
            }

            // 4. Validar montos y cierre condicional (V11.5 Senior Pattern)
            // decimal totalPagado = recibo.ObtenerTotalPagadoBase(); // Ya disponible arriba
            // decimal totalCuenta = cuenta.CalcularTotal(); // Ya calculado arriba

            if (totalPagado >= totalCuenta)
            {
                // Pago total o excedente: Cerrar Cuenta
                cuenta.Facturar();
            }
            else
            {
                // Pago parcial: Generar Deuda (Cuentas por Cobrar)
                var deuda = new CuentaPorCobrar(cuenta.Id, cuenta.PacienteId, totalCuenta, totalPagado);
                await _context.CuentasPorCobrar.AddAsync(deuda, cancellationToken);
            }

            // 5. Persistencia Atómica
            await _context.RecibosFactura.AddAsync(recibo, cancellationToken);
            await _billingRepository.ActualizarCuentaAsync(cuenta, cancellationToken);
            await _billingRepository.GuardarCambiosAsync(cancellationToken);

            return recibo.Id;
        }
    }
}
