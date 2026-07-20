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
    public class RevertirCheckOutTests
    {
        [Fact]
        public async Task Handle_ShouldReopenAccountAndRestoreBedToOcupada()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<RevertirCheckOutCommandHandler>>();

            var paciente = new PacienteAdmision("12345", "Juan Pérez", "555-1234", null, DateTime.UtcNow.AddYears(-30), "Dirección");
            var sede = new Sede("S01", "Sede Central", true);
            var cama = new AreaClinica(sede.Id, "C01", "Cama 101");
            cama.MarcarComoOcupada();

            var cuenta = new CuentaServicios(paciente.Id, "nurse_admin", "Hospitalizacion", null, cama.Id, null, null);
            cuenta.Facturar(); // Marks as Facturada and sets FechaCierre

            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios> { cuenta }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(new List<AreaClinica> { cama }.BuildMockDbSet().Object);

            var command = new RevertirCheckOutCommand(cuenta.Id, "nurse_admin");
            var handler = new RevertirCheckOutCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(EstadoConstants.Abierta, cuenta.Estado);
            Assert.Null(cuenta.FechaCierre);
            Assert.Equal(EstadoUbicacion.Ocupada, cama.Estado);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAccountIsAlreadyOpen()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<RevertirCheckOutCommandHandler>>();

            var paciente = new PacienteAdmision("12345", "Juan Pérez", "555-1234", null, DateTime.UtcNow.AddYears(-30), "Dirección");
            var sede = new Sede("S01", "Sede Central", true);
            var cama = new AreaClinica(sede.Id, "C01", "Cama 101");

            var cuenta = new CuentaServicios(paciente.Id, "nurse_admin", "Hospitalizacion", null, cama.Id, null, null);
            // It is open by default

            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios> { cuenta }.BuildMockDbSet().Object);

            var command = new RevertirCheckOutCommand(cuenta.Id, "nurse_admin");
            var handler = new RevertirCheckOutCommandHandler(mockContext.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
