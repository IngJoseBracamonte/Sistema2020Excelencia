using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CierreInventarioDetalle
    {
        public Guid Id { get; private set; }
        public Guid CierreInventarioId { get; private set; }
        public Guid InsumoId { get; private set; }
        public decimal StockTeoricoBase { get; private set; }
        public decimal StockRealBase { get; private set; }
        public decimal CostoBaseUSD { get; private set; }

        public virtual CierreInventario CierreInventario { get; private set; }
        public virtual Insumo Insumo { get; private set; }

        protected CierreInventarioDetalle() { }

        public CierreInventarioDetalle(Guid cierreInventarioId, Guid insumoId, decimal stockTeoricoBase, decimal stockRealBase, decimal costoBaseUSD)
        {
            Id = Guid.NewGuid();
            CierreInventarioId = cierreInventarioId;
            InsumoId = insumoId;
            StockTeoricoBase = stockTeoricoBase;
            StockRealBase = stockRealBase;
            CostoBaseUSD = costoBaseUSD;
        }
    }
}
