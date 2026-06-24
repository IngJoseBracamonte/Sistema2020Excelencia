using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class StockSede
    {
        public Guid Id { get; private set; }
        public Guid InsumoId { get; private set; }
        public virtual Insumo Insumo { get; private set; }
        public Guid SedeId { get; private set; }
        public virtual Sede Sede { get; private set; }
        public decimal StockActual { get; private set; }
        public decimal? StockMinimo { get; private set; }
        public decimal? StockMaximo { get; private set; }

        private StockSede() { }

        public StockSede(Guid insumoId, Guid sedeId, decimal stockActual, decimal? stockMinimo = null, decimal? stockMaximo = null)
        {
            Id = Guid.NewGuid();
            InsumoId = insumoId;
            SedeId = sedeId;
            StockActual = stockActual;
            StockMinimo = stockMinimo;
            StockMaximo = stockMaximo;
        }

        public void RegistrarMovimientoStock(decimal cantidadBase, bool permiteFraccionamiento)
        {
            if (!permiteFraccionamiento)
            {
                if (cantidadBase < 0)
                {
                    // Redondear hacia arriba en magnitud (Ceiling del valor absoluto, es decir, Floor/Ceiling dependiendo del signo)
                    // Ej: consumo de -1.2 unidades debe resultar en un decremento de -2 unidades.
                    // Math.Floor(-1.2) = -2.0
                    cantidadBase = Math.Floor(cantidadBase);
                }
                else if (cantidadBase > 0)
                {
                    // Para ingresos positivos no fraccionables, redondeamos al más cercano
                    cantidadBase = Math.Round(cantidadBase, MidpointRounding.AwayFromZero);
                }
            }

            StockActual = StockActual + cantidadBase;
        }

        public void EstablecerStockCierre(decimal stockFisicoReal)
        {
            StockActual = stockFisicoReal;
        }

        public void UpdateLimites(decimal? stockMinimo, decimal? stockMaximo)
        {
            StockMinimo = stockMinimo;
            StockMaximo = stockMaximo;
        }
    }
}
