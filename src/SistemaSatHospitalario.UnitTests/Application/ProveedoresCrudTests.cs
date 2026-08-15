using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ProveedoresCrudTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task CrearProveedor_DebePersistirCorrectamenteConRifYRazonSocial()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var rif = "J-12345678-9";
            var razonSocial = "DROGUERÍA NENA C.A.";
            var direccion = "Av. Principal Zona Industrial, Caracas";
            var telefono = "0212-5551234";

            var proveedor = new Proveedor(rif, razonSocial, direccion, telefono);
            db.Proveedores.Add(proveedor);
            await db.SaveChangesAsync();

            // Act
            var provEnBD = await db.Proveedores.FirstOrDefaultAsync(p => p.Id == proveedor.Id);

            // Assert
            Assert.NotNull(provEnBD);
            Assert.Equal("J-12345678-9", provEnBD.RIF);
            Assert.Equal(razonSocial, provEnBD.RazonSocial);
            Assert.Equal(direccion, provEnBD.Direccion);
            Assert.Equal(telefono, provEnBD.Telefono);
            Assert.True(provEnBD.Activo);
        }

        [Fact]
        public async Task RegistrarOrdenCompraConProveedorId_DebeVincularAmbasEntidades()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var proveedor = new Proveedor("J-98765432-1", "FARMACEUTICA VENEZUELA C.A.");
            db.Proveedores.Add(proveedor);
            await db.SaveChangesAsync();

            var ordenCompra = new OrdenCompraInventario(
                "FAC-100200",
                proveedor.RazonSocial,
                DateTime.Now,
                150.00m,
                50.00m,
                proveedor.Id,
                "Compra probada en unit test"
            );
            db.OrdenesCompraInventario.Add(ordenCompra);
            await db.SaveChangesAsync();

            // Act
            var ordenEnBD = await db.OrdenesCompraInventario.FirstOrDefaultAsync(o => o.Id == ordenCompra.Id);

            // Assert
            Assert.NotNull(ordenEnBD);
            Assert.Equal(proveedor.Id, ordenEnBD.ProveedorId);
            Assert.Equal(proveedor.RazonSocial, ordenEnBD.ProveedorNombre);
        }
    }
}
