using System;
using System.Collections.Generic;
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
                _billingRepoMock.Object
            );
        }

        [Fact]
        public async Task Should_CallLegacySync_When_LabItemsExist_And_PatientHasLegacyId()
        {
            // Arrange
            var paciente = new PacienteAdmision("12345", "Test Patient", "555-000", 101);
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");
            
            var servicioId = Guid.NewGuid();
            cuenta.AgregarServicio(servicioId, "Perfil Lipidico", 50, 1, EstadoConstants.Laboratorio, "Admin");

            var servicios = new List<ServicioClinico> 
            { 
                new ServicioClinico("123", "Perfil Lipidico", 50, EstadoConstants.Laboratorio)
            };
            // Note: In real scenarios, the ID in account must match the service. 
            // Since we mock the context, we'll ensure the IDs align in the setup.
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(servicios[0], servicioId);

            SetupMocks(cuenta, paciente, servicios.AsQueryable());

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
            cuenta.AgregarServicio(servicioId, "Perfil ABC", 50, 1, EstadoConstants.Laboratorio, "Admin");

            var servicios = new List<ServicioClinico> 
            { 
                new ServicioClinico("PERF-999", "Perfil ABC", 50, EstadoConstants.Laboratorio)
            };
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(servicios[0], servicioId);

            SetupMocks(cuenta, paciente, servicios.AsQueryable());

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
