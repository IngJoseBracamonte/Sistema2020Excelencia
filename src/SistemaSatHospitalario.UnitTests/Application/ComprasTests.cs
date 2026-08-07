using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ComprasTests
    {
        [Fact]
        public void Insumo_ShouldInitializeNewFieldsAsDefault()
        {
            // Arrange & Act
            var insumo = new Insumo("INS-001", "Paracetamol 500mg", 100, UnidadMedida.UNIDAD, 1.50m);

            // Assert
            Assert.Null(insumo.ReactivosCombinados);
            Assert.Null(insumo.Indicaciones);
            Assert.Null(insumo.FechaVencimiento);
            Assert.False(insumo.OcultoEnTraslados);
        }

        [Fact]
        public void Insumo_ShouldAllowUpdatingNewFields()
        {
            // Arrange
            var insumo = new Insumo("INS-001", "Paracetamol 500mg", 100, UnidadMedida.UNIDAD, 1.50m);
            var vencimiento = DateTime.UtcNow.AddYears(2);

            // Act
            insumo.ActualizarDetalles(
                "Paracetamol Forte",
                UnidadMedida.UNIDAD,
                2.00m,
                true,
                "Medicamento",
                "Acetaminofen",
                "Tomar cada 8 horas",
                vencimiento
            );

            // Assert
            Assert.Equal("Paracetamol Forte", insumo.Nombre);
            Assert.Equal(2.00m, insumo.CostoUnitarioBaseUSD);
            Assert.Equal("Acetaminofen", insumo.ReactivosCombinados);
            Assert.Equal("Tomar cada 8 horas", insumo.Indicaciones);
            Assert.Equal(vencimiento, insumo.FechaVencimiento);
        }

        [Fact]
        public void Insumo_ShouldSoftDeleteAndRestoreCorrectly()
        {
            // Arrange
            var insumo = new Insumo("INS-001", "Paracetamol 500mg", 100, UnidadMedida.UNIDAD, 1.50m);

            // Act - Soft Delete
            insumo.AlternarOcultoEnTraslados(true);

            // Assert
            Assert.True(insumo.OcultoEnTraslados);

            // Act - Restore
            insumo.AlternarOcultoEnTraslados(false);

            // Assert
            Assert.False(insumo.OcultoEnTraslados);
        }

        [Fact]
        public void CrearInsumo_StockInicialCero_EsPermitidoSinRequiereIngresoCentral()
        {
            // Arrange & Act: Se registra medicamento en catálogo con stock 0 (no requiere compra inicial obligatoria)
            var medicamento = new Insumo("MED-CAT-01", "Omeprazol 20mg", 0m, UnidadMedida.UNIDAD, 0.80m, true, "Medicamento");

            // Assert
            Assert.Equal(0m, medicamento.StockActual);
            Assert.Single(medicamento.StocksPorSede);
            Assert.Equal(0m, medicamento.StocksPorSede.First().StockActual);
        }

        [Fact]
        public void Compra_StockSedeRegistrarMovimiento_ConCantidadPositiva_IncrementaStock()
        {
            // Arrange
            var insumo = new Insumo("MED-CAT-02", "Losartán 50mg", 0m, UnidadMedida.UNIDAD, 1.20m, true, "Medicamento");
            var stockSede = insumo.StocksPorSede.First();

            // Act: Se registra una compra de 50 unidades
            stockSede.RegistrarMovimientoStock(50m, insumo.PermiteFraccionamiento);

            // Assert
            Assert.Equal(50m, stockSede.StockActual);
            Assert.Equal(50m, insumo.StockActual);
        }

        [Fact]
        public void Compra_CalculoCostosTotalYUnitario_ActualizaCostoUnitarioBaseEIncrementaStock()
        {
            // Arrange: 100 tabletas compradas en 5 cajas a $5 c/u = $25.00 total. Costo Unitario = $25 / 100 = $0.25
            var insumo = new Insumo("MED-NEO-PRED-5", "Neolpharma Prednisona 5mg", 0m, UnidadMedida.UNIDAD, 0m, true, "Medicamento");
            var cantidadUnidades = 100m;
            var costoTotalCompraUSD = 25.00m;
            var costoUnitarioDerivado = costoTotalCompraUSD / cantidadUnidades;

            // Act: Actualizar detalles de costo e incrementar stock
            insumo.ActualizarDetalles(insumo.Nombre, insumo.UnidadMedidaBase, costoUnitarioDerivado, insumo.PermiteFraccionamiento, insumo.Categoria);
            var stockSede = insumo.StocksPorSede.First();
            stockSede.RegistrarMovimientoStock(cantidadUnidades, insumo.PermiteFraccionamiento);

            // Assert
            Assert.Equal(0.25m, insumo.CostoUnitarioBaseUSD);
            Assert.Equal(100m, insumo.StockActual);
        }
    }
}

