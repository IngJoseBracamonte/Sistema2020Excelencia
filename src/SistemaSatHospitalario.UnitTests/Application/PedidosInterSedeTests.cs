using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class PedidosInterSedeTests
    {
        [Fact]
        public void PedidoInterSede_ShouldThrowException_IfSedeSolicitanteIsSameAsSedeProveedora()
        {
            // Arrange
            var sedeId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new PedidoInterSede("PED-2026-0001", sedeId, sedeId, "UsuarioPrueba", "Misma sede")
            );
            Assert.Contains("La sede solicitante no puede ser la misma que la sede proveedora", ex.Message);
        }

        [Fact]
        public async Task CreatePedidoInterSedeCommandHandler_ShouldAssignUniqueCorrelativo()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var pedidosList = new List<PedidoInterSede>();
            var pedidosMock = pedidosList.BuildMockDbSet<PedidoInterSede>();

            mockContext.Setup(c => c.PedidosInterSede).Returns(pedidosMock.Object);

            var handler = new CreatePedidoInterSedeCommandHandler(mockContext.Object);
            var command = new CreatePedidoInterSedeCommand
            {
                Usuario = "TestUser",
                Dto = new CreatePedidoInterSedeDto
                {
                    SedeSolicitanteId = Guid.NewGuid(),
                    SedeProveedoraId = Guid.NewGuid(),
                    Observaciones = "Pedido urgente",
                    Lineas = new List<PedidoInterSedeLineaDto>
                    {
                        new PedidoInterSedeLineaDto { InsumoId = Guid.NewGuid(), CantidadSolicitada = 10 }
                    }
                }
            };

            // Act
            var resultId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, resultId);
            mockContext.Verify(c => c.PedidosInterSede.Add(It.Is<PedidoInterSede>(p => p.Correlativo.StartsWith($"PED-{DateTime.UtcNow.Year}-"))), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DispatchPedido_ShouldDeductStockAndRegisterOutboundMovement()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<SistemaSatHospitalario.Core.Application.Common.Services.InventoryService>>();
            
            var insumo = new Insumo("I01", "Insumo 1", 100, UnidadMedida.UNIDAD, 1.50m);
            var sedeSolicitante = new Sede("S01", "Sede Solicitante", false);
            var sedeProveedora = new Sede("S02", "Sede Proveedora", true);

            var stockProveedora = new StockSede(insumo.Id, sedeProveedora.Id, 50);
            insumo.StocksPorSede.Add(stockProveedora);

            var pedido = new PedidoInterSede("PED-2026-0001", sedeSolicitante.Id, sedeProveedora.Id, "Admin", "Requerido");
            var detalle = new PedidoInterSedeDetalle(insumo, 20);
            pedido.AgregarDetalle(detalle);

            var stocksList = new List<StockSede> { stockProveedora };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            var pedidosList = new List<PedidoInterSede> { pedido };
            var pedidosMock = pedidosList.BuildMockDbSet<PedidoInterSede>();

            var movimientosList = new List<MovimientoInsumo>();
            var movimientosMock = movimientosList.BuildMockDbSet<MovimientoInsumo>();

            var insumosList = new List<Insumo> { insumo };
            var insumosMock = insumosList.BuildMockDbSet<Insumo>();

            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.PedidosInterSede).Returns(pedidosMock.Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosMock.Object);
            mockContext.Setup(c => c.Insumos).Returns(insumosMock.Object);

            var inventoryService = new SistemaSatHospitalario.Core.Application.Common.Services.InventoryService(mockContext.Object, loggerMock.Object);

            // Act
            await inventoryService.DispatchPedidoAsync(pedido.Id, "Admin", CancellationToken.None);

            // Assert
            Assert.Equal(30, stockProveedora.StockActual); // 50 - 20
            Assert.Equal(EstadoPedidoInterSede.Despachado, pedido.Estado);
            Assert.Equal(20, detalle.CantidadDespachada);
            mockContext.Verify(c => c.MovimientosInsumo.Add(It.Is<MovimientoInsumo>(m => m.TipoMovimiento == "TransferenciaSalida" && m.CantidadBase == -20)), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DispatchPedido_ShouldThrowException_IfStockIsInsufficient()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<SistemaSatHospitalario.Core.Application.Common.Services.InventoryService>>();
            
            var insumo = new Insumo("I01", "Insumo 1", 100, UnidadMedida.UNIDAD, 1.50m);
            var sedeSolicitante = new Sede("S01", "Sede Solicitante", false);
            var sedeProveedora = new Sede("S02", "Sede Proveedora", true);

            var stockProveedora = new StockSede(insumo.Id, sedeProveedora.Id, 10); // Solo 10 unidades
            insumo.StocksPorSede.Add(stockProveedora);

            var pedido = new PedidoInterSede("PED-2026-0001", sedeSolicitante.Id, sedeProveedora.Id, "Admin", "Requerido");
            var detalle = new PedidoInterSedeDetalle(insumo, 20); // Solicita 20 unidades
            pedido.AgregarDetalle(detalle);

            var stocksList = new List<StockSede> { stockProveedora };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            var pedidosList = new List<PedidoInterSede> { pedido };
            var pedidosMock = pedidosList.BuildMockDbSet<PedidoInterSede>();

            var insumosList = new List<Insumo> { insumo };
            var insumosMock = insumosList.BuildMockDbSet<Insumo>();

            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.PedidosInterSede).Returns(pedidosMock.Object);
            mockContext.Setup(c => c.Insumos).Returns(insumosMock.Object);

            var inventoryService = new SistemaSatHospitalario.Core.Application.Common.Services.InventoryService(mockContext.Object, loggerMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                inventoryService.DispatchPedidoAsync(pedido.Id, "Admin", CancellationToken.None)
            );
            Assert.Contains("Stock insuficiente", ex.Message);
        }

        [Fact]
        public async Task ReceivePedido_ShouldIncreaseStockAndRegisterInboundMovementWithDiscrepancy()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<SistemaSatHospitalario.Core.Application.Common.Services.InventoryService>>();
            
            var insumo = new Insumo("I01", "Insumo 1", 100, UnidadMedida.UNIDAD, 1.50m);
            var sedeSolicitante = new Sede("S01", "Sede Solicitante", false);
            var sedeProveedora = new Sede("S02", "Sede Proveedora", true);

            var stockSolicitante = new StockSede(insumo.Id, sedeSolicitante.Id, 5);
            insumo.StocksPorSede.Add(stockSolicitante);

            var pedido = new PedidoInterSede("PED-2026-0001", sedeSolicitante.Id, sedeProveedora.Id, "Admin", "Requerido");
            pedido.CambiarEstado(EstadoPedidoInterSede.Solicitado);
            var detalle = new PedidoInterSedeDetalle(insumo, 20);
            detalle.SetDespachado(20);
            pedido.AgregarDetalle(detalle);
            pedido.CambiarEstado(EstadoPedidoInterSede.Despachado);

            var stocksList = new List<StockSede> { stockSolicitante };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            var pedidosList = new List<PedidoInterSede> { pedido };
            var pedidosMock = pedidosList.BuildMockDbSet<PedidoInterSede>();

            var movimientosList = new List<MovimientoInsumo>();
            var movimientosMock = movimientosList.BuildMockDbSet<MovimientoInsumo>();

            var insumosList = new List<Insumo> { insumo };
            var insumosMock = insumosList.BuildMockDbSet<Insumo>();

            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.PedidosInterSede).Returns(pedidosMock.Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosMock.Object);
            mockContext.Setup(c => c.Insumos).Returns(insumosMock.Object);

            var inventoryService = new SistemaSatHospitalario.Core.Application.Common.Services.InventoryService(mockContext.Object, loggerMock.Object);

            // Reportamos una discrepancia: de 20 despachados solo recibimos 18
            var discrepancias = new Dictionary<Guid, decimal>
            {
                { insumo.Id, 18 }
            };

            // Act
            await inventoryService.ReceivePedidoAsync(pedido.Id, "Admin", discrepancias, CancellationToken.None);

            // Assert
            Assert.Equal(23, stockSolicitante.StockActual); // 5 inicial + 18 recibidos
            Assert.Equal(EstadoPedidoInterSede.Recibido, pedido.Estado);
            Assert.Equal(18, detalle.CantidadRecibida);
            mockContext.Verify(c => c.MovimientosInsumo.Add(It.Is<MovimientoInsumo>(m => m.TipoMovimiento == "TransferenciaEntrada" && m.CantidadBase == 18)), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
