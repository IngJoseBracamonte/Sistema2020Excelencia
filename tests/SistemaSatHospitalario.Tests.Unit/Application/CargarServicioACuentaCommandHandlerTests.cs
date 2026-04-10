using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Tests.Unit.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
            _handler = new CargarServicioACuentaCommandHandler(_repositoryMock.Object, _externaServiceMock.Object, _contextMock.Object, NullLogger<CargarServicioACuentaCommandHandler>.Instance);
        }

        [Fact]
        public async Task Should_CreateNewAccount_And_ReturnDetalleId_When_NoAccountExists()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();

            // Setup Mocks usando el helper TestAsyncEnumerable para soportar querys asíncronas
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable();
            var mockPacienteSet = new Mock<DbSet<PacienteAdmision>>();
            mockPacienteSet.As<IQueryable<PacienteAdmision>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<PacienteAdmision>(pacienteSet.Provider));
            mockPacienteSet.As<IQueryable<PacienteAdmision>>().Setup(m => m.Expression).Returns(pacienteSet.Expression);
            mockPacienteSet.As<IQueryable<PacienteAdmision>>().Setup(m => m.ElementType).Returns(pacienteSet.ElementType);
            mockPacienteSet.As<IQueryable<PacienteAdmision>>().Setup(m => m.GetEnumerator()).Returns(pacienteSet.GetEnumerator());
            mockPacienteSet.As<IAsyncEnumerable<PacienteAdmision>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<PacienteAdmision>(pacienteSet.GetEnumerator()));
            
            _contextMock.Setup(c => c.PacientesAdmision).Returns(mockPacienteSet.Object);

            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CuentaServicios)null);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = servicioId.ToString(),
                Descripcion = "Consulta",
                Precio = 100,
                Honorario = 20,
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
            var cuentaExistente = new CuentaServicios(pacienteId, "Admin", "Particular");
            
            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuentaExistente);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = Guid.NewGuid().ToString(),
                Descripcion = "Examen",
                Precio = 50,
                Honorario = 0,
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
