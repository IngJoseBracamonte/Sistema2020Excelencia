using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Constants;
using MockQueryable.Moq;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class StockSedeConcurrencyTests
    {
        [Fact]
        public void StockSede_ShouldHaveRowVersionProperty()
        {
            // Arrange & Act
            var stockSede = new StockSede(Guid.NewGuid(), Guid.NewGuid(), 10.00m);

            // Assert
            Assert.Null(stockSede.RowVersion); // initially null before DB persistence
        }

        [Fact]
        public async Task DeductInventory_ShouldRetryOnConcurrencyException()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<InventoryService>>();

            var insumo = new Insumo("I01", "Insumo de prueba", 10.00m, UnidadMedida.G, 5.00m, true, "Medicamento");
            var service = new ServicioClinico("S01", "Cirugía Prueba", 100.00m, "MEDICO");
            service.RequiereInventario = true;

            var recipe = new ServicioInsumoReceta(service.Id, service.Codigo, insumo.Id, 2.00m, UnidadMedida.G);
            recipe.Insumo = insumo;

            var stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Principal, 50.00m);
            var stocksList = new List<StockSede> { stockSede };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            mockContext.Setup(c => c.ServiciosClinicos).Returns(new List<ServicioClinico> { service }.BuildMockDbSet<ServicioClinico>().Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(new List<ServicioInsumoReceta> { recipe }.BuildMockDbSet<ServicioInsumoReceta>().Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.ConsumosServiciosRealizados).Returns(new List<ConsumoServicioRealizado>().BuildMockDbSet<ConsumoServicioRealizado>().Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(new List<MovimientoInsumo>().BuildMockDbSet<MovimientoInsumo>().Object);

            var dbTransactionMock = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            mockContext.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(dbTransactionMock.Object);

            // Throw DbUpdateConcurrencyException twice, then succeed
            var callCount = 0;
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns(() =>
                       {
                           callCount++;
                           if (callCount <= 2)
                           {
                               throw new DbUpdateConcurrencyException("Concurrency conflict simulated", new List<Microsoft.EntityFrameworkCore.Update.IUpdateEntry>());
                           }
                           return Task.FromResult(1);
                       });

            var inventoryService = new InventoryService(mockContext.Object, mockLogger.Object);

            // Act
            await inventoryService.DeductInventoryForServiceDetailAsync(
                Guid.NewGuid(),
                service.Id,
                service.Codigo,
                "Cirugía Prueba",
                1,
                "admin",
                Guid.NewGuid(),
                stockSede.SedeId,
                CancellationToken.None
            );

            // Assert
            Assert.Equal(3, callCount); // 2 retries + 1 successful save
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
            dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeductInventory_ShouldNotRetry_OnNonConcurrencyException()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<InventoryService>>();

            var insumo = new Insumo("I01", "Insumo de prueba", 10.00m, UnidadMedida.G, 5.00m, true, "Medicamento");
            var service = new ServicioClinico("S01", "Cirugía Prueba", 100.00m, "MEDICO");
            service.RequiereInventario = true;

            var recipe = new ServicioInsumoReceta(service.Id, service.Codigo, insumo.Id, 2.00m, UnidadMedida.G);
            recipe.Insumo = insumo;

            var stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Principal, 50.00m);
            var stocksList = new List<StockSede> { stockSede };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            mockContext.Setup(c => c.ServiciosClinicos).Returns(new List<ServicioClinico> { service }.BuildMockDbSet<ServicioClinico>().Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(new List<ServicioInsumoReceta> { recipe }.BuildMockDbSet<ServicioInsumoReceta>().Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.ConsumosServiciosRealizados).Returns(new List<ConsumoServicioRealizado>().BuildMockDbSet<ConsumoServicioRealizado>().Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(new List<MovimientoInsumo>().BuildMockDbSet<MovimientoInsumo>().Object);

            var dbTransactionMock = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            mockContext.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(dbTransactionMock.Object);

            // Mock SaveChangesAsync to throw general InvalidOperationException (non-concurrency)
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Non-concurrency database constraint conflict"));

            var inventoryService = new InventoryService(mockContext.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                inventoryService.DeductInventoryForServiceDetailAsync(
                    Guid.NewGuid(),
                    service.Id,
                    service.Codigo,
                    "Cirugía Prueba",
                    1,
                    "admin",
                    Guid.NewGuid(),
                    stockSede.SedeId,
                    CancellationToken.None
                ));

            // Verify SaveChangesAsync was called exactly once and transaction rollback happened
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
