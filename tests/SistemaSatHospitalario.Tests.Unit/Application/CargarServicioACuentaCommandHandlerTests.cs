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
using SistemaSatHospitalario.Tests.Unit.Common;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CargarServicioACuentaCommandHandlerTests
    {
        private readonly Mock<IBillingRepository> _repositoryMock;
        private readonly Mock<IOrdenExternaService> _externaServiceMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IHonorariumMapperService> _mapperServiceMock;
        private readonly CargarServicioACuentaCommandHandler _handler;

        public CargarServicioACuentaCommandHandlerTests()
        {
            _repositoryMock = new Mock<IBillingRepository>();
            _externaServiceMock = new Mock<IOrdenExternaService>();
            _contextMock = new Mock<IApplicationDbContext>();
            _mapperServiceMock = new Mock<IHonorariumMapperService>();

            // Setup default empty DB sets to avoid NullReferenceExceptions
            var emptyPacientes = new List<PacienteAdmision>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.PacientesAdmision).Returns(emptyPacientes);

            var emptyServicios = new List<ServicioClinico>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(emptyServicios);

            var emptyConfig = new List<ConfiguracionGeneral>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.ConfiguracionGeneral).Returns(emptyConfig);

            var emptyHonorarios = new List<HonorarioConfig>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.HonorariosConfig).Returns(emptyHonorarios);

            var mockLogs = new Mock<DbSet<LogAsignacionHonorario>>();
            _contextMock.Setup(c => c.LogsAsignacionHonorario).Returns(mockLogs.Object);

            var emptyMedicos = new List<Medico>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.Medicos).Returns(emptyMedicos);

            var mockInventory = new Mock<IInventoryService>();

            _handler = new CargarServicioACuentaCommandHandler(
                _repositoryMock.Object, 
                _externaServiceMock.Object, 
                _contextMock.Object, 
                _mapperServiceMock.Object, 
                mockInventory.Object,
                NullLogger<CargarServicioACuentaCommandHandler>.Instance);
        }

        [Fact]
        public async Task Should_CreateNewAccount_And_ReturnDetalleId_When_NoAccountExists()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();

            // Setup Mocks
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("C001", "Consulta", 100, "Medico");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation;
            service.HonorarioBase = 20;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

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
            var servicioId = Guid.NewGuid();
            var cuentaExistente = new CuentaServicios(pacienteId, "Admin", "Particular");
            
            // Setup Mocks
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("E001", "Examen", 50, "Laboratorio");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Other;
            service.HonorarioBase = 0;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuentaExistente);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = servicioId.ToString(),
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

        [Fact]
        public async Task Should_SumPriceToHonorarium_When_ServiceIsConsultationAndHonorariumIsDefaultOrZero()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            CuentaServicios capturedCuenta = null;

            // Setup Mocks
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("C001", "Consulta Médica", 100, "Medico");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation;
            service.HonorarioBase = 20;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            var medico = new Medico("Dr. House", Guid.NewGuid(), 20);
            typeof(Medico).GetProperty("Id")?.SetValue(medico, medicoId);
            var medicoSet = new List<Medico> { medico }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Medicos).Returns(medicoSet.Object);

            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CuentaServicios)null);

            _repositoryMock.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Callback<CuentaServicios, CancellationToken>((c, _) => capturedCuenta = c)
                .Returns(Task.CompletedTask);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = pacienteId,
                TipoIngreso = "Particular",
                ServicioId = servicioId.ToString(),
                Descripcion = "Consulta Médica",
                Precio = 100,
                Honorario = 20, // Coincide con HonorarioBase
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "Admin",
                MedicoId = medicoId,
                HoraCita = DateTime.Today.AddHours(10)
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            capturedCuenta.Should().NotBeNull();
            capturedCuenta.Detalles.Should().HaveCount(1);
            
            // Honorario final debe ser el base (20) y el precio la suma (120)
            capturedCuenta.Detalles.First().Honorario.Should().Be(20);
            capturedCuenta.Detalles.First().Precio.Should().Be(120);
        }
    }
}

