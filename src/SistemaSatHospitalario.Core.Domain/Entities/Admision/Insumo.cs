using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Insumo
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public decimal StockActual { get; private set; }
        public UnidadMedida UnidadMedidaBase { get; private set; }
        public decimal CostoUnitarioBaseUSD { get; private set; }

        protected Insumo() { }

        public Insumo(string codigo, string nombre, decimal stockActual, UnidadMedida unidadMedidaBase, decimal costoUnitarioBaseUSD)
        {
            Id = Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            StockActual = stockActual;
            UnidadMedidaBase = unidadMedidaBase;
            CostoUnitarioBaseUSD = costoUnitarioBaseUSD;
        }

        public void RegistrarMovimientoStock(decimal cantidadBase)
        {
            StockActual += cantidadBase;
        }

        public void EstablecerStockCierre(decimal stockFisicoReal)
        {
            StockActual = stockFisicoReal;
        }

        public void ActualizarDetalles(string nombre, decimal costoUSD)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            CostoUnitarioBaseUSD = costoUSD;
        }
    }
}
