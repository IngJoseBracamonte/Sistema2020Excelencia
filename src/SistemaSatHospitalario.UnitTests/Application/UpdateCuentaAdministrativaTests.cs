using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class UpdateCuentaAdministrativaTests
    {
        [Fact]
        public async Task Handle_ShouldModifyAccountAndAddAuditHistory()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockTransaction = new Mock<IDbContextTransaction>();

            mockContext.Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockTransaction.Object);

            var accountId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var newPatientId = Guid.NewGuid();
            var serviceDetailId = Guid.NewGuid();

            var patient = new PacienteAdmision("12345", "Juan Perez", "04141234567");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(patient, patientId);

            var targetPatient = new PacienteAdmision("67890", "Pedro Gomez", "04147654321");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(targetPatient, newPatientId);

            var patientsList = new List<PacienteAdmision> { patient, targetPatient }.BuildMockDbSet<PacienteAdmision>();
            mockContext.Setup(c => c.PacientesAdmision).Returns(patientsList.Object);

            var convenio = new SeguroConvenio("Seguro Alfa", "Alpha-001", "Direccion", "12345", "alpha@test.com");
            typeof(SeguroConvenio).GetProperty("Id")?.SetValue(convenio, 1);

            var conveniosList = new List<SeguroConvenio> { convenio }.BuildMockDbSet<SeguroConvenio>();
            mockContext.Setup(c => c.SegurosConvenios).Returns(conveniosList.Object);

            var account = new CuentaServicios(patientId, "Admin", EstadoConstants.Particular);
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(account, accountId);
            
            var detail = account.AgregarServicio(Guid.NewGuid(), "Consulta General", 50m, 10m, 1, EstadoConstants.Medico, "Admin");
            typeof(DetalleServicioCuenta).GetProperty("Id")?.SetValue(detail, serviceDetailId);

            var accountsList = new List<CuentaServicios> { account }.BuildMockDbSet<CuentaServicios>();
            mockContext.Setup(c => c.CuentasServicios).Returns(accountsList.Object);

            var recibosList = new List<ReciboFactura>().BuildMockDbSet<ReciboFactura>();
            mockContext.Setup(c => c.RecibosFactura).Returns(recibosList.Object);

            var cxcList = new List<CuentaPorCobrar>().BuildMockDbSet<CuentaPorCobrar>();
            mockContext.Setup(c => c.CuentasPorCobrar).Returns(cxcList.Object);

            var historyList = new Mock<DbSet<HistorialModificacionCuenta>>();
            mockContext.Setup(c => c.HistorialModificacionCuentas).Returns(historyList.Object);

            var citas = new List<CitaMedica>().BuildMockDbSet<CitaMedica>();
            mockContext.Setup(c => c.CitasMedicas).Returns(citas.Object);

            var handler = new UpdateCuentaAdministrativaCommandHandler(mockContext.Object);

            var command = new UpdateCuentaAdministrativaCommand
            {
                CuentaId = accountId,
                NuevoPacienteId = newPatientId,
                NuevoTipoIngreso = EstadoConstants.Seguro,
                NuevoConvenioId = 1,
                CorreccionesPrecios = new List<DetallePrecioCorreccionDto>
                {
                    new DetallePrecioCorreccionDto { DetalleId = serviceDetailId, NuevoPrecio = 60m, NuevoHonorario = 15m }
                },
                UsuarioModificacion = "SupervisorAdmin"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(newPatientId, account.PacienteId);
            Assert.Equal(EstadoConstants.Seguro, account.TipoIngreso);
            Assert.Equal(1, account.ConvenioId);
            Assert.Equal(60m, detail.Precio);
            Assert.Equal(15m, detail.Honorario);

            historyList.Verify(m => m.Add(It.Is<HistorialModificacionCuenta>(h => 
                h.CuentaServicioId == accountId &&
                h.Usuario == "SupervisorAdmin" &&
                h.PacienteAnteriorId == patientId &&
                h.PacienteNuevoId == newPatientId &&
                h.TipoIngresoAnterior == EstadoConstants.Particular &&
                h.TipoIngresoNuevo == EstadoConstants.Seguro &&
                h.ConvenioNuevoId == 1 &&
                h.TotalAnteriorUSD == 50m &&
                h.TotalNuevoUSD == 60m
            )), Times.Once);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
