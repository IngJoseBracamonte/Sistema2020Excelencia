using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class FastChargeManualPriceTests
    {
        [Fact]
        public async Task CargarServicio_WithManualPrice_ShouldOverrideStandardTariff()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockBillingRepo = new Mock<IBillingRepository>();
            var mockExternaService = new Mock<IOrdenExternaService>();
            var mockMapper = new Mock<IHonorariumMapperService>();
            var mockLogger = new Mock<ILogger<CargarServicioACuentaCommandHandler>>();
            var mockInvLogger = new Mock<ILogger<InventoryService>>();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, null, "Direccion");
            var doctor = new Medico("Dr. Jose", Guid.NewGuid(), 80);

            var servicioConsulta = new ServicioClinico("CON-01", "Consulta General", 50, "Medico")
            {
                PrecioBase = 50,
                HonorarioBase = 30
            };

            var cuenta = new CuentaServicios(paciente.Id, "Nurse", "Emergencia");

            var pacientesList = new List<PacienteAdmision> { paciente };
            var pacientesMock = pacientesList.BuildMockDbSet<PacienteAdmision>();

            var serviciosList = new List<ServicioClinico> { servicioConsulta };
            var serviciosMock = serviciosList.BuildMockDbSet<ServicioClinico>();

            var medicosList = new List<Medico> { doctor };
            var medicosMock = medicosList.BuildMockDbSet<Medico>();

            var cuentasList = new List<CuentaServicios> { cuenta };
            var cuentasMock = cuentasList.BuildMockDbSet<CuentaServicios>();

            var logAsignaciones = new List<LogAsignacionHonorario>();
            var logsMock = logAsignaciones.BuildMockDbSet<LogAsignacionHonorario>();

            var detallesCuentasList = new List<DetalleServicioCuenta>();
            var detallesCuentasMock = detallesCuentasList.BuildMockDbSet<DetalleServicioCuenta>();

            var recetasMock = new List<ServicioInsumoReceta>().BuildMockDbSet<ServicioInsumoReceta>();

            mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesMock.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosMock.Object);
            mockContext.Setup(c => c.Medicos).Returns(medicosMock.Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasMock.Object);
            mockContext.Setup(c => c.LogsAsignacionHonorario).Returns(logsMock.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesCuentasMock.Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetasMock.Object);

            mockBillingRepo.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(paciente.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuenta);

            var inventoryService = new InventoryService(mockContext.Object, mockInvLogger.Object);
            var handler = new CargarServicioACuentaCommandHandler(
                mockBillingRepo.Object, mockExternaService.Object, mockContext.Object, mockMapper.Object, inventoryService, mockLogger.Object);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicioConsulta.Id.ToString(),
                Descripcion = servicioConsulta.Descripcion,
                Precio = 50,
                Honorario = 30,
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "Nurse",
                TipoIngreso = "Emergencia",
                MedicoId = doctor.Id,
                HoraCita = DateTime.Now,
                PrecioModificado = 99.99m,      // Manual Override
                HonorarioModificado = 75.00m,   // Manual Override
                IsPrivilegedUser = true
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var addedDetail = cuenta.Detalles.FirstOrDefault();
            Assert.NotNull(addedDetail);
            Assert.Equal(99.99m, addedDetail.Precio);
            Assert.Equal(75.00m, addedDetail.Honorario);
        }

        [Fact]
        public async Task CargarServicio_WithoutManualPrice_ShouldApplyStandardTariff()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockBillingRepo = new Mock<IBillingRepository>();
            var mockExternaService = new Mock<IOrdenExternaService>();
            var mockMapper = new Mock<IHonorariumMapperService>();
            var mockLogger = new Mock<ILogger<CargarServicioACuentaCommandHandler>>();
            var mockInvLogger = new Mock<ILogger<InventoryService>>();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, null, "Direccion");
            var doctor = new Medico("Dr. Jose", Guid.NewGuid(), 80);

            var servicioConsulta = new ServicioClinico("CON-01", "Consulta General", 50, "Medico")
            {
                PrecioBase = 50,
                HonorarioBase = 30
            };

            var cuenta = new CuentaServicios(paciente.Id, "Nurse", "Emergencia");

            var pacientesList = new List<PacienteAdmision> { paciente };
            var pacientesMock = pacientesList.BuildMockDbSet<PacienteAdmision>();

            var serviciosList = new List<ServicioClinico> { servicioConsulta };
            var serviciosMock = serviciosList.BuildMockDbSet<ServicioClinico>();

            var medicosList = new List<Medico> { doctor };
            var medicosMock = medicosList.BuildMockDbSet<Medico>();

            var cuentasList = new List<CuentaServicios> { cuenta };
            var cuentasMock = cuentasList.BuildMockDbSet<CuentaServicios>();

            var logAsignaciones = new List<LogAsignacionHonorario>();
            var logsMock = logAsignaciones.BuildMockDbSet<LogAsignacionHonorario>();

            var detallesCuentasList = new List<DetalleServicioCuenta>();
            var detallesCuentasMock = detallesCuentasList.BuildMockDbSet<DetalleServicioCuenta>();

            var recetasMock = new List<ServicioInsumoReceta>().BuildMockDbSet<ServicioInsumoReceta>();

            mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesMock.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosMock.Object);
            mockContext.Setup(c => c.Medicos).Returns(medicosMock.Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasMock.Object);
            mockContext.Setup(c => c.LogsAsignacionHonorario).Returns(logsMock.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesCuentasMock.Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetasMock.Object);

            mockBillingRepo.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(paciente.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuenta);

            var inventoryService = new InventoryService(mockContext.Object, mockInvLogger.Object);
            var handler = new CargarServicioACuentaCommandHandler(
                mockBillingRepo.Object, mockExternaService.Object, mockContext.Object, mockMapper.Object, inventoryService, mockLogger.Object);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicioConsulta.Id.ToString(),
                Descripcion = servicioConsulta.Descripcion,
                Precio = 50,
                Honorario = 0,
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "Nurse",
                TipoIngreso = "Emergencia",
                MedicoId = doctor.Id,
                HoraCita = DateTime.Now,
                PrecioModificado = null,      // No Override
                HonorarioModificado = null,   // No Override
                IsPrivilegedUser = true
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var addedDetail = cuenta.Detalles.FirstOrDefault();
            Assert.NotNull(addedDetail);
            // PrecioBase (50) + doctor.HonorarioBase (80) = 130
            Assert.Equal(130.00m, addedDetail.Precio);
            Assert.Equal(80.00m, addedDetail.Honorario);
        }
    }
}
