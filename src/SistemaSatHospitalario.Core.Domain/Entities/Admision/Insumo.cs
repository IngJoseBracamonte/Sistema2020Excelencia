using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Insumo
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public virtual System.Collections.Generic.ICollection<StockSede> StocksPorSede { get; private set; } = new System.Collections.Generic.List<StockSede>();
        public decimal StockActual => System.Linq.Enumerable.Sum(StocksPorSede, s => s.StockActual);
        public UnidadMedida UnidadMedidaBase { get; private set; }
        public decimal CostoUnitarioBaseUSD { get; private set; }
        public bool PermiteFraccionamiento { get; private set; }
        public string Categoria { get; private set; }
        public string? ReactivosCombinados { get; private set; }
        public string? Indicaciones { get; private set; }
        public DateTime? FechaVencimiento { get; private set; }
        public bool OcultoEnTraslados { get; private set; }

        protected Insumo() { }

        public Insumo(string codigo, string nombre, decimal stockActual, UnidadMedida unidadMedidaBase, decimal costoUnitarioBaseUSD, bool permiteFraccionamiento = true, string categoria = "Medicamento")
        {
            Id = Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaBase = unidadMedidaBase;
            CostoUnitarioBaseUSD = costoUnitarioBaseUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
            Categoria = categoria;
            OcultoEnTraslados = false;
            
            if (stockActual > 0)
            {
                StocksPorSede.Add(new StockSede(Id, Guid.Empty, stockActual));
            }
        }

        public void RegistrarMovimientoStock(decimal cantidadBase)
        {
            // RegistrarMovimientoStock legacy/fallback (e.g. for default principal sede if no sede specified, or throws)
            // But we will have a new method/overload, or update this to be a no-op / warning
        }

        public void EstablecerStockCierre(decimal stockFisicoReal)
        {
        }

        public void ActualizarDetalles(string nombre, decimal costoUSD)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            CostoUnitarioBaseUSD = costoUSD;
        }

        public void ActualizarDetalles(string nombre, UnidadMedida unidadMedidaBase, decimal costoUSD, bool permiteFraccionamiento, string categoria, string? reactivos, string? indicaciones, DateTime? vencimiento)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaBase = unidadMedidaBase;
            CostoUnitarioBaseUSD = costoUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
            Categoria = categoria;
            ReactivosCombinados = reactivos;
            Indicaciones = indicaciones;
            FechaVencimiento = vencimiento;
        }

        public void AlternarOcultoEnTraslados(bool ocultar)
        {
            OcultoEnTraslados = ocultar;
        }
    }
}
