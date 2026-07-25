using System;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ServicioCatalogCrudTests
    {
        [Fact]
        public void TipoServicioConstants_Informe_ShouldBeEqualToSix()
        {
            // Assert
            Assert.Equal(6, TipoServicioConstants.Informe);
        }

        [Fact]
        public void ServicioClinico_Desactivar_ShouldPerformSoftDeleteWithUserAudit()
        {
            // Arrange
            var servicio = new ServicioClinico("RX01", "RX de Tórax", 25.00m, "RX");
            string usuarioAuditoria = "admin_test";

            // Act
            servicio.Desactivar(usuarioAuditoria);

            // Assert
            Assert.False(servicio.Activo);
            Assert.Equal(usuarioAuditoria, servicio.DesactivadoPorUsuarioId);
            Assert.NotNull(servicio.FechaDesactivacion);
        }

        [Fact]
        public void ServicioClinico_TipoInformeWithPriceLessThanHonorarium_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var servicioInforme = new ServicioClinico("INF01", "Informe RX Tórax", 10.00m, "INFORME")
            {
                TipoServicioId = TipoServicioConstants.Informe,
                HonorarioBase = 15.00m // Honorario ($15) mayor a PrecioBase ($10) -> Inválido
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => servicioInforme.ValidarInvariantes());
            Assert.Contains("El precio base del informe ($10.00) no puede ser inferior al honorario asignado al médico ($15.00)", ex.Message);
        }

        [Fact]
        public void ServicioClinico_TipoInformeWithValidPriceAndHonorarium_ShouldPassValidation()
        {
            // Arrange
            var servicioInforme = new ServicioClinico("INF01", "Informe RX Tórax", 20.00m, "INFORME")
            {
                TipoServicioId = TipoServicioConstants.Informe,
                HonorarioBase = 15.00m // Honorario ($15) menor a PrecioBase ($20) -> Válido
            };

            // Act & Assert (Sin excepción)
            servicioInforme.ValidarInvariantes();
            Assert.Equal(6, servicioInforme.TipoServicioId);
        }

        [Fact]
        public void ServicioClinico_LinkServicioInforme_ShouldSetServicioInformeIdCorrectly()
        {
            // Arrange
            var servicioBase = new ServicioClinico("RX01", "RX de Tórax", 25.00m, "RX");
            Guid informeId = Guid.NewGuid();

            // Act
            servicioBase.ServicioInformeId = informeId;

            // Assert
            Assert.Equal(informeId, servicioBase.ServicioInformeId);
        }
    }
}
