using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class EnviarASubAreaCommandTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<EnviarASubAreaCommandHandler>> _mockLogger;

        private readonly Guid _insumoId = Guid.NewGuid();
        private readonly Guid _subAreaId = SeedConstants.AreaId_Laboratorio;
        private readonly string _usuario = "farmaceutico_test";

        public EnviarASubAreaCommandTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockLogger = new Mock<ILogger<EnviarASubAreaCommandHandler>>();
        }

        [Fact]
        public async Task Handle_StockSuficiente_DebeDescontarUnicamenteDeAlmacenPrincipalYRegistrarMovimiento()
        {
            // Arrange
            var insumo = new Insumo("INS-001", "Tubos de Ensayo Tapa Roja", 100, UnidadMedida.UNIDAD, 0.50m);
            typeof(Insumo).GetProperty("Id")!.SetValue(insumo, _insumoId);

            var stockPrincipal = new StockSede(_insumoId, SeedConstants.SedeId_Principal, 100m);

            var insumosList = new List<Insumo> { insumo }.BuildMockDbSet<Insumo>();
            var stocksList = new List<StockSede> { stockPrincipal }.BuildMockDbSet<StockSede>();
            var movimientosList = new List<MovimientoInsumo>();
            var mockMovimientosDbSet = movimientosList.BuildMockDbSet<MovimientoInsumo>();

            var areaLab = new AreaClinica(SeedConstants.SedeId_Principal, "LABORATORIO", "Laboratorio Central");
            typeof(AreaClinica).GetProperty("Id")!.SetValue(areaLab, _subAreaId);
            var areasList = new List<AreaClinica> { areaLab }.BuildMockDbSet<AreaClinica>();
            _mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);

            _mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);
            _mockContext.Setup(c => c.StocksSedes).Returns(stocksList.Object);
            _mockContext.Setup(c => c.MovimientosInsumo).Returns(mockMovimientosDbSet.Object);
            _mockContext.Setup(c => c.MovimientosInsumo.Add(It.IsAny<MovimientoInsumo>()))
                .Callback<MovimientoInsumo>(m => movimientosList.Add(m));

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var handler = new EnviarASubAreaCommandHandler(_mockContext.Object, _mockLogger.Object);
            var command = new EnviarASubAreaCommand
            {
                InsumoId = _insumoId,
                AreaClinicaId = _subAreaId,
                NombreSubArea = "Laboratorio",
                Cantidad = 25m,
                Motivo = "Reposición semanal para reactivos",
                Usuario = _usuario
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            // Debe haber descontado 25 del stock principal (100 - 25 = 75)
            Assert.Equal(75m, stockPrincipal.StockActual);

            // Verificar que se haya registrado 1 movimiento de tipo EnvioSubArea
            Assert.Single(movimientosList);
            var mov = movimientosList[0];
            Assert.Equal(_insumoId, mov.InsumoId);
            Assert.Equal(SeedConstants.SedeId_Principal, mov.SedeId);
            Assert.Equal("EnvioSubArea", mov.TipoMovimiento);
            Assert.Equal(25m, mov.CantidadOriginal);
            Assert.Equal(_usuario, mov.Usuario);
            Assert.Contains("Laboratorio", mov.Motivo);

            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_StockInsuficiente_DebeLanzarExcepcion()
        {
            // Arrange
            var insumo = new Insumo("INS-002", "Reactivo Glucosa", 10, UnidadMedida.UNIDAD, 12.00m);
            typeof(Insumo).GetProperty("Id")!.SetValue(insumo, _insumoId);

            var stockPrincipal = new StockSede(_insumoId, SeedConstants.SedeId_Principal, 5m); // Solo 5 disponibles

            var insumosList = new List<Insumo> { insumo }.BuildMockDbSet<Insumo>();
            var stocksList = new List<StockSede> { stockPrincipal }.BuildMockDbSet<StockSede>();

            var areaLab = new AreaClinica(SeedConstants.SedeId_Principal, "LABORATORIO", "Laboratorio Central");
            typeof(AreaClinica).GetProperty("Id")!.SetValue(areaLab, _subAreaId);
            var areasList = new List<AreaClinica> { areaLab }.BuildMockDbSet<AreaClinica>();
            _mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);

            _mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);
            _mockContext.Setup(c => c.StocksSedes).Returns(stocksList.Object);

            var handler = new EnviarASubAreaCommandHandler(_mockContext.Object, _mockLogger.Object);
            var command = new EnviarASubAreaCommand
            {
                InsumoId = _insumoId,
                AreaClinicaId = _subAreaId,
                NombreSubArea = "Laboratorio",
                Cantidad = 20m, // Solicitando 20 cuando solo hay 5
                Motivo = "Urgente",
                Usuario = _usuario
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Contains("Stock insuficiente", ex.Message);
        }

        [Fact]
        public async Task Handle_InsumoNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange
            var insumosList = new List<Insumo>().BuildMockDbSet<Insumo>();
            _mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);

            var handler = new EnviarASubAreaCommandHandler(_mockContext.Object, _mockLogger.Object);
            var command = new EnviarASubAreaCommand
            {
                InsumoId = Guid.NewGuid(),
                AreaClinicaId = _subAreaId,
                NombreSubArea = "Laboratorio",
                Cantidad = 5m,
                Motivo = "Test",
                Usuario = _usuario
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
