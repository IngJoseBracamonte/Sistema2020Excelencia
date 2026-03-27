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

            // 3. Crear Recibo
            var recibo = new ReciboFactura(request.CuentaServicioId, cuenta.PacienteId, cajaAbierta.Id, request.TasaCambioDia, EstadoConstants.Borrador);

            foreach (var pago in request.PagosMultidivisa)
            {
                recibo.AgregarDetallePago(pago.MetodoPago, pago.ReferenciaBancaria, pago.MontoAbonadoMoneda, pago.EquivalenteAbonadoBase);
            }

            // 4. Validar montos y cierre condicional (V11.5 Senior Pattern)
            decimal totalPagado = recibo.ObtenerTotalPagadoBase();
            decimal totalCuenta = cuenta.CalcularTotal();

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
