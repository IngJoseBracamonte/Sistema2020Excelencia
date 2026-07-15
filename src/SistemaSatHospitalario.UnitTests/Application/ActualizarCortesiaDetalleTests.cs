using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Constants;
using MockQueryable.Moq;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ActualizarCortesiaDetalleTests
    {
        [Fact]
        public async Task Handle_ShouldMarkAsCourtesy_WhenIncluidoEnTarifaBaseIsTrue()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<ActualizarCortesiaDetalleCommandHandler>>();

            var detalle = new DetalleServicioCuenta(Guid.NewGuid(), Guid.NewGuid(), "Insumo de prueba", 100.00m, 0m, 2, "Insumo", "admin");

            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(new List<DetalleServicioCuenta> { detalle }.BuildMockDbSet().Object);

            var command = new ActualizarCortesiaDetalleCommand(detalle.Id, true, "admin");
            var handler = new ActualizarCortesiaDetalleCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.True(detalle.IncluidoEnTarifaBase);
            Assert.Equal(0.00m, detalle.Precio);
            Assert.Equal(100.00m, detalle.PrecioCatalogoHistorico);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRestoreOriginalPriceFromCatalog_ToAvoidPriceTampering_WhenIncluidoEnTarifaBaseIsFalse()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<ActualizarCortesiaDetalleCommandHandler>>();

            var service = new ServicioClinico("I01", "Insumo de prueba", 150.00m, "Insumo");
            var detalle = new DetalleServicioCuenta(Guid.NewGuid(), service.Id, "Insumo de prueba", 100.00m, 0m, 2, "Insumo", "admin");
            detalle.MarcarComoIncluidoEnTarifaBase(); // sets price to 0 and catalog to 100

            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(new List<DetalleServicioCuenta> { detalle }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(new List<ServicioClinico> { service }.BuildMockDbSet().Object);

            // User turns off courtesy (incluido = false)
            var command = new ActualizarCortesiaDetalleCommand(detalle.Id, false, "admin");
            var handler = new ActualizarCortesiaDetalleCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.False(detalle.IncluidoEnTarifaBase);
            // It MUST restore the real price from catalog (150.00m) and NOT what the user or detail had historically (100.00m) if they modified it
            Assert.Equal(150.00m, detalle.Precio);
            Assert.Equal(0.00m, detalle.PrecioCatalogoHistorico);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
