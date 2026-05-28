using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class GarantiaItem
    {
        public Guid Id { get; private set; }
        public Guid CuentaPorCobrarId { get; private set; }
        public string Descripcion { get; private set; }
        public decimal ValorEstimado { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        public CuentaPorCobrar CuentaPorCobrar { get; private set; }

        protected GarantiaItem() { }

        public GarantiaItem(Guid cuentaPorCobrarId, string descripcion, decimal valorEstimado)
        {
            Id = Guid.NewGuid();
            CuentaPorCobrarId = cuentaPorCobrarId;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            ValorEstimado = valorEstimado;
            FechaRegistro = DateTime.UtcNow;
        }
    }
}
