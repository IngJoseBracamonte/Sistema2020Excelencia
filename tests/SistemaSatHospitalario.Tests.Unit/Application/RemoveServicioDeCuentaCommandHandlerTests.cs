using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class RemoveServicioDeCuentaCommandHandlerTests
    {
        private readonly Mock<IBillingRepository> _repositoryMock;
        private readonly RemoveServicioDeCuentaCommandHandler _handler;

        public RemoveServicioDeCuentaCommandHandlerTests()
        {
            _repositoryMock = new Mock<IBillingRepository>();
            _handler = new RemoveServicioDeCuentaCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Should_RemoveService_And_CancelAppointment_When_MetadataProvided()
        {
            // Arrange
            var cuentaId = Guid.NewGuid();
            var detalleId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            var horaCita = DateTime.Today.AddHours(10);
            var pacienteId = Guid.NewGuid();

            var cuenta = new CuentaServicios(pacienteId, "Admin", "Particular");
            // Usamos reflection para setear el ID o simplemente confiamos en el flujo
            // En una prueba real, podriamos necesitar un Factory o setear el ID via reflection si es privado
            
            var detalle = cuenta.AgregarServicio(Guid.NewGuid(), "Consulta Medica", 100, 1, "Medico", "Admin");
            
            // Mocking repository to return our account
            _repositoryMock.Setup(r => r.ObtenerCuentaPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuenta);

            var command = new RemoveServicioDeCuentaCommand
            {
                CuentaId = cuentaId,
                DetalleId = detalle.Id,
                MedicoId = medicoId,
                HoraCita = horaCita
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            cuenta.Detalles.Should().BeEmpty();
            _repositoryMock.Verify(r => r.CancelarCitaMedicaAsync(cuentaId, medicoId, horaCita, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Only_RemoveService_When_NoMetadataProvided()
        {
            // Arrange
            var cuentaId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Admin", "Particular");
            var detalle = cuenta.AgregarServicio(Guid.NewGuid(), "Examen RX", 50, 1, "RX", "Admin");

            _repositoryMock.Setup(r => r.ObtenerCuentaPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuenta);

            var command = new RemoveServicioDeCuentaCommand
            {
                CuentaId = cuentaId,
                DetalleId = detalle.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            cuenta.Detalles.Should().BeEmpty();
            _repositoryMock.Verify(r => r.CancelarCitaMedicaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
