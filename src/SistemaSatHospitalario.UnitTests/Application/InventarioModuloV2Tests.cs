using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class InventarioModuloV2Tests
    {
        [Fact]
        public void Insumo_SoftDelete_MarcaIsDeletedYFechaInactivacion()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-01", "Amoxicilina 500mg", 50, UnidadMedida.UNIDAD, 2.50m);

            // Act
            insumo.SoftDelete();

            // Assert
            Assert.True(insumo.IsDeleted);
            Assert.NotNull(insumo.FechaInactivacion);
        }

        [Fact]
        public void Insumo_Restaurar_LimpiaIsDeletedYFechaInactivacion()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-01", "Amoxicilina 500mg", 50, UnidadMedida.UNIDAD, 2.50m);
            insumo.SoftDelete();

            // Act
            insumo.Restaurar();

            // Assert
            Assert.False(insumo.IsDeleted);
            Assert.Null(insumo.FechaInactivacion);
        }

        [Fact]
        public void PrincipioActivo_CrearConNombreValido_Exitoso()
        {
            // Arrange & Act
            var pa = new PrincipioActivo("Ibuprofeno");

            // Assert
            Assert.NotEqual(Guid.Empty, pa.Id);
            Assert.Equal("Ibuprofeno", pa.Nombre);
            Assert.True(pa.Activo);
        }

        [Fact]
        public void PrincipioActivo_NombreVacio_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new PrincipioActivo(null!));
            Assert.Throws<ArgumentException>(() => new PrincipioActivo(""));
        }

        [Fact]
        public void Insumo_AgregarPrincipioActivo_IncrementaColeccion()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-02", "Alivet Cap", 100, UnidadMedida.UNIDAD, 1.20m);
            var pa1 = new PrincipioActivo("Ibuprofeno");
            var pa2 = new PrincipioActivo("Clorfeniramina");

            // Act
            insumo.AgregarPrincipioActivo(pa1, "400mg");
            insumo.AgregarPrincipioActivo(pa2, "4mg");

            // Assert
            Assert.Equal(2, insumo.PrincipiosActivos.Count);
            Assert.Contains(insumo.PrincipiosActivos, p => p.PrincipioActivoId == pa1.Id && p.Concentracion == "400mg");
            Assert.Contains(insumo.PrincipiosActivos, p => p.PrincipioActivoId == pa2.Id && p.Concentracion == "4mg");
        }

        [Fact]
        public void Insumo_AgregarPrincipioActivoDuplicado_ThrowsInvalidOperationException()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-03", "Ibuprofeno Simple", 100, UnidadMedida.UNIDAD, 1.00m);
            var pa = new PrincipioActivo("Ibuprofeno");
            insumo.AgregarPrincipioActivo(pa, "400mg");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => insumo.AgregarPrincipioActivo(pa, "600mg"));
        }

        [Fact]
        public void Insumo_RemoverPrincipioActivo_DecrementaColeccion()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-04", "Alivet Cap", 100, UnidadMedida.UNIDAD, 1.20m);
            var pa = new PrincipioActivo("Ibuprofeno");
            insumo.AgregarPrincipioActivo(pa, "400mg");

            // Act
            insumo.RemoverPrincipioActivo(pa.Id);

            // Assert
            Assert.Empty(insumo.PrincipiosActivos);
        }

        [Fact]
        public void PedidoInterSedeDetalle_ObservacionDespacho_EstablecidaCorrectamente()
        {
            // Arrange
            var insumo = new Insumo("INS-V2-05", "Solucion Fisiologica", 200, UnidadMedida.ML, 3.00m);
            var detalle = new PedidoInterSedeDetalle(insumo, 10);

            // Act
            detalle.SetDespachado(5);
            detalle.SetObservacionDespacho("Stock insuficiente en almacén central, se envían solo 5 unidades");

            // Assert
            Assert.Equal(5, detalle.CantidadDespachada);
            Assert.Equal("Stock insuficiente en almacén central, se envían solo 5 unidades", detalle.ObservacionDespacho);
        }

        [Fact]
        public void EstadoPedidoInterSede_ValoresUnicosSinColision()
        {
            // Assert: Verificar que Cancelado y Rechazado no tengan el mismo valor numérico
            Assert.NotEqual((int)EstadoPedidoInterSede.Rechazado, (int)EstadoPedidoInterSede.Cancelado);
        }
    }
}
