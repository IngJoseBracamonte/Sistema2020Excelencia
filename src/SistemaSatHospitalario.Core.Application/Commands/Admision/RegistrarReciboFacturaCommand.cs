using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

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

        public RegistrarReciboFacturaCommandHandler(ICajaAdministrativaRepository cajaRepository, IBillingRepository billingRepository)
        {
            _cajaRepository = cajaRepository;
            _billingRepository = billingRepository;
        }

        public async Task<Guid> Handle(RegistrarReciboFacturaCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar Caja Abierta
            var cajaAbierta = await _cajaRepository.ObtenerCajaAbiertaAsync(cancellationToken);
            if (cajaAbierta == null) throw new InvalidOperationException("No se pueden registrar pagos si no hay una Caja Abierta.");
            
            // 2. Obtener Cuenta de Servicios
            var cuenta = await _billingRepository.ObtenerCuentaPorIdAsync(request.CuentaServicioId, cancellationToken);
            if (cuenta == null) throw new InvalidOperationException("La cuenta de servicio referenciada no existe.");
            if (cuenta.Estado != "Abierta") throw new InvalidOperationException("La cuenta ya ha sido procesada.");

            // 3. Crear Recibo
            var recibo = new ReciboFactura(request.CuentaServicioId, cajaAbierta.Id, request.TasaCambioDia, "Borrador");

            foreach (var pago in request.PagosMultidivisa)
            {
                recibo.AgregarDetallePago(pago.MetodoPago, pago.ReferenciaBancaria, pago.MontoAbonadoMoneda, pago.EquivalenteAbonadoBase);
            }

            // 4. Validar montos y cerrar cuenta
            decimal totalPagado = recibo.ObtenerTotalPagadoBase();
            decimal totalCuenta = cuenta.CalcularTotal();

            // Nota: En un sistema real se permite pago parcial, aqui simplificamos a cierre total si el monto cuadra
            // o segun requerimiento del usuario "anexar los pagos y todos hacen el mismo proceso"
            if (totalPagado < totalCuenta)
            {
                // Manejar deuda persistente o pago parcial segun sea el caso
            }

            cuenta.Facturar(); // Cierra la cuenta

            await _billingRepository.ActualizarCuentaAsync(cuenta, cancellationToken);
            // El recibo se guardaria a traves del DbContext o un Repositorio específico si existiera uno solo de pagos.
            // Para simplificar, asumimos que el UnitOfWork (DbContext) maneja el rastro.
            
            await _billingRepository.GuardarCambiosAsync(cancellationToken);

            return recibo.Id;
        }
    }
}
