using System;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CompromisoPagoDomainTests
    {
        [Fact]
        public void CrearCompromisoPago_ConDatosValidos_InicializaCorrectamente()
        {
            // Arrange
            var cuentaPorCobrarId = Guid.NewGuid();
            var usuarioCreacion = "usuario_test";

            // Act
            var compromiso = new CompromisoPago(cuentaPorCobrarId, usuarioCreacion, false, null);

            // Assert
            Assert.NotEqual(Guid.Empty, compromiso.Id);
            Assert.Equal(cuentaPorCobrarId, compromiso.CuentaPorCobrarId);
            Assert.Equal(usuarioCreacion, compromiso.UsuarioCreacion);
            Assert.False(compromiso.Omitido);
            Assert.Null(compromiso.Observacion);
            Assert.True(compromiso.FechaCreacion <= DateTime.UtcNow);
        }

        [Fact]
        public void CrearCompromisoPago_SinUsuarioCreacion_LanzaArgumentNullException()
        {
            // Arrange
            var cuentaPorCobrarId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CompromisoPago(cuentaPorCobrarId, null!, false, null));
        }

        [Fact]
        public void Omitir_ConObservacionValida_ActualizaEstado()
        {
            // Arrange
            var compromiso = new CompromisoPago(Guid.NewGuid(), "usuario_test", false, null);
            var observacion = "Autorizado por junta médica";

            // Act
            compromiso.Omitir(observacion);

            // Assert
            Assert.True(compromiso.Omitido);
            Assert.Equal(observacion, compromiso.Observacion);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Omitir_SinObservacionValida_LanzaArgumentException(string? observacionInvalida)
        {
            // Arrange
            var compromiso = new CompromisoPago(Guid.NewGuid(), "usuario_test", false, null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => compromiso.Omitir(observacionInvalida));
        }
    }
}
