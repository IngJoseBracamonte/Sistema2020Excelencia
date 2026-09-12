using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ExpedienteFacturacionTests
    {
        [Fact]
        public async Task Handle_Should_Not_Throw_NullReferenceException_When_ReciboFactura_Or_Navigations_Are_Null()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockIdentity = new Mock<IIdentityService>();

            var pacienteId = Guid.NewGuid();
            var paciente = new PacienteAdmision(
                cedulaPasaporte: "V-99999999",
                nombreCorto: "Paciente Test",
                telefonoContact: "04120000000",
                idLegacy: 1
            );
            typeof(PacienteAdmision).GetProperty("Id")!.SetValue(paciente, pacienteId);

            var cuentaId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Emergencia");
            typeof(CuentaServicios).GetProperty("Id")!.SetValue(cuenta, cuentaId);
            typeof(CuentaServicios).GetProperty("ConvenioId")!.SetValue(cuenta, 1);

            var detalle = cuenta.AgregarServicio(
                servicioId: Guid.NewGuid(),
                descripcion: "Consulta General",
                precio: 20m,
                honorario: 10m,
                cantidad: 1m,
                tipoServicio: "Consulta",
                usuarioAuditoriaId: Guid.NewGuid()
            );
            typeof(DetalleServicioCuenta).GetProperty("CuentaServicioId")!.SetValue(detalle, cuentaId);
            typeof(DetalleServicioCuenta).GetProperty("FechaCarga")!.SetValue(detalle, DateTime.UtcNow);
            typeof(DetalleServicioCuenta).GetProperty("TipoServicioId")!.SetValue(detalle, 1);

            var convenio = new SeguroConvenio("Seguro Test", null, null, null, null);
            typeof(SeguroConvenio).GetProperty("Id")!.SetValue(convenio, 1);

            var detallesList = new List<DetalleServicioCuenta> { detalle }.BuildMockDbSet();
            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var pacientesList = new List<PacienteAdmision> { paciente }.BuildMockDbSet();
            var recibosList = new List<ReciboFactura>().BuildMockDbSet();
            var conveniosList = new List<SeguroConvenio> { convenio }.BuildMockDbSet();
            var cxcList = new List<CuentaPorCobrar>().BuildMockDbSet();
            var citasList = new List<CitaMedica>().BuildMockDbSet();
            var cajasList = new List<CajaDiaria>().BuildMockDbSet();
            var pagosList = new List<DetallePago>().BuildMockDbSet();
            var configList = new List<ConfiguracionGeneral>().BuildMockDbSet();

            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesList.Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesList.Object);
            mockContext.Setup(c => c.RecibosFactura).Returns(recibosList.Object);
            mockContext.Setup(c => c.SegurosConvenios).Returns(conveniosList.Object);
            mockContext.Setup(c => c.CuentasPorCobrar).Returns(cxcList.Object);
            mockContext.Setup(c => c.CitasMedicas).Returns(citasList.Object);
            mockContext.Setup(c => c.CajasDiarias).Returns(cajasList.Object);
            mockContext.Setup(c => c.DetallesPago).Returns(pagosList.Object);
            mockContext.Setup(c => c.ConfiguracionGeneral).Returns(configList.Object);

            mockIdentity.Setup(i => i.GetUsersAsync())
                .ReturnsAsync(new List<UserDto>());

            var handler = new GetExpedienteFacturacionQueryHandler(mockContext.Object, mockIdentity.Object);

            var query = new GetExpedienteFacturacionQuery
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                FilterType = "todo"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Consulta General", result[0].Estudio);
            Assert.Equal("V-99999999", result[0].PacienteCedula);
        }
    }
}
