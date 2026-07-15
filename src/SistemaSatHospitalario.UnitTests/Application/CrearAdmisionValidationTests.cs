using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using MockQueryable.Moq;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CrearAdmisionValidationTests
    {
        [Fact]
        public async Task Handle_ShouldSucceed_WhenSpecialtyIsValidForUCI()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AbrirCuentaClinicaCommandHandler>>();

            var paciente = new PacienteAdmision("12345", "Juan Pérez", "555-1234", null, DateTime.UtcNow.AddYears(-30), "Dirección de prueba");
            var especialidadIntensivista = new Especialidad("Médico Intensivista");
            var intensivista = new Medico("Dr. Carlos (UCI)", especialidadIntensivista.Id, 200m);
            typeof(Medico).GetProperty("Especialidad")?.SetValue(intensivista, especialidadIntensivista);

            mockContext.Setup(c => c.PacientesAdmision).Returns(new List<PacienteAdmision> { paciente }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.Medicos).Returns(new List<Medico> { intensivista }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios>().BuildMockDbSet().Object);

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "UCI",
                MedicoId = intensivista.Id,
                UsuarioCarga = "nurse_admin",
                PermitirBypassExcepcionMedica = false
            };

            var handler = new AbrirCuentaClinicaCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenSpecialtyIsInvalidForUCI_AndNoBypass()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AbrirCuentaClinicaCommandHandler>>();

            var paciente = new PacienteAdmision("12345", "Juan Pérez", "555-1234", null, DateTime.UtcNow.AddYears(-30), "Dirección de prueba");
            var especialidadPediatra = new Especialidad("Pediatra");
            var pediatra = new Medico("Dr. Luis (Pediatra)", especialidadPediatra.Id, 100m);
            typeof(Medico).GetProperty("Especialidad")?.SetValue(pediatra, especialidadPediatra);

            mockContext.Setup(c => c.PacientesAdmision).Returns(new List<PacienteAdmision> { paciente }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.Medicos).Returns(new List<Medico> { pediatra }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios>().BuildMockDbSet().Object);

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "UCI",
                MedicoId = pediatra.Id,
                UsuarioCarga = "nurse_admin",
                PermitirBypassExcepcionMedica = false
            };

            var handler = new AbrirCuentaClinicaCommandHandler(mockContext.Object, mockLogger.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("no es válida para el área de UCI", ex.Message);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSucceedAndLogWarning_WhenSpecialtyIsInvalidForUCI_ButBypassEnabled()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AbrirCuentaClinicaCommandHandler>>();

            var paciente = new PacienteAdmision("12345", "Juan Pérez", "555-1234", null, DateTime.UtcNow.AddYears(-30), "Dirección de prueba");
            var especialidadPediatra = new Especialidad("Pediatra");
            var pediatra = new Medico("Dr. Luis (Pediatra)", especialidadPediatra.Id, 100m);
            typeof(Medico).GetProperty("Especialidad")?.SetValue(pediatra, especialidadPediatra);

            mockContext.Setup(c => c.PacientesAdmision).Returns(new List<PacienteAdmision> { paciente }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.Medicos).Returns(new List<Medico> { pediatra }.BuildMockDbSet().Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(new List<CuentaServicios>().BuildMockDbSet().Object);

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "UCI",
                MedicoId = pediatra.Id,
                UsuarioCarga = "nurse_admin",
                PermitirBypassExcepcionMedica = true
            };

            var handler = new AbrirCuentaClinicaCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify LogWarning was called (representing critical audit log of bypass)
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ADVERTENCIA DE SEGURIDAD CLÍNICA")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
