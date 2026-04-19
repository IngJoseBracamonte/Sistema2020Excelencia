using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Tests.Unit.Common;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CloseAccountLegacySyncTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<ILegacyLabRepository> _legacyRepoMock;
        private readonly Mock<ICajaAdministrativaRepository> _cajaRepoMock;
        private readonly Mock<IBillingRepository> _billingRepoMock;
        private readonly CloseAccountCommandHandler _handler;

        public CloseAccountLegacySyncTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _legacyRepoMock = new Mock<ILegacyLabRepository>();
            _cajaRepoMock = new Mock<ICajaAdministrativaRepository>();
            _billingRepoMock = new Mock<IBillingRepository>();

            _handler = new CloseAccountCommandHandler(
                _contextMock.Object,
                _legacyRepoMock.Object,
                _cajaRepoMock.Object,
                _billingRepoMock.Object,
                new Mock<ILegacyErrorReportingService>().Object
            );
        }

        [Fact]
        public async Task Should_CallLegacySync_When_LabItemsExist_And_PatientHasLegacyId()
        {
            // Arrange
            var paciente = new PacienteAdmision("12345", "Test Patient", "555-000", 101);
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");
            
            var servicioId = Guid.NewGuid();
            cuenta.AgregarServicio(servicioId, "Perfil Lipidico", 50, 0, 1, EstadoConstants.Laboratorio, "Admin", "123");

            SetupMocks(cuenta, paciente, new List<ServicioClinico>().AsQueryable());

            // Act
            await _handler.Handle(new CloseAccountCommand 
            { 
                CuentaId = cuenta.Id, 
                UsuarioId = "1", 
                Pagos = new List<DetallePagoDto>() 
            }, CancellationToken.None);

            // Assert
            _legacyRepoMock.Verify(r => r.GenerarOrdenLaboratorioAsync(
                It.Is<OrdenLegacy>(o => o.IdPersona == 101),
                It.Is<List<PerfilesFacturadosLegacy>>(p => p.Any(x => x.IdPerfil == 123)),
                It.IsAny<List<ResultadosPacienteLegacy>>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Should_HandleAlphanumericCodes_UsingSmartParser()
        {
            // Arrange
            var paciente = new PacienteAdmision("12345", "Test Patient", "555-000", 101);
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");
            
            var servicioId = Guid.NewGuid();
            // In a real scenario, the UI/Backend sets the mapping ID when adding to account
            cuenta.AgregarServicio(servicioId, "PERF-999 Profile", 50, 0, 1, EstadoConstants.Laboratorio, "Admin", "999");

            SetupMocks(cuenta, paciente, new List<ServicioClinico>().AsQueryable());

            // Act
            await _handler.Handle(new CloseAccountCommand 
            { 
                CuentaId = cuenta.Id, 
                UsuarioId = "1", 
                Pagos = new List<DetallePagoDto>() 
            }, CancellationToken.None);

            // Assert
            _legacyRepoMock.Verify(r => r.GenerarOrdenLaboratorioAsync(
                It.IsAny<OrdenLegacy>(),
                It.Is<List<PerfilesFacturadosLegacy>>(p => p.Any(x => x.IdPerfil == 999)),
                It.IsAny<List<ResultadosPacienteLegacy>>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Should_TriggerJITOnboarding_When_PatientLacksLegacyId()
        {
            // Arrange
            var paciente = new PacienteAdmision("V-7318923", "Brunequilde Gil", "555-000", null);
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");
            
            var servicioId = Guid.NewGuid();
            cuenta.AgregarServicio(servicioId, "PERFIL 20", 50, 0, 1, EstadoConstants.Laboratorio, "Admin", "1403");

            SetupMocks(cuenta, paciente, new List<ServicioClinico>().AsQueryable());

            _legacyRepoMock.Setup(r => r.GetPatientByCedulaAsync(paciente.CedulaPasaporte, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DatosPersonalesLegacy)null);
            
            _legacyRepoMock.Setup(r => r.CreatePatientLegacyAsync(It.IsAny<DatosPersonalesLegacy>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(2000);

            // Act
            await _handler.Handle(new CloseAccountCommand 
            { 
                CuentaId = cuenta.Id, 
                UsuarioId = "1", 
                Pagos = new List<DetallePagoDto>() 
            }, CancellationToken.None);

            // Assert
            paciente.IdPacienteLegacy.Should().Be(2000);
            _legacyRepoMock.Verify(r => r.CreatePatientLegacyAsync(It.IsAny<DatosPersonalesLegacy>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ApplySelfHealing_When_MappingIdIsMissing_ButDescriptionHasPrefix()
        {
            // Arrange
            var paciente = new PacienteAdmision("12345", "Test Patient", "555-000", 101);
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");
            
            var servicioId = Guid.NewGuid();
            // MappingId is null, but description has LAB-1234
            cuenta.AgregarServicio(servicioId, "LAB-1234 Test Service", 20, 0, 1, EstadoConstants.Laboratorio, "Admin", null);

            SetupMocks(cuenta, paciente, new List<ServicioClinico>().AsQueryable());

            // Act
            await _handler.Handle(new CloseAccountCommand 
            { 
                CuentaId = cuenta.Id, 
                UsuarioId = "1", 
                Pagos = new List<DetallePagoDto>() 
            }, CancellationToken.None);

            // Assert
            _legacyRepoMock.Verify(r => r.GenerarOrdenLaboratorioAsync(
                It.IsAny<OrdenLegacy>(),
                It.Is<List<PerfilesFacturadosLegacy>>(p => p.Any(x => x.IdPerfil == 1234)),
                It.IsAny<List<ResultadosPacienteLegacy>>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        private void SetupMocks(CuentaServicios cuenta, PacienteAdmision paciente, IQueryable<ServicioClinico> servicios)
        {
            var cuentasMock = new List<CuentaServicios> { cuenta }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.CuentasServicios).Returns(cuentasMock.Object);

            var pacientesMock = new List<PacienteAdmision> { paciente }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.PacientesAdmision).Returns(pacientesMock.Object);

            var serviciosMock = servicios.BuildMockDbSet();
            _contextMock.Setup(c => c.ServiciosClinicos).Returns(serviciosMock.Object);
            
            _contextMock.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>().Object);

            _contextMock.Setup(x => x.RecibosFactura).Returns(new List<ReciboFactura>().AsQueryable().BuildMockDbSet().Object);
            _contextMock.Setup(x => x.CuentasPorCobrar).Returns(new List<CuentaPorCobrar>().AsQueryable().BuildMockDbSet().Object);
        }
    }
}
