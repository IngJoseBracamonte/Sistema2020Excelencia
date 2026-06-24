using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class TransactionsTests
    {
        [Fact]
        public async Task RecordMovement_ShouldAffectStockOnlyInSpecifiedSede()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<InventoryService>>();

            var insumo = new Insumo("INS01", "Gasa Estéril", 0, UnidadMedida.UNIDAD, 0.2m, true, "Descartable");
            var sedeAId = Guid.NewGuid();
            var sedeBId = Guid.NewGuid();

            var stockSedeA = new StockSede(insumo.Id, sedeAId, 10.0m);
            var stockSedeB = new StockSede(insumo.Id, sedeBId, 20.0m);

            insumo.StocksPorSede.Add(stockSedeA);
            insumo.StocksPorSede.Add(stockSedeB);

            var insumosList = new List<Insumo> { insumo }.BuildMockDbSet<Insumo>();
            var stocksList = new List<StockSede> { stockSedeA, stockSedeB }.BuildMockDbSet<StockSede>();
            var movimientosList = new List<MovimientoInsumo>().BuildMockDbSet<MovimientoInsumo>();

            mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksList.Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosList.Object);

            var service = new InventoryService(mockContext.Object, mockLogger.Object);

            // Act: Add 5 units of stock to Sede A
            await service.RecordMovementAsync(
                insumo.Id,
                sedeAId,
                "Ingreso",
                5.0m,
                UnidadMedida.UNIDAD,
                "TestUser",
                "Ingreso rutinario",
                CancellationToken.None
            );

            // Assert: Sede A stock should be 15, Sede B stock should remain 20
            Assert.Equal(15.0m, stockSedeA.StockActual);
            Assert.Equal(20.0m, stockSedeB.StockActual);

            movimientosList.Verify(m => m.Add(It.Is<MovimientoInsumo>(mov =>
                mov.InsumoId == insumo.Id &&
                mov.SedeId == sedeAId &&
                mov.TipoMovimiento == "Ingreso" &&
                mov.CantidadBase == 5.0m)), Times.Once);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PerformClosing_ShouldCreateAdjustmentMovOnlyForSpecifiedSede()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<InventoryService>>();

            var insumo = new Insumo("INS01", "Gasa Estéril", 0, UnidadMedida.UNIDAD, 0.2m, true, "Descartable");
            var sedeAId = Guid.NewGuid();

            var stockSedeA = new StockSede(insumo.Id, sedeAId, 10.0m);
            insumo.StocksPorSede.Add(stockSedeA);

            var insumosList = new List<Insumo> { insumo }.BuildMockDbSet<Insumo>();
            var stocksList = new List<StockSede> { stockSedeA }.BuildMockDbSet<StockSede>();
            var cierresList = new List<CierreInventario>().BuildMockDbSet<CierreInventario>();
            var cierreDetallesList = new List<CierreInventarioDetalle>().BuildMockDbSet<CierreInventarioDetalle>();
            var movimientosList = new List<MovimientoInsumo>().BuildMockDbSet<MovimientoInsumo>();

            mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksList.Object);
            mockContext.Setup(c => c.CierresInventario).Returns(cierresList.Object);
            mockContext.Setup(c => c.CierresInventarioDetalles).Returns(cierreDetallesList.Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosList.Object);

            var service = new InventoryService(mockContext.Object, mockLogger.Object);
            var detallesInput = new List<CierreDetalleInputDto>
            {
                new CierreDetalleInputDto(insumo.Id, 8.0m) // Theoretical is 10, Real physical is 8 (diff is -2)
            };

            // Act
            await service.PerformClosingAsync(
                sedeAId,
                "TestUser",
                "Cierre mensual Sede A",
                detallesInput,
                CancellationToken.None
            );

            // Assert
            Assert.Equal(8.0m, stockSedeA.StockActual);

            cierresList.Verify(c => c.Add(It.Is<CierreInventario>(ci =>
                ci.SedeId == sedeAId &&
                ci.Usuario == "TestUser" &&
                ci.Observaciones == "Cierre mensual Sede A")), Times.Once);

            cierreDetallesList.Verify(d => d.Add(It.Is<CierreInventarioDetalle>(cd =>
                cd.InsumoId == insumo.Id &&
                cd.StockTeoricoBase == 10.0m &&
                cd.StockRealBase == 8.0m)), Times.Once);

            movimientosList.Verify(m => m.Add(It.Is<MovimientoInsumo>(mov =>
                mov.InsumoId == insumo.Id &&
                mov.SedeId == sedeAId &&
                mov.TipoMovimiento == "AjusteCierre" &&
                mov.CantidadBase == -2.0m)), Times.Once);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
