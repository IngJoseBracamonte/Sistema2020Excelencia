using System;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class OrdenImagenDomainTests
    {
        [Fact]
        public void OrdenImagen_Creation_ShouldSetInitialStateToPendiente()
        {
            // Arrange
            var cuentaId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();

            // Act
            var orden = new OrdenImagen(cuentaId, pacienteId, "Paciente Prueba", "RX Torax PA", "RX");

            // Assert
            Assert.Equal(EstadoOrdenImagen.Pendiente, orden.Estado);
            Assert.Equal(cuentaId, orden.CuentaId);
            Assert.Equal(pacienteId, orden.PacienteId);
            Assert.Equal("Paciente Prueba", orden.PacienteNombre);
            Assert.Equal("RX Torax PA", orden.Estudio);
            Assert.Equal("RX", orden.TipoServicio);
        }

        [Fact]
        public void MarcarComoProcesado_ShouldUpdateEstadoToProcesadoAndSetMetadata()
        {
            // Arrange
            var orden = new OrdenImagen(Guid.NewGuid(), Guid.NewGuid(), "Paciente Prueba", "TOMO Cranial", "TOMO");

            // Act
            orden.MarcarComoProcesado("usuario.tecnico");

            // Assert
            Assert.Equal(EstadoOrdenImagen.Procesado, orden.Estado);
            Assert.Equal("usuario.tecnico", orden.ProcesadoPor);
            Assert.NotNull(orden.FechaProcesado);
        }

        [Fact]
        public void EstadoOrdenImagen_EnumValues_ShouldParseCorrectly()
        {
            // Arrange & Act
            bool parsedPendiente = Enum.TryParse<EstadoOrdenImagen>("Pendiente", true, out var estadoPendiente);
            bool parsedProcesado = Enum.TryParse<EstadoOrdenImagen>("Procesado", true, out var estadoProcesado);
            bool parsedAnulado = Enum.TryParse<EstadoOrdenImagen>("Anulado", true, out var estadoAnulado);

            // Assert
            Assert.True(parsedPendiente);
            Assert.Equal(EstadoOrdenImagen.Pendiente, estadoPendiente);

            Assert.True(parsedProcesado);
            Assert.Equal(EstadoOrdenImagen.Procesado, estadoProcesado);

            Assert.True(parsedAnulado);
            Assert.Equal(EstadoOrdenImagen.Anulado, estadoAnulado);
        }
    }
}
