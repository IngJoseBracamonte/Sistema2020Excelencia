using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class SyncCarritoCommandTests
    {
        private readonly Mock<IBillingRepository> _mockBillingRepo;
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ILegacyLabRepository> _mockLegacyRepo;
        private readonly Mock<IHonorariumMapperService> _mockMapperService;
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly Mock<ILogger<SyncCarritoCommandHandler>> _mockLogger;

        private readonly PacienteAdmision _testPaciente;
        private readonly Guid _testPacienteId;
        private readonly ServicioClinico _testServicio;
        private readonly Guid _testServicioGuidId;
        private readonly string _testServicioLegacyId = "158";
        private readonly string _testUsuario = "admin_test";

        public SyncCarritoCommandTests()
        {
            _mockBillingRepo = new Mock<IBillingRepository>();
            _mockContext = new Mock<IApplicationDbContext>();
            _mockLegacyRepo = new Mock<ILegacyLabRepository>();
            _mockMapperService = new Mock<IHonorariumMapperService>();
            _mockInventoryService = new Mock<IInventoryService>();
            _mockLogger = new Mock<ILogger<SyncCarritoCommandHandler>>();

            // Paciente existente con Id auto-generado
            _testPaciente = new PacienteAdmision("V-99999997", "Paciente Test Sync", "04241234567");
            _testPacienteId = _testPaciente.Id;

            var pacientesList = new List<PacienteAdmision> { _testPaciente }.BuildMockDbSet<PacienteAdmision>();
            _mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesList.Object);

            // Servicio GUID válido
            _testServicio = new ServicioClinico("SRV01", "Servicio Test GUID", 25.00m, "General");
            _testServicioGuidId = _testServicio.Id;

            var serviciosList = new List<ServicioClinico> { _testServicio }.BuildMockDbSet<ServicioClinico>();
            _mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);

            // DbSets vacíos requeridos
            _mockContext.Setup(c => c.DetallesServicioCuenta).Returns(new List<DetalleServicioCuenta>().BuildMockDbSet<DetalleServicioCuenta>().Object);
            _mockContext.Setup(c => c.HonorariosMedicosServicios).Returns(new List<HonorarioMedicoServicio>().BuildMockDbSet<HonorarioMedicoServicio>().Object);
            _mockContext.Setup(c => c.AuditLogsPrecios).Returns(new List<LogAuditoriaPrecio>().BuildMockDbSet<LogAuditoriaPrecio>().Object);
            _mockContext.Setup(c => c.LogsAsignacionHonorario).Returns(new List<LogAsignacionHonorario>().BuildMockDbSet<LogAsignacionHonorario>().Object);
            _mockContext.Setup(c => c.HonorariosConfig).Returns(new List<HonorarioConfig>().BuildMockDbSet<HonorarioConfig>().Object);
            _mockContext.Setup(c => c.Medicos).Returns(new List<Medico>().BuildMockDbSet<Medico>().Object);
            _mockContext.Setup(c => c.ConvenioPerfilPrecios).Returns(new List<ConvenioPerfilPrecio>().BuildMockDbSet<ConvenioPerfilPrecio>().Object);
            _mockContext.Setup(c => c.PreciosServicioConvenio).Returns(new List<PrecioServicioConvenio>().BuildMockDbSet<PrecioServicioConvenio>().Object);
            _mockContext.Setup(c => c.ConfiguracionGeneral).Returns(new List<ConfiguracionGeneral>().BuildMockDbSet<ConfiguracionGeneral>().Object);

            // Billing repo
            _mockBillingRepo.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CuentaServicios?)null);
            _mockBillingRepo.Setup(r => r.AgregarCuentaAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mockBillingRepo.Setup(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Mapper
            _mockMapperService.Setup(m => m.MapToCategoryAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync("Otros");

            // Inventory
            _mockInventoryService.Setup(i => i.DeductInventoryForServiceDetailAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);
        }

        private SyncCarritoCommandHandler CreateHandler()
        {
            return new SyncCarritoCommandHandler(
                _mockBillingRepo.Object,
                _mockContext.Object,
                _mockLegacyRepo.Object,
                _mockMapperService.Object,
                _mockInventoryService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ItemConGuid_DebeRetornarServicioIdGuidEnDetalles()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = _testPacienteId,
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioGuidId.ToString(),
                        Descripcion = "Servicio Test GUID",
                        Precio = 25.00m,
                        Honorario = 0m,
                        Cantidad = 1,
                        TipoServicio = "General"
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.CuentaId);
            Assert.Single(result.Detalles);
            Assert.Equal(_testServicioGuidId, result.Detalles[0].ServicioId);
            Assert.NotEqual(Guid.Empty, result.Detalles[0].DetalleId);
        }

        [Fact]
        public async Task Handle_ItemConIdLegacy_DebeRetornarGuidEmptyComoServicioId()
        {
            // Arrange: Item de laboratorio con ID legacy "158" (no parseable como GUID)
            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = _testPacienteId,
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioLegacyId, // "158"
                        Descripcion = "Perfil Hepático",
                        Precio = 15.00m,
                        Honorario = 0m,
                        Cantidad = 1,
                        TipoServicio = "Laboratorio"
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert: para items legacy, ServicioId será Guid.Empty porque "158" no es parseable como GUID
            Assert.NotNull(result);
            Assert.Single(result.Detalles);
            Assert.Equal(Guid.Empty, result.Detalles[0].ServicioId);
            Assert.NotEqual(Guid.Empty, result.Detalles[0].DetalleId);
        }

        [Fact]
        public async Task Handle_PrecioModificadoSinSupervisorKey_DebeLanzarExcepcion()
        {
            // Arrange: Configurar la clave de supervisor
            var config = new ConfiguracionGeneral("Hospital Test", "J-12345", 16m, "CLAVE_SECRETA_123");
            var configList = new List<ConfiguracionGeneral> { config }.BuildMockDbSet<ConfiguracionGeneral>();
            _mockContext.Setup(c => c.ConfiguracionGeneral).Returns(configList.Object);

            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = _testPacienteId,
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = false, // No es privilegiado
                SupervisorKey = null, // Sin clave
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioGuidId.ToString(),
                        Descripcion = "Servicio Test GUID",
                        Precio = 999.99m, // Precio alterado vs base 25.00
                        Honorario = 0m,
                        Cantidad = 1,
                        TipoServicio = "General"
                    }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Contains("Clave de Supervisor", ex.Message);
        }

        [Fact]
        public async Task Handle_PrecioModificadoConSupervisorKeyValida_DebePermitirSync()
        {
            // Arrange
            var config = new ConfiguracionGeneral("Hospital Test", "J-12345", 16m, "CLAVE_SECRETA_123");
            var configList = new List<ConfiguracionGeneral> { config }.BuildMockDbSet<ConfiguracionGeneral>();
            _mockContext.Setup(c => c.ConfiguracionGeneral).Returns(configList.Object);

            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = _testPacienteId,
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = false,
                SupervisorKey = "CLAVE_SECRETA_123", // Clave válida
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioGuidId.ToString(),
                        Descripcion = "Servicio Test GUID",
                        Precio = 999.99m, // Precio modificado
                        Honorario = 0m,
                        Cantidad = 1,
                        TipoServicio = "General"
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Detalles);
            _mockBillingRepo.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PacienteNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = Guid.NewGuid(), // ID que no existe en el mock
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioGuidId.ToString(),
                        Descripcion = "Test",
                        Precio = 25.00m,
                        Cantidad = 1,
                        TipoServicio = "General"
                    }
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Contains("No se pudo resolver la identidad del paciente", ex.Message);
        }

        [Fact]
        public async Task Handle_ItemConTipoServicioInforme_DebeSincronizarCorrectamente()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new SyncCarritoCommand
            {
                PacienteId = _testPacienteId,
                UsuarioCarga = _testUsuario,
                TipoIngreso = "Particular",
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new ServicioCarritoDto
                    {
                        ServicioId = _testServicioGuidId.ToString(),
                        Descripcion = "Informe Radiológico RX Tórax",
                        Precio = 10.00m,
                        Honorario = 5.00m,
                        Cantidad = 1,
                        TipoServicio = "Informe"
                    }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Detalles);
            _mockBillingRepo.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
