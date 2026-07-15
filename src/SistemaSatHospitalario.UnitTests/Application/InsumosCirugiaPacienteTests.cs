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
    public class InsumosCirugiaPacienteTests
    {
        [Fact]
        public void RegistrarDevolucion_ShouldDecreaseConsumidaAndIncreaseDevuelta()
        {
            // Arrange
            var entity = new InsumoCirugiaPaciente(Guid.NewGuid(), Guid.NewGuid(), 10m);

            // Act
            entity.RegistrarDevolucion(3m);

            // Assert
            Assert.Equal(10m, entity.CantidadEntregada);
            Assert.Equal(3m, entity.CantidadDevuelta);
            Assert.Equal(7m, entity.CantidadConsumida);
        }

        [Fact]
        public void RegistrarDevolucion_ShouldThrowException_WhenDevolucionExceedsEntregada()
        {
            // Arrange
            var entity = new InsumoCirugiaPaciente(Guid.NewGuid(), Guid.NewGuid(), 10m);
            entity.RegistrarDevolucion(5m);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => entity.RegistrarDevolucion(6m));
            Assert.Contains("No se pueden devolver más insumos de los entregados", ex.Message);
        }

        [Fact]
        public async Task HandleDevolucionCommand_ShouldRestockPhysicalInventoryAndAdjustBillingCargo()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<DevolverInsumoCirugiaCommandHandler>>();

            var insumo = new Insumo("I01", "Insumo Quirurgico", 0m, UnidadMedida.UNIDAD, 10m, true);
            var kitAsignacion = new InsumoCirugiaPaciente(Guid.NewGuid(), insumo.Id, 10m);
            typeof(InsumoCirugiaPaciente).GetProperty("Insumo")?.SetValue(kitAsignacion, insumo);

            var stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Principal, 5m);

            var cuenta = new CuentaServicios(Guid.NewGuid(), "admin", "Hospitalizacion");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, kitAsignacion.CuentaServicioId);

            var detalleCargo = new DetalleServicioCuenta(cuenta.Id, insumo.Id, "Cargo Insumo", 15m, 0m, 10m, "Insumo", "admin");

            mockContext.Setup(c => c.InsumosCirugiasPacientes).Returns(new List<InsumoCirugiaPaciente> { kitAsignacion }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios> { cuenta }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.StocksSedes).Returns(new List<StockSede> { stockSede }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(new List<DetalleServicioCuenta> { detalleCargo }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(new List<MovimientoInsumo>().BuildMockDbSet().Object);

            var command = new DevolverInsumoCirugiaCommand(cuenta.Id, insumo.Id, 4m, "nurse_user");
            var handler = new DevolverInsumoCirugiaCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(4m, kitAsignacion.CantidadDevuelta);
            Assert.Equal(6m, kitAsignacion.CantidadConsumida);

            // Check stock was restocked: 5 initial + 4 returned = 9
            Assert.Equal(9m, stockSede.StockActual);

            // Check billing charge adjusted: 10 original - 4 returned = 6
            Assert.Equal(6m, detalleCargo.Cantidad);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
