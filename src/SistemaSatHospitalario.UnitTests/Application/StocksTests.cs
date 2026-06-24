using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class StocksTests
    {
        [Fact]
        public void InsumoStockActual_ShouldBeSumOfAllStocksPorSede()
        {
            // Arrange
            var insumo = new Insumo("INS01", "Paracetamol 500mg", 0, UnidadMedida.UNIDAD, 1.5m, true, "Medicamento");
            var sede1Id = Guid.NewGuid();
            var sede2Id = Guid.NewGuid();

            var stockSede1 = new StockSede(insumo.Id, sede1Id, 10.5m);
            var stockSede2 = new StockSede(insumo.Id, sede2Id, 5.0m);

            insumo.StocksPorSede.Add(stockSede1);
            insumo.StocksPorSede.Add(stockSede2);

            // Act
            var totalStock = insumo.StockActual;

            // Assert
            Assert.Equal(15.5m, totalStock);
        }

        [Fact]
        public void RegistrarMovimientoStock_ShouldRoundValue_WhenPermiteFraccionamientoIsFalse()
        {
            // Arrange
            var insumoNoFraccionable = new Insumo("INS02", "Jeringa 5ml", 0, UnidadMedida.UNIDAD, 0.5m, false, "Descartable");
            var sedeId = Guid.NewGuid();
            var stockSede = new StockSede(insumoNoFraccionable.Id, sedeId, 10.0m);

            // Act: registrar un consumo de -1.2 (no fraccionable, debe redondear hacia arriba en magnitud, es decir, -2 unidades)
            stockSede.RegistrarMovimientoStock(-1.2m, insumoNoFraccionable.PermiteFraccionamiento);

            // Assert: 10.0 - 2.0 = 8.0m
            Assert.Equal(8.0m, stockSede.StockActual);

            // Act: registrar un ingreso positivo de 1.8m (no fraccionable, debe redondear al más cercano: 2 unidades)
            stockSede.RegistrarMovimientoStock(1.8m, insumoNoFraccionable.PermiteFraccionamiento);

            // Assert: 8.0 + 2.0 = 10.0m
            Assert.Equal(10.0m, stockSede.StockActual);
        }

        [Fact]
        public void RegistrarMovimientoStock_ShouldKeepDecimal_WhenPermiteFraccionamientoIsTrue()
        {
            // Arrange
            var insumoFraccionable = new Insumo("INS03", "Solución Fisiológica 1L", 0, UnidadMedida.L, 2.0m, true, "Medicamento");
            var sedeId = Guid.NewGuid();
            var stockSede = new StockSede(insumoFraccionable.Id, sedeId, 10.0m);

            // Act: registrar consumo de -0.25 (fraccionable, debe descontar exactamente -0.25 unidades)
            stockSede.RegistrarMovimientoStock(-0.25m, insumoFraccionable.PermiteFraccionamiento);

            // Assert: 10.0 - 0.25 = 9.75m
            Assert.Equal(9.75m, stockSede.StockActual);
        }
    }
}
