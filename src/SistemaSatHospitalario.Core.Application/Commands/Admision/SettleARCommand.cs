using System;
using System.Collections.Generic;
using MediatR;


namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SettleARCommand : IRequest<bool>
    {
        public Guid ArId { get; set; }
        public List<PaymentItem> Payments { get; set; } = new List<PaymentItem>();
        public string Observaciones { get; set; }
    }

    public class PaymentItem
    {
        public string Method { get; set; }
        public decimal Amount { get; set; } // Monto base en USD ($)
        public decimal AmountMoneda { get; set; } // Monto en moneda original ($ o Bs.)
        public decimal TasaAplicada { get; set; } // Tasa de cambio usada al registrar
        public string Reference { get; set; }
    }
}
