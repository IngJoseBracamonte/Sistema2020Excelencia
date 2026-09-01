using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CajaDeclaracionMetodo
    {
        public Guid Id { get; private set; }
        public Guid CajaDiariaId { get; private set; }
        public Guid MetodoPagoId { get; private set; }
        public decimal MontoIngresado { get; private set; }
        public decimal MontoVueltos { get; private set; }
        public decimal MontoEsperadoIngreso { get; private set; }
        public decimal MontoEsperadoVueltos { get; private set; }
        public decimal DiferenciaOriginal { get; private set; }
        public decimal DiferenciaBase { get; private set; }

        public virtual CajaDiaria CajaDiaria { get; private set; } = null!;
        public virtual CatalogoMetodoPago MetodoPago { get; private set; } = null!;

        private CajaDeclaracionMetodo() { }

        public CajaDeclaracionMetodo(
            Guid cajaDiariaId,
            Guid metodoPagoId,
            decimal montoIngresado,
            decimal montoVueltos,
            decimal montoEsperadoIngreso,
            decimal montoEsperadoVueltos,
            decimal diferenciaOriginal,
            decimal diferenciaBase)
        {
            if (cajaDiariaId == Guid.Empty) throw new ArgumentException("La caja es requerida.", nameof(cajaDiariaId));
            if (metodoPagoId == Guid.Empty) throw new ArgumentException("El método de pago es requerido.", nameof(metodoPagoId));

            Id = Guid.NewGuid();
            CajaDiariaId = cajaDiariaId;
            MetodoPagoId = metodoPagoId;
            MontoIngresado = montoIngresado;
            MontoVueltos = montoVueltos;
            MontoEsperadoIngreso = montoEsperadoIngreso;
            MontoEsperadoVueltos = montoEsperadoVueltos;
            DiferenciaOriginal = diferenciaOriginal;
            DiferenciaBase = diferenciaBase;
        }
    }
}