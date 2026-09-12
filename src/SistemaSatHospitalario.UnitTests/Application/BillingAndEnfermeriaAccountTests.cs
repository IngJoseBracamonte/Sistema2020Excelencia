using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class BillingAndEnfermeriaAccountTests
    {
        [Fact]
        public void CuentaServicios_AgregarServicio_Should_Succeed_When_Estado_Is_AbiertaId()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Emergencia");
            var servicioId = Guid.NewGuid();

            // Act
            var detalle = cuenta.AgregarServicio(
                servicioId: servicioId,
                descripcion: "Atención Inicial",
                precio: 15.0m,
                honorario: 0m,
                cantidad: 1m,
                tipoServicio: "Servicio",
                usuarioAuditoriaId: Guid.NewGuid()
            );

            // Assert
            Assert.NotNull(detalle);
            Assert.Single(cuenta.Detalles);
            Assert.Equal(15.0m, cuenta.CalcularTotal());
        }

        [Fact]
        public async Task AbrirCuenta_Should_Succeed_When_ConvenioId_Is_Null()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AbrirCuentaClinicaCommandHandler>>();
            var mockCurrentUser = new Mock<ICurrentUserService>();
            var mockAreaValidation = new Mock<IAreaClinicaValidationService>();

            var pacienteId = Guid.NewGuid();
            var paciente = new PacienteAdmision(
                cedulaPasaporte: "V-12345678",
                nombreCorto: "Juan Perez",
                telefonoContact: "04141234567",
                idLegacy: 101
            );

            var pacientesList = new List<PacienteAdmision> { paciente }.BuildMockDbSet();
            var cuentasList = new List<CuentaServicios>().BuildMockDbSet();
            var areasList = new List<AreaClinica>().BuildMockDbSet();
            var preciosConvenioList = new List<PrecioServicioConvenio>().BuildMockDbSet();

            mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesList.Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);
            mockContext.Setup(c => c.PreciosServicioConvenio).Returns(preciosConvenioList.Object);

            mockCurrentUser.Setup(u => u.UserId).Returns(Guid.NewGuid());

            var handler = new AbrirCuentaClinicaCommandHandler(
                mockContext.Object,
                mockLogger.Object,
                mockCurrentUser.Object,
                mockAreaValidation.Object
            );

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Emergencia",
                ConvenioId = null, // Particular sin convenio
                AreaClinicaId = null,
                MedicoId = null
            };

            // Act
            var cuentaId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, cuentaId);
            mockContext.Verify(c => c.CuentasServicios.AddAsync(It.IsAny<CuentaServicios>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCuentasAdministrativas_Should_Handle_Null_Navigations_Gracefully()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var paciente = new PacienteAdmision(
                cedulaPasaporte: "V-87654321",
                nombreCorto: "Maria Gonzalez",
                telefonoContact: null,
                idLegacy: 102
            );

            var cuenta = new CuentaServicios(paciente.Id, "Emergencia");
            // Navegaciones intencionalmente nulas para validar resiliencia
            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var recibosList = new List<ReciboFactura>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.RecibosFactura).Returns(recibosList.Object);

            var handler = new GetCuentasAdministrativasQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(new GetCuentasAdministrativasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var item = result[0];
            Assert.Equal(cuenta.Id, item.CuentaId);
            Assert.False(string.IsNullOrEmpty(item.Estado));
            Assert.Equal("Emergencia", item.TipoIngreso);
        }

        [Fact]
        public async Task GetCuentasAdministrativasQuery_Should_Order_Accounts_By_FechaCarga_Descending()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var pacId = Guid.NewGuid();
            var paciente = new PacienteAdmision(
                cedulaPasaporte: "V-12345678",
                nombreCorto: "Jose Perez",
                telefonoContact: "04141234567",
                idLegacy: 102
            );

            var cuentaAntigua = new CuentaServicios(pacId, "Emergencia");
            var cuentaReciente = new CuentaServicios(pacId, "Emergencia");

            typeof(CuentaServicios).GetProperty(nameof(CuentaServicios.FechaCarga))!
                .SetValue(cuentaAntigua, DateTime.UtcNow.AddHours(-5));
            typeof(CuentaServicios).GetProperty(nameof(CuentaServicios.FechaCarga))!
                .SetValue(cuentaReciente, DateTime.UtcNow);

            var cuentasList = new List<CuentaServicios> { cuentaAntigua, cuentaReciente }.BuildMockDbSet();
            var recibosList = new List<ReciboFactura>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.RecibosFactura).Returns(recibosList.Object);

            var handler = new GetCuentasAdministrativasQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(new GetCuentasAdministrativasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(cuentaReciente.Id, result[0].CuentaId);
            Assert.Equal(cuentaAntigua.Id, result[1].CuentaId);
        }
    }
}
