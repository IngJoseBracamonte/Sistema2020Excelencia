using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CargarServiciosMasivoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Process_All_Items_Without_External_Transaction_Conflict()
        {
            // Arrange
            var mockSingleHandler = new Mock<IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult>>();
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<CargarServiciosMasivoCommandHandler>>();
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(u => u.UserId).Returns(Guid.NewGuid());

            mockSingleHandler
                .Setup(h => h.Handle(It.IsAny<CargarServicioACuentaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CargarServicioACuentaCommand cmd, CancellationToken ct) => 
                    new CargarServicioResult(cmd.CuentaId ?? Guid.NewGuid(), Guid.NewGuid()));

            var handler = new CargarServiciosMasivoCommandHandler(
                mockSingleHandler.Object,
                mockContext.Object,
                mockLogger.Object,
                mockUserService.Object
            );

            var command = new CargarServiciosMasivoCommand
            {
                CuentaId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                TipoIngreso = "UCI",
                Items = new List<ServicioMasivoItemDto>
                {
                    new ServicioMasivoItemDto { ServicioId = Guid.NewGuid().ToString(), Descripcion = "Item 1", Cantidad = 1, Precio = 10m },
                    new ServicioMasivoItemDto { ServicioId = Guid.NewGuid().ToString(), Descripcion = "Item 2", Cantidad = 2, Precio = 20m }
                }
            };

            // Act
            var results = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            mockSingleHandler.Verify(h => h.Handle(It.IsAny<CargarServicioACuentaCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
