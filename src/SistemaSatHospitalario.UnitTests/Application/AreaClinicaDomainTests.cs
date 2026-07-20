using System;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class AreaClinicaDomainTests
    {
        [Fact]
        public void AreaClinica_ShouldInitializeAsDisponible()
        {
            // Arrange
            var sedeId = Guid.NewGuid();
            var codigo = "AC01";
            var nombre = "Cama Hospitalización 101";

            // Act
            var area = new AreaClinica(sedeId, codigo, nombre);

            // Assert
            Assert.Equal(EstadoUbicacion.Disponible, area.Estado);
            Assert.True(area.Activo);
        }

        [Fact]
        public void AreaClinica_MarcarComoOcupada_ShouldTransitionToOcupada()
        {
            // Arrange
            var area = new AreaClinica(Guid.NewGuid(), "AC01", "Cama 101");

            // Act
            area.MarcarComoOcupada();

            // Assert
            Assert.Equal(EstadoUbicacion.Ocupada, area.Estado);
        }

        [Fact]
        public void AreaClinica_Liberar_ShouldTransitionToDisponible()
        {
            // Arrange
            var area = new AreaClinica(Guid.NewGuid(), "AC01", "Cama 101");
            area.MarcarComoOcupada();

            // Act
            area.Liberar();

            // Assert
            Assert.Equal(EstadoUbicacion.Disponible, area.Estado);
        }

        [Fact]
        public void ServicioIncluidoArea_ShouldInitializeAsActiveAndEncapsulateTransitions()
        {
            // Arrange
            var areaClinicaId = Guid.NewGuid();
            var servicioClinicoId = Guid.NewGuid();

            // Act
            var relacion = new ServicioIncluidoArea(areaClinicaId, servicioClinicoId);

            // Assert
            Assert.Equal(areaClinicaId, relacion.AreaClinicaId);
            Assert.Equal(servicioClinicoId, relacion.ServicioClinicoId);
            Assert.True(relacion.Activo);

            // Act & Assert (Desactivar)
            relacion.Desactivar();
            Assert.False(relacion.Activo);

            // Act & Assert (Activar)
            relacion.Activar();
            Assert.True(relacion.Activo);
        }
    }
}
