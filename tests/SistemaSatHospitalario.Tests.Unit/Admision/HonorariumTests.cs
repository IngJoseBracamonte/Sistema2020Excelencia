using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Application.UnitTests.Admision
{
    public class HonorariumTests
    {
        [Fact]
        public void DetalleServicioCuenta_ShouldStoreHonorarium()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var descripcion = "Consulta Especializada";
            var precio = 50.00m;
            var honorario = 15.00m;
            var usuario = "test-admin";

            // Act
            var detalle = new DetalleServicioCuenta(
                Guid.NewGuid(),
                serviceId, 
                descripcion, 
                precio, 
                honorario, 
                1, 
                "MEDICO", 
                usuario);

            // Assert
            Assert.Equal(honorario, detalle.Honorario);
            Assert.Equal(precio, detalle.Precio);
        }

        [Fact]
        public void CuentaServicios_ShouldPassHonorariumToDetails()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "test-user", "Particular");
            var serviceId = Guid.NewGuid();
            var honorarioEsperado = 12.50m;

            // Act
            var detalle = cuenta.AgregarServicio(
                serviceId, 
                "Prueba Honorario", 
                40.00m, 
                honorarioEsperado, 
                1, 
                "MEDICO", 
                "test-user");

            // Assert
            Assert.Contains(detalle, cuenta.Detalles);
            Assert.Equal(honorarioEsperado, detalle.Honorario);
        }

        [Fact]
        public void AuditLog_ShouldCaptureHonorariumChange()
        {
            // Arrange
            var detalleId = Guid.NewGuid();
            var honorarioAnterior = 10.00m;
            var nuevoHonorario = 20.00m;

            // Act
            var log = new LogAuditoriaPrecio(
                detalleId,
                "Servicio Test",
                50.00m,
                55.00m,
                honorarioAnterior,
                nuevoHonorario,
                "admin",
                "Supervisor123"
            );

            // Assert
            Assert.Equal(honorarioAnterior, log.HonorarioAnterior);
            Assert.Equal(nuevoHonorario, log.NuevoHonorario);
        }
    }
}
