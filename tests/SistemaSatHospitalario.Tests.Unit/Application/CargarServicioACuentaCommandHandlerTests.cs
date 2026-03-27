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
        private readonly Mock<SistemaSatHospitalario.Core.Application.Common.Interfaces.IApplicationDbContext> _contextMock;
        private readonly CargarServicioACuentaCommandHandler _handler;

        public CargarServicioACuentaCommandHandlerTests()
        {
            _repositoryMock = new Mock<IBillingRepository>();
            _externaServiceMock = new Mock<IOrdenExternaService>();
            _contextMock = new Mock<SistemaSatHospitalario.Core.Application.Common.Interfaces.IApplicationDbContext>();
            _handler = new CargarServicioACuentaCommandHandler(_repositoryMock.Object, _externaServiceMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task Should_CreateNewAccount_And_ReturnDetalleId_When_NoAccountExists()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();

            var mockPaciente = new PacienteAdmision("123", "Test", "Test");
            // Reflection hack to set the ID since it's protected set and GUID is generated in constructor but we want to control it or just use the generated one
            // Let's assume we can just use a real object for simplicity if IApplicationDbContext allows it
            
            // Setting up DBSet mock is complex, let's keep it simple for now by just returning a valid object or null
            // We'll use a real collection if possible or just mock the FirstOrDefaultAsync
            
            // For now, let's just make the code compile and run with GUIDs.
            // In a full implementation, we would mock the DbSet<PacienteAdmision> 
            // to return our mockPaciente when queried by FirstOrDefaultAsync.
            
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
            var pacienteId = Guid.NewGuid();
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
