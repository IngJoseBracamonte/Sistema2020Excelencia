using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetKardexQueryTests
    {
        [Fact]
        public async Task Handle_ShouldCalculateBalancesAndEntradasSalidas_Correctly()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var insumo = new Insumo("INS-001", "Paracetamol 500mg", 100, UnidadMedida.UNIDAD, 1.5m);
            var sede = new Sede("ALM", "Almacén Central", true);

            var baseDate = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);

            var mov1 = new MovimientoInsumo(insumo.Id, sede.Id, "Ingreso", 50, UnidadMedida.UNIDAD, 50, "Admin", "Stock inicial compra");
            typeof(MovimientoInsumo).GetProperty("Fecha")?.SetValue(mov1, baseDate);
            typeof(MovimientoInsumo).GetProperty("Insumo")?.SetValue(mov1, insumo);
            typeof(MovimientoInsumo).GetProperty("Sede")?.SetValue(mov1, sede);

            var mov2 = new MovimientoInsumo(insumo.Id, sede.Id, "Despacho", 15, UnidadMedida.UNIDAD, 15, "Enfermera", "Despacho a Emergencia");
            typeof(MovimientoInsumo).GetProperty("Fecha")?.SetValue(mov2, baseDate.AddHours(2));
            typeof(MovimientoInsumo).GetProperty("Insumo")?.SetValue(mov2, insumo);
            typeof(MovimientoInsumo).GetProperty("Sede")?.SetValue(mov2, sede);

            var mov3 = new MovimientoInsumo(insumo.Id, sede.Id, "Descarte", 5, UnidadMedida.UNIDAD, 5, "Admin", "Insumo dañado");
            typeof(MovimientoInsumo).GetProperty("Fecha")?.SetValue(mov3, baseDate.AddHours(4));
            typeof(MovimientoInsumo).GetProperty("Insumo")?.SetValue(mov3, insumo);
            typeof(MovimientoInsumo).GetProperty("Sede")?.SetValue(mov3, sede);

            var listMovs = new List<MovimientoInsumo> { mov1, mov2, mov3 };
            var mockSetMovs = listMovs.BuildMockDbSet();
            mockContext.Setup(c => c.MovimientosInsumo).Returns(mockSetMovs.Object);

            var query = new GetKardexQuery
            {
                InsumoId = insumo.Id,
                SedeId = sede.Id
            };

            var handler = new GetKardexQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.InitialBalance);
            Assert.Equal(50, result.TotalEntradas);
            Assert.Equal(20, result.TotalSalidas);
            Assert.Equal(30, result.FinalBalance);
            Assert.Equal(3, result.Movimientos.Count);
        }

        [Fact]
        public async Task Handle_WithFechaDesde_ShouldCalculateInitialBalanceFromPriorMovements()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var insumo = new Insumo("INS-002", "Ibuprofeno 400mg", 50, UnidadMedida.UNIDAD, 2.0m);
            var sede = new Sede("EMG", "Sede Emergencia", false);

            var priorDate = new DateTime(2026, 7, 25, 10, 0, 0, DateTimeKind.Utc);
            var currentDate = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);

            // Movimiento previo (debe incluirse en initialBalance)
            var movPrev = new MovimientoInsumo(insumo.Id, sede.Id, "Ingreso", 40, UnidadMedida.UNIDAD, 40, "Admin", "Ingreso previo");
            typeof(MovimientoInsumo).GetProperty("Fecha")?.SetValue(movPrev, priorDate);
            typeof(MovimientoInsumo).GetProperty("Insumo")?.SetValue(movPrev, insumo);
            typeof(MovimientoInsumo).GetProperty("Sede")?.SetValue(movPrev, sede);

            // Movimiento dentro del rango
            var movCurr = new MovimientoInsumo(insumo.Id, sede.Id, "Consumo", 10, UnidadMedida.UNIDAD, 10, "Enfermero", "Consumo en guardia");
            typeof(MovimientoInsumo).GetProperty("Fecha")?.SetValue(movCurr, currentDate);
            typeof(MovimientoInsumo).GetProperty("Insumo")?.SetValue(movCurr, insumo);
            typeof(MovimientoInsumo).GetProperty("Sede")?.SetValue(movCurr, sede);

            var listMovs = new List<MovimientoInsumo> { movPrev, movCurr };
            var mockSetMovs = listMovs.BuildMockDbSet();
            mockContext.Setup(c => c.MovimientosInsumo).Returns(mockSetMovs.Object);

            var query = new GetKardexQuery
            {
                InsumoId = insumo.Id,
                FechaDesde = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var handler = new GetKardexQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(40, result.InitialBalance);
            Assert.Equal(0, result.TotalEntradas);
            Assert.Equal(10, result.TotalSalidas);
            Assert.Equal(30, result.FinalBalance);
            Assert.Single(result.Movimientos);
        }

        [Fact]
        public async Task Handle_WhenNoMovementsExist_ShouldReturnNonNullEmptyKardexResult()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var insumo = new Insumo("INS-003", "Suero Fisiológico", 25, UnidadMedida.UNIDAD, 3.0m);
            var listInsumos = new List<Insumo> { insumo };
            var mockSetInsumos = listInsumos.BuildMockDbSet();
            mockContext.Setup(c => c.Insumos).Returns(mockSetInsumos.Object);

            var listMovs = new List<MovimientoInsumo>();
            var mockSetMovs = listMovs.BuildMockDbSet();
            mockContext.Setup(c => c.MovimientosInsumo).Returns(mockSetMovs.Object);

            var listStocks = new List<StockSede>();
            var mockSetStocks = listStocks.BuildMockDbSet();
            mockContext.Setup(c => c.StocksSedes).Returns(mockSetStocks.Object);

            var query = new GetKardexQuery
            {
                InsumoId = insumo.Id
            };

            var handler = new GetKardexQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Movimientos);
            Assert.Empty(result.Movimientos);
            Assert.Equal(25, result.InitialBalance);
            Assert.Equal(0, result.TotalEntradas);
            Assert.Equal(0, result.TotalSalidas);
            Assert.Equal(25, result.FinalBalance);
        }
    }
}
