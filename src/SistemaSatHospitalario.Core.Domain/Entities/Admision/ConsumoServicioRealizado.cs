using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ConsumoServicioRealizado
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioCuentaId { get; private set; }
        public Guid InsumoId { get; private set; }
        public decimal CantidadConsumidaBase { get; private set; }
        public decimal CostoTotalUSD { get; private set; }
        public DateTime FechaConsumo { get; private set; }

        public virtual DetalleServicioCuenta DetalleServicioCuenta { get; private set; }
        public virtual Insumo Insumo { get; private set; }

        protected ConsumoServicioRealizado() { }

        public ConsumoServicioRealizado(Guid detalleServicioCuentaId, Guid insumoId, decimal cantidadConsumidaBase, decimal costoTotalUSD)
        {
            Id = Guid.NewGuid();
            DetalleServicioCuentaId = detalleServicioCuentaId;
            InsumoId = insumoId;
            CantidadConsumidaBase = cantidadConsumidaBase;
            CostoTotalUSD = costoTotalUSD;
            FechaConsumo = DateTime.UtcNow;
        }
    }
}
