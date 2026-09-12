using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ReactivateCatalogItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Reactivate_Item_And_Clear_Deactivation_Metadata()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var servicio = new ServicioClinico("CONS-TEST", "Consulta de Prueba", 50m, "CONSULTA");
            servicio.Desactivar("UsuarioAuditor");

            Assert.False(servicio.Activo);
            Assert.NotNull(servicio.DesactivadoPorUsuarioId);
            Assert.NotNull(servicio.FechaDesactivacion);

            mockContext.Setup(c => c.ServiciosClinicos.FindAsync(new object[] { servicio.Id }, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(servicio);

            var handler = new ReactivateCatalogItemCommandHandler(mockContext.Object);
            var command = new ReactivateCatalogItemCommand { Id = servicio.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.True(servicio.Activo);
            Assert.Null(servicio.DesactivadoPorUsuarioId);
            Assert.Null(servicio.FechaDesactivacion);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_Item_Not_Found()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var nonExistentId = Guid.NewGuid();

            mockContext.Setup(c => c.ServiciosClinicos.FindAsync(new object[] { nonExistentId }, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((ServicioClinico?)null);

            var handler = new ReactivateCatalogItemCommandHandler(mockContext.Object);
            var command = new ReactivateCatalogItemCommand { Id = nonExistentId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
