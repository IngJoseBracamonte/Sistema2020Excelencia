using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MediatR;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CargarServiciosMasivoCommandHandlerTests
    {
        private readonly Mock<IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult>> _singleHandlerMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly CargarServiciosMasivoCommandHandler _handler;

        public CargarServiciosMasivoCommandHandlerTests()
        {
            _singleHandlerMock = new Mock<IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult>>();
            _contextMock = new Mock<IApplicationDbContext>();
            _transactionMock = new Mock<IDbContextTransaction>();

            _contextMock.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _handler = new CargarServiciosMasivoCommandHandler(
                _singleHandlerMock.Object,
                _contextMock.Object,
                NullLogger<CargarServiciosMasivoCommandHandler>.Instance);
        }

        [Fact]
        public async Task Should_ProcessAllItems_And_CommitTransaction_When_AllSucceed()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();
            var serviceId1 = Guid.NewGuid();
            var serviceId2 = Guid.NewGuid();

            var item1 = new ServicioMasivoItemDto
            {
                ServicioId = serviceId1.ToString(),
                Descripcion = "Servicio 1",
                Precio = 50,
                Cantidad = 2,
                TipoServicio = "Medicamento"
            };

            var item2 = new ServicioMasivoItemDto
            {
                ServicioId = serviceId2.ToString(),
                Descripcion = "Servicio 2",
                Precio = 100,
                Cantidad = 1,
                TipoServicio = "Laboratorio"
            };

            var command = new CargarServiciosMasivoCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Emergencia",
                ConvenioId = 1,
                Items = new List<ServicioMasivoItemDto> { item1, item2 }
            };

            _singleHandlerMock.Setup(h => h.Handle(It.Is<CargarServicioACuentaCommand>(c => c.ServicioId == item1.ServicioId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CargarServicioResult(cuentaId, Guid.NewGuid()));

            _singleHandlerMock.Setup(h => h.Handle(It.Is<CargarServicioACuentaCommand>(c => c.ServicioId == item2.ServicioId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CargarServicioResult(cuentaId, Guid.NewGuid()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            _singleHandlerMock.Verify(h => h.Handle(It.IsAny<CargarServicioACuentaCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_RollbackTransaction_And_ThrowException_When_AnyItemFails()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var serviceId1 = Guid.NewGuid();

            var item1 = new ServicioMasivoItemDto
            {
                ServicioId = serviceId1.ToString(),
                Descripcion = "Servicio Fallido",
                Precio = 50,
                Cantidad = 1,
                TipoServicio = "Medicamento"
            };

            var command = new CargarServiciosMasivoCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Emergencia",
                Items = new List<ServicioMasivoItemDto> { item1 }
            };

            _singleHandlerMock.Setup(h => h.Handle(It.IsAny<CargarServicioACuentaCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Insumo sin stock suficiente"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
