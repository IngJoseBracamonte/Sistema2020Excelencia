using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CreateInsumoWithServiceAndNotificationTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task CrearInsumo_DebeCrearServicioClinicoConPrecioCeroDesactivadoYNotificacion()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            
            var insumoCodigo = "MED-001";
            var insumoNombre = "Atropina 1mg Injectable";
            var categoria = "Medicamento";
            var stockInicial = 10m;
            var costoUSD = 1.50m;

            var insumo = new Insumo(insumoCodigo, insumoNombre, stockInicial, UnidadMedida.UNIDAD, costoUSD, true, categoria);
            db.Insumos.Add(insumo);

            // Generar código aleatorio con prefijo
            string prefix = categoria.ToUpper().Contains("MEDICAMENTO") ? "MED-AUT-" : "INS-AUT-";
            string randomCode = $"{prefix}1234";

            var servicioClinico = new ServicioClinico(
                randomCode,
                insumoNombre,
                0m, // Sin precio base USD asignado
                "Insumo / Medicamento"
            );
            servicioClinico.Activo = false; // Desactivado por defecto
            servicioClinico.UnidadMedida = UnidadMedida.UNIDAD.ToString();
            servicioClinico.PermiteFraccionamiento = true;
            servicioClinico.RequiereInventario = true;

            db.ServiciosClinicos.Add(servicioClinico);

            var receta = new ServicioInsumoReceta(
                servicioClinico.Id,
                servicioClinico.Codigo,
                insumo.Id,
                1m,
                UnidadMedida.UNIDAD
            );
            db.ServiciosInsumoRecetas.Add(receta);

            var notification = new SistemaSatHospitalario.Core.Domain.Entities.Common.Notification(
                "Nuevo Insumo por Configurar",
                $"Se ha registrado el insumo '{insumoNombre}'. Asigne precio de venta y código final en Maestro de Servicios.",
                "Warning",
                null,
                "Administrador",
                $"/catalog?edit={servicioClinico.Id}"
            );
            db.Notifications.Add(notification);

            await db.SaveChangesAsync();

            // Assert
            var servicioEnBD = await db.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == servicioClinico.Id);
            Assert.NotNull(servicioEnBD);
            Assert.Equal(insumoNombre, servicioEnBD.Descripcion);
            Assert.Equal(0m, servicioEnBD.PrecioBase);
            Assert.False(servicioEnBD.Activo);
            Assert.StartsWith("MED-AUT-", servicioEnBD.Codigo);

            var recetaEnBD = await db.ServiciosInsumoRecetas.FirstOrDefaultAsync(r => r.ServicioClinicoId == servicioClinico.Id);
            Assert.NotNull(recetaEnBD);
            Assert.Equal(insumo.Id, recetaEnBD.InsumoId);

            var notifEnBD = await db.Notifications.FirstOrDefaultAsync(n => n.TargetRole == "Administrador");
            Assert.NotNull(notifEnBD);
            Assert.Equal("Nuevo Insumo por Configurar", notifEnBD.Title);
            Assert.Contains(servicioClinico.Id.ToString(), notifEnBD.ActionUrl);
        }
    }
}
