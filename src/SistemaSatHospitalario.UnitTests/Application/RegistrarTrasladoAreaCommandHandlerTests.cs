using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class RegistrarTrasladoAreaCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Update_TipoIngresoId_To_Hospitalizacion_When_Transferring_To_Hospitalizacion()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(u => u.UserId).Returns(Guid.NewGuid());

            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Emergencia");
            Assert.Equal(TipoIngresoConstants.EmergenciaId, cuenta.TipoIngresoId);

            var camaDestino = new AreaClinica(Guid.NewGuid(), "HAB-101", "Habitación 101");

            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var areasList = new List<AreaClinica> { camaDestino }.BuildMockDbSet();
            var serviciosList = new List<ServicioClinico>().BuildMockDbSet();
            var detallesList = new List<DetalleServicioCuenta>().BuildMockDbSet();
            var auditLogsList = new List<AuditLog>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesList.Object);
            mockContext.Setup(c => c.AuditLogs).Returns(auditLogsList.Object);

            var handler = new RegistrarTrasladoAreaCommandHandler(mockContext.Object, mockUserService.Object);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "Hospitalización",
                CamaDestinoId = camaDestino.Id,
                CantidadHoras = 24,
                MontoACobrarUsd = 100m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TipoIngresoConstants.HospitalizacionId, cuenta.TipoIngresoId);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Update_TipoIngresoId_To_UCI_When_Transferring_To_UCI()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(u => u.UserId).Returns(Guid.NewGuid());

            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Hospitalizacion");

            var camaDestino = new AreaClinica(Guid.NewGuid(), "UCI-1", "Cama UCI 1");

            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var areasList = new List<AreaClinica> { camaDestino }.BuildMockDbSet();
            var serviciosList = new List<ServicioClinico>().BuildMockDbSet();
            var detallesList = new List<DetalleServicioCuenta>().BuildMockDbSet();
            var auditLogsList = new List<AuditLog>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesList.Object);
            mockContext.Setup(c => c.AuditLogs).Returns(auditLogsList.Object);

            var handler = new RegistrarTrasladoAreaCommandHandler(mockContext.Object, mockUserService.Object);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "UCI",
                CamaDestinoId = camaDestino.Id,
                CantidadHoras = 12,
                MontoACobrarUsd = 600m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TipoIngresoConstants.UciId, cuenta.TipoIngresoId);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Resolve_CamaDestino_By_AreaDestino_When_CamaDestinoId_Is_Empty()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(u => u.UserId).Returns(Guid.NewGuid());

            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Emergencia");

            var camaHospitalizacion = new AreaClinica(Guid.NewGuid(), "HAB-101", "Habitación 101");

            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var areasList = new List<AreaClinica> { camaHospitalizacion }.BuildMockDbSet();
            var serviciosList = new List<ServicioClinico>().BuildMockDbSet();
            var detallesList = new List<DetalleServicioCuenta>().BuildMockDbSet();
            var auditLogsList = new List<AuditLog>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesList.Object);
            mockContext.Setup(c => c.AuditLogs).Returns(auditLogsList.Object);

            var handler = new RegistrarTrasladoAreaCommandHandler(mockContext.Object, mockUserService.Object);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "Habitación 101",
                CamaDestinoId = Guid.Empty, // No seleccionada explícitamente
                CantidadHoras = 24,
                MontoACobrarUsd = 100m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(camaHospitalizacion.Id, result.CamaDestinoId);
            Assert.Equal(TipoIngresoConstants.HospitalizacionId, cuenta.TipoIngresoId);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Record_AuditLog_With_SedeName_In_AreaDestino_And_BedName_In_Cama()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockUserService = new Mock<ICurrentUserService>();
            mockUserService.Setup(u => u.UserId).Returns(Guid.NewGuid());

            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Hospitalizacion");

            var sedeUci = new Sede("SEDE-UCI", "Depósito UCI", false, SeedConstants.SedeId_UCI);
            var camaUci = new AreaClinica(sedeUci.Id, "UCI-1", "Cama UCI 1");

            // Asociar la Sede a la Cama
            typeof(AreaClinica).GetProperty("Sede")?.SetValue(camaUci, sedeUci);

            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var areasList = new List<AreaClinica> { camaUci }.BuildMockDbSet();
            var sedesList = new List<Sede> { sedeUci }.BuildMockDbSet();
            var serviciosList = new List<ServicioClinico>().BuildMockDbSet();
            var detallesList = new List<DetalleServicioCuenta>().BuildMockDbSet();
            var auditLogsList = new List<AuditLog>().BuildMockDbSet();

            AuditLog? capturedAudit = null;
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);
            mockContext.Setup(c => c.Sedes).Returns(sedesList.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesList.Object);
            mockContext.Setup(c => c.AuditLogs).Returns(auditLogsList.Object);

            mockContext.Setup(c => c.AuditLogs.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
                .Callback<AuditLog, CancellationToken>((log, _) => capturedAudit = log)
                .Returns(new System.Threading.Tasks.ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditLog>>((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditLog>)null!));

            var handler = new RegistrarTrasladoAreaCommandHandler(mockContext.Object, mockUserService.Object);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "Cama UCI 1", // Pasado por error o legacy desde frontend
                CamaDestinoId = camaUci.Id,
                CantidadHoras = 12,
                MontoACobrarUsd = 0m,
                UsuarioTraslado = "admin"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(capturedAudit);
            Assert.Contains("AreaDestino: Depósito UCI", capturedAudit!.NewValue);
            Assert.Contains("Cama: Cama UCI 1", capturedAudit.NewValue);
            Assert.DoesNotContain(camaUci.Id.ToString(), capturedAudit.NewValue);
            Assert.Equal("Depósito UCI", cuenta.SubAreaClinica);
        }
    }
}
