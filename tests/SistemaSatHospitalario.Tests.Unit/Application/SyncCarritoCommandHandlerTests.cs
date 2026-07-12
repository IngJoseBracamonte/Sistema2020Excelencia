using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using SistemaSatHospitalario.Tests.Unit.Common;
using SistemaSatHospitalario.Tests.Unit.Common.Builders;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class SyncCarritoCommandHandlerTests
    {
        private readonly Mock<IBillingRepository> _repositoryMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ILegacyLabRepository> _legacyRepositoryMock;
        private readonly Mock<IHonorariumMapperService> _mapperServiceMock;
        private readonly Mock<ILogger<SyncCarritoCommandHandler>> _loggerMock;
        private readonly SyncCarritoCommandHandler _handler;

        public SyncCarritoCommandHandlerTests()
        {
            _repositoryMock = new Mock<IBillingRepository>();
            _contextMock = new Mock<IApplicationDbContext>();
            _legacyRepositoryMock = new Mock<ILegacyLabRepository>();
            _mapperServiceMock = new Mock<IHonorariumMapperService>();
            _loggerMock = new Mock<ILogger<SyncCarritoCommandHandler>>();

            // Setup default empty DB sets to avoid NullReferenceExceptions
            var emptyPacientes = new List<PacienteAdmision>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.PacientesAdmision).Returns(emptyPacientes);

            var emptyServicios = new List<ServicioClinico>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(emptyServicios);

            var emptyConfig = new List<ConfiguracionGeneral>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.ConfiguracionGeneral).Returns(emptyConfig);

            var mockAudit = new Mock<DbSet<LogAuditoriaPrecio>>();
            _contextMock.Setup(c => c.AuditLogsPrecios).Returns(mockAudit.Object);

            var emptyMedicos = new List<Medico>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.Medicos).Returns(emptyMedicos);

            var emptyHonorariosConfig = new List<HonorarioConfig>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.HonorariosConfig).Returns(emptyHonorariosConfig);

            var mockLogsAsignacion = new Mock<DbSet<LogAsignacionHonorario>>();
            _contextMock.Setup(c => c.LogsAsignacionHonorario).Returns(mockLogsAsignacion.Object);

            var emptyRules = new List<HonorariumMappingRule>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.HonorariumMappingRules).Returns(emptyRules);

            var emptyConvenioPerfilPrecios = new List<ConvenioPerfilPrecio>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.ConvenioPerfilPrecios).Returns(emptyConvenioPerfilPrecios);

            var emptyPreciosServicioConvenio = new List<PrecioServicioConvenio>().AsQueryable().BuildMockDbSet().Object;
            _contextMock.Setup(c => c.PreciosServicioConvenio).Returns(emptyPreciosServicioConvenio);

            var mockInventory = new Mock<IInventoryService>();

            _handler = new SyncCarritoCommandHandler(
                _repositoryMock.Object, 
                _contextMock.Object, 
                _legacyRepositoryMock.Object, 
                _mapperServiceMock.Object, 
                mockInventory.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithInvalidPacienteId_ShouldThrowException_InsteadOfCreatingStub()
        {
            // Arrange (V11.1: El paciente no existe en el contexto nativo)
            var pacienteId = Guid.NewGuid();
            
            // Simular DBSet vacío (ya configurado en el constructor)
            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "TestUser",
                Items = new List<ServicioCarritoDto> { new ServicioCarritoDto { ServicioId = Guid.NewGuid().ToString(), Precio = 10, TipoServicio = "Medico" } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            
            _repositoryMock.Verify(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithValidPacienteId_ShouldSyncCorrectly()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("S001", "Servicio Test", 50, "Otros");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Other;
            service.HonorarioBase = 10;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            var cuenta = new CuentaServicios(pacienteId, "TestUser", "Particular");
            _repositoryMock.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Callback<CuentaServicios, CancellationToken>((c, _) => {
                    typeof(CuentaServicios).GetProperty("Id")?.SetValue(c, Guid.NewGuid());
                })
                .Returns(Task.CompletedTask);

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "TestUser",
                TipoIngreso = "Particular",
                Items = new List<ServicioCarritoDto>
                {
                    new()
                    {
                        ServicioId = servicioId.ToString(),
                        Descripcion = "Servicio Test",
                        Precio = 50,
                        Honorario = 10,
                        Cantidad = 1,
                        TipoServicio = "Otros"
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaId.Should().NotBeEmpty();
            result.Detalles.Should().HaveCount(1);
            result.Detalles[0].ServicioId.Should().Be(servicioId);

            _repositoryMock.Verify(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithConsultationService_ShouldSumPriceToHonorarium()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            CuentaServicios capturedCuenta = null;

            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("C001", "Consulta Médica", 150, "Medico");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation;
            service.HonorarioBase = 30;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            var medico = new Medico("Dr. House", Guid.NewGuid(), 30);
            typeof(Medico).GetProperty("Id")?.SetValue(medico, medicoId);
            var medicoSet = new List<Medico> { medico }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Medicos).Returns(medicoSet.Object);

            _repositoryMock.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Callback<CuentaServicios, CancellationToken>((c, _) => {
                    capturedCuenta = c;
                    typeof(CuentaServicios).GetProperty("Id")?.SetValue(c, Guid.NewGuid());
                })
                .Returns(Task.CompletedTask);

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "TestUser",
                TipoIngreso = "Particular",
                Items = new List<ServicioCarritoDto>
                {
                    new()
                    {
                        ServicioId = servicioId.ToString(),
                        Descripcion = "Consulta Médica",
                        Precio = 150,
                        Honorario = 30, // Coincide con HonorarioBase
                        Cantidad = 1,
                        TipoServicio = "Medico",
                        MedicoId = medicoId,
                        HoraCita = DateTime.Today.AddHours(10)
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            capturedCuenta.Should().NotBeNull();
            capturedCuenta.Detalles.Should().HaveCount(1);
            
            // Honorario final debe ser el base (30) y el precio la suma (180)
            capturedCuenta.Detalles.First().Honorario.Should().Be(30);
            capturedCuenta.Detalles.First().Precio.Should().Be(180);
        }

        [Fact]
        public async Task Handle_WithCumulativeTipoIngreso_ShouldReuseOpenAccount()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();
            
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("S001", "Servicio Test", 50, "Otros");
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            service.Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Other;
            service.HonorarioBase = 0;
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            var cuentaExistente = new CuentaServicios(pacienteId, "TestUser", "Hospitalizacion");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuentaExistente, cuentaId);
            
            // Simular repositorio para retornar la cuenta abierta
            _repositoryMock.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuentaExistente);

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "TestUser",
                TipoIngreso = "Hospitalizacion",
                Items = new List<ServicioCarritoDto>
                {
                    new()
                    {
                        ServicioId = servicioId.ToString(),
                        Descripcion = "Servicio Test",
                        Precio = 50,
                        Honorario = 0,
                        Cantidad = 1,
                        TipoServicio = "Otros"
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // Assert
            result.Should().NotBeNull();
            result.CuentaId.Should().Be(cuentaId);
            result.Detalles.Should().HaveCount(1);

            // Verificar que NO se haya agregado una nueva cuenta
            _repositoryMock.Verify(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositoryMock.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithHospitalizationAndNoAppointmentTime_ShouldSyncWithoutCita()
        {
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("CONS-HOSP", "Consulta Hospitalización", 60, "Medico")
            {
                Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Consultation,
                HonorarioBase = 20
            };
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            CuentaServicios? capturedCuenta = null;
            _repositoryMock.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Callback<CuentaServicios, CancellationToken>((c, _) => capturedCuenta = c)
                .Returns(Task.CompletedTask);

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "NurseAdmin",
                TipoIngreso = "Hospitalizacion", // No requiere HoraCita ni MedicoId obligatorio
                Items = new List<ServicioCarritoDto>
                {
                    new()
                    {
                        ServicioId = servicioId.ToString(),
                        Descripcion = "Consulta Hospitalización",
                        Precio = 60,
                        Honorario = 20,
                        Cantidad = 1,
                        TipoServicio = "Medico"
                        // HoraCita y MedicoId quedan null/no especificados
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            capturedCuenta.Should().NotBeNull();
            capturedCuenta!.Detalles.Should().ContainSingle();
            var detail = capturedCuenta.Detalles.First();
            detail.Precio.Should().Be(80); // 60 (base) + 20 (honorario)
        }

        [Fact]
        public async Task Handle_WithConvenioPricing_ShouldUseConvenioBasePrice()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var servicioId = Guid.NewGuid();
            int convenioId = 42;
            
            var paciente = new PacienteAdmision("123", "Test Patient", "555-1234");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            var pacienteSet = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacienteSet.Object);

            var service = new ServicioClinico("SRV-CONV", "Servicio Convenio", 100, "Otros")
            {
                Category = SistemaSatHospitalario.Core.Domain.Enums.ServiceCategory.Other,
                HonorarioBase = 15
            };
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, servicioId);
            var serviceSet = new List<ServicioClinico> { service }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviceSet.Object);

            var precioConvenio = new PrecioServicioConvenio(servicioId, convenioId, 75.00m);
            var pricesSet = new List<PrecioServicioConvenio> { precioConvenio }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PreciosServicioConvenio).Returns(pricesSet.Object);

            CuentaServicios? capturedCuenta = null;
            _repositoryMock.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Callback<CuentaServicios, CancellationToken>((c, _) => capturedCuenta = c)
                .Returns(Task.CompletedTask);

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "NurseAdmin",
                TipoIngreso = "Particular",
                ConvenioId = convenioId,
                Items = new List<ServicioCarritoDto>
                {
                    new()
                    {
                        ServicioId = servicioId.ToString(),
                        Descripcion = "Servicio Convenio",
                        Precio = 75.00m, // Coincide con el precio del convenio
                        Honorario = 0,
                        Cantidad = 1,
                        TipoServicio = "Otros"
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            capturedCuenta.Should().NotBeNull();
            capturedCuenta!.Detalles.Should().ContainSingle();
            var detail = capturedCuenta.Detalles.First();
            detail.Precio.Should().Be(90.00m); // 75 (convenio) + 15 (honorario)
            detail.Honorario.Should().Be(15.00m);
        }
    }
}

