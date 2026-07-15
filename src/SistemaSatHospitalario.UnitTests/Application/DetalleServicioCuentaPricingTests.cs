using System;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class DetalleServicioCuentaPricingTests
    {
        [Fact]
        public void DetalleServicioCuenta_ShouldInitializeWithDefaultPricing()
        {
            // Arrange
            var cuentaId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();

            // Act
            var detalle = new DetalleServicioCuenta(cuentaId, servicioId, "Consulta médica", 150.00m, 50.00m, 1, "MEDICO", "admin");

            // Assert
            Assert.False(detalle.IncluidoEnTarifaBase);
            Assert.Equal(0.00m, detalle.PrecioCatalogoHistorico);
            Assert.Equal(150.00m, detalle.Precio);
            Assert.Equal(150.00m, detalle.ObtenerSubtotal());
        }

        [Fact]
        public void DetalleServicioCuenta_MarcarComoIncluidoEnTarifaBase_ShouldSetPriceToZeroAndSaveHistory()
        {
            // Arrange
            var detalle = new DetalleServicioCuenta(Guid.NewGuid(), Guid.NewGuid(), "Insumo Jeringa", 10.00m, 0.00m, 2, "INSUMO", "admin");

            // Act
            detalle.MarcarComoIncluidoEnTarifaBase();

            // Assert
            Assert.True(detalle.IncluidoEnTarifaBase);
            Assert.Equal(10.00m, detalle.PrecioCatalogoHistorico);
            Assert.Equal(0.00m, detalle.Precio);
            Assert.Equal(0.00m, detalle.ObtenerSubtotal());
        }

        [Fact]
        public void DetalleServicioCuenta_RemoverDeTarifaBase_ShouldRestorePriceFromCatalog()
        {
            // Arrange
            var detalle = new DetalleServicioCuenta(Guid.NewGuid(), Guid.NewGuid(), "Insumo Jeringa", 10.00m, 0.00m, 2, "INSUMO", "admin");
            detalle.MarcarComoIncluidoEnTarifaBase();

            // Act
            detalle.RemoverDeTarifaBase(12.50m); // restaurar con el precio actual del catálogo

            // Assert
            Assert.False(detalle.IncluidoEnTarifaBase);
            Assert.Equal(12.50m, detalle.Precio);
            Assert.Equal(25.00m, detalle.ObtenerSubtotal());
        }
    }
}
