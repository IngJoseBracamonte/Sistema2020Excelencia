using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CategoriaInsumoUnitTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public void CategoriaInsumo_CrearConNombreValido_Exitoso()
        {
            // Arrange & Act
            var categoria = new CategoriaInsumo("  Material Quirúrgico  ", "MAT-QX");

            // Assert
            Assert.NotEqual(Guid.Empty, categoria.Id);
            Assert.Equal("Material Quirúrgico", categoria.Nombre);
            Assert.Equal("MAT-QX", categoria.Codigo);
            Assert.True(categoria.Activo);
            Assert.True(categoria.FechaCreacion <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CategoriaInsumo_NombreVacio_ThrowsArgumentException(string? nombreInvalido)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new CategoriaInsumo(nombreInvalido!));
        }

        [Fact]
        public void CategoriaInsumo_ActualizarNombre_Exitoso()
        {
            // Arrange
            var categoria = new CategoriaInsumo("Descartable");

            // Act
            categoria.ActualizarNombre("  Material Descartable  ", "DESC-01");

            // Assert
            Assert.Equal("Material Descartable", categoria.Nombre);
            Assert.Equal("DESC-01", categoria.Codigo);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CategoriaInsumo_ActualizarNombreVacio_ThrowsArgumentException(string? nombreInvalido)
        {
            // Arrange
            var categoria = new CategoriaInsumo("Medicamento");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => categoria.ActualizarNombre(nombreInvalido!));
        }

        [Fact]
        public void CategoriaInsumo_SetEstado_CambiaEstadoCorrectamente()
        {
            // Arrange
            var categoria = new CategoriaInsumo("Reactivo");
            Assert.True(categoria.Activo);

            // Act
            categoria.SetEstado(false);

            // Assert
            Assert.False(categoria.Activo);

            // Act
            categoria.SetEstado(true);

            // Assert
            Assert.True(categoria.Activo);
        }

        [Fact]
        public async Task CategoriaInsumo_GuardarEnDbContextYPropagarCambioAInsumos_Exitoso()
        {
            // Arrange
            var db = GetInMemoryDbContext();
            var cat = new CategoriaInsumo("Descartable", "DESC");
            db.CategoriasInsumo.Add(cat);

            var insumo1 = new Insumo("INS-001", "Guantes Estériles", 100, UnidadMedida.UNIDAD, 1.50m, false, "Descartable");
            var insumo2 = new Insumo("INS-002", "Jeringa 5ml", 200, UnidadMedida.UNIDAD, 0.50m, false, "Descartable");
            var insumo3 = new Insumo("MED-001", "Paracetamol", 50, UnidadMedida.UNIDAD, 2.00m, true, "Medicamento");

            db.Insumos.AddRange(insumo1, insumo2, insumo3);
            await db.SaveChangesAsync();

            // Act: Actualizar nombre de la categoría y propagar
            var nombreAnterior = cat.Nombre;
            var nuevoNombre = "Material Descartable";
            cat.ActualizarNombre(nuevoNombre, "MAT-DESC");

            var insumosAfectados = await db.Insumos.Where(i => i.Categoria == nombreAnterior).ToListAsync();
            foreach (var i in insumosAfectados)
            {
                i.ActualizarDetalles(i.Nombre, i.UnidadMedidaBase, i.CostoUnitarioBaseUSD, i.PermiteFraccionamiento, nuevoNombre);
            }
            await db.SaveChangesAsync();

            // Assert
            var catDb = await db.CategoriasInsumo.FindAsync(cat.Id);
            Assert.NotNull(catDb);
            Assert.Equal("Material Descartable", catDb.Nombre);

            var insumo1Db = await db.Insumos.FindAsync(insumo1.Id);
            var insumo2Db = await db.Insumos.FindAsync(insumo2.Id);
            var insumo3Db = await db.Insumos.FindAsync(insumo3.Id);

            Assert.Equal("Material Descartable", insumo1Db!.Categoria);
            Assert.Equal("Material Descartable", insumo2Db!.Categoria);
            Assert.Equal("Medicamento", insumo3Db!.Categoria);
        }
    }
}
