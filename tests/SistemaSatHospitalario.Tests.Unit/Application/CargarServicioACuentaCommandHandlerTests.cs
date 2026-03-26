using System;
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
    public class CargarServicioACuentaCommandHandlerTests
    {
        private readonly Mock<IBillingRepository> _repositoryMock;
        private readonly Mock<IOrdenExternaService> _externaServiceMock;
        private readonly CargarServicioACuentaCommandHandler _handler;

        public CargarServicioACuentaCommandHandlerTests()
        {
            _repositoryMock = new Mock<IBillingRepository>();
            _externaServiceMock = new Mock<IOrdenExternaService>();
            _handler = new CargarServicioACuentaCommandHandler(_repositoryMock.Object, _externaServiceMock.Object);
        }

        [Fact]
        public async Task Should_CreateNewAccount_And_ReturnDetalleId_When_NoAccountExists()
        {
            // Arrange
            var pacienteId = 123;
            var servicioId = Guid.NewGuid();
            
            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CuentaServicios)null);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = servicioId,
                Descripcion = "Consulta",
                Precio = 100,
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "Admin",
                MedicoId = Guid.NewGuid(),
                HoraCita = DateTime.Today.AddHours(10)
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaId.Should().NotBeEmpty();
            result.DetalleId.Should().NotBeEmpty();
            
            _repositoryMock.Verify(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Should_ReuseExistingAccount_When_Available()
        {
            // Arrange
            var pacienteId = 456;
            var cuentaExistente = new CuentaServicios(pacienteId, "Particular");
            
            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuentaExistente);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = Guid.NewGuid(),
                Descripcion = "Examen",
                Precio = 50,
                Cantidad = 1,
                TipoServicio = "Laboratorio",
                UsuarioCarga = "Admin"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.CuentaId.Should().Be(cuentaExistente.Id);
            cuentaExistente.Detalles.Should().HaveCount(1);
            
            _repositoryMock.Verify(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
