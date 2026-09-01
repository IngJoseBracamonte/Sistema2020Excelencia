using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class DeductStockPerAreaUnitTests
    {
        private DbContextOptions<SatHospitalarioDbContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private async Task<(SatHospitalarioDbContext db, Insumo insumo, ServicioClinico servicio, PacienteAdmision paciente, Sede sede, AreaClinica area)> SetupAreaEnvironment(
            string areaCodigo,
            string areaNombre,
            Guid sedeId,
            decimal stockInicial)
        {
            var options = GetInMemoryOptions();
            var db = new SatHospitalarioDbContext(options);

            // 1. Sede y Área Clínica
            var sede = new Sede($"Sede {areaNombre}", areaNombre, true);
            typeof(Sede).GetProperty("Id")!.SetValue(sede, sedeId);
            await db.Sedes.AddAsync(sede);

            var area = new AreaClinica(sedeId, areaCodigo, areaNombre, true);
            await db.AreasClinicas.AddAsync(area);

            // 2. Insumo / Medicamento y Stock en esa Sede
            var insumo = new Insumo($"INS-{areaCodigo}", $"MEDICAMENTO {areaNombre}", 0, UnidadMedida.UNIDAD, 5.00m, true, "Medicamento");
            await db.Insumos.AddAsync(insumo);

            var stockSede = new StockSede(insumo.Id, sedeId, stockInicial);
            await db.StocksSedes.AddAsync(stockSede);

            // 3. Servicio Clínico + Receta / Insumo Asociado
            var servicio = new ServicioClinico($"SVC-{areaCodigo}", $"APLICACIÓN MEDICAMENTO {areaNombre}", 10.00m, "MEDICO");
            servicio.RequiereInventario = true;
            await db.ServiciosClinicos.AddAsync(servicio);

            var receta = new ServicioInsumoReceta(servicio.Id, insumo.Id, 2m, UnidadMedida.UNIDAD);
            receta.Insumo = insumo;
            await db.ServiciosInsumoRecetas.AddAsync(receta);

            // 4. Paciente
            var paciente = new PacienteAdmision("V-11111111", $"PACIENTE {areaNombre}", "04140000000", 100, DateTime.Now.AddYears(-30), "Caracas");
            await db.PacientesAdmision.AddAsync(paciente);

            await db.SaveChangesAsync();

            return (db, insumo, servicio, paciente, sede, area);
        }

        private CargarServicioACuentaCommandHandler CreateHandler(SatHospitalarioDbContext db)
        {
            var mockBillingRepo = new Mock<IBillingRepository>();
            var mockExternaService = new Mock<IOrdenExternaService>();
            var mockMapper = new Mock<IHonorariumMapperService>();
            var mockLegacyRepo = new Mock<ILegacyLabRepository>();
            var mockLogger = new Mock<ILogger<CargarServicioACuentaCommandHandler>>();
            var mockInvLogger = new Mock<ILogger<InventoryService>>();

            var inventoryService = new InventoryService(db, mockInvLogger.Object);

            return new CargarServicioACuentaCommandHandler(
                mockBillingRepo.Object,
                mockExternaService.Object,
                db,
                mockMapper.Object,
                inventoryService,
                mockLegacyRepo.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public async Task CargarMedicamento_EnAreaEmergencia_DescuentaStockExclusivoDeSedeEmergencia()
        {
            // Arrange
            var (db, insumo, servicio, paciente, sede, area) = await SetupAreaEnvironment(
                "EMERGENCIA", "EMERGENCIA", SeedConstants.SedeId_Emergencia, 50m);

            var handler = CreateHandler(db);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicio.Id.ToString(),
                Cantidad = 1m,
                Precio = 10m,
                TipoServicio = "FARMA",
                UsuarioCarga = "enfermero_emergencia",
                AreaClinicaId = area.Id
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            // Verificar que el stock fue descontado específicamente de SedeId_Emergencia (50 - 2 = 48)
            var stockEnBD = await db.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Emergencia);
            Assert.NotNull(stockEnBD);
            Assert.Equal(48m, stockEnBD.StockActual);
        }

        [Fact]
        public async Task CargarMedicamento_EnAreaHospitalizacion_DescuentaStockExclusivoDeSedeHospitalizacion()
        {
            // Arrange
            var (db, insumo, servicio, paciente, sede, area) = await SetupAreaEnvironment(
                "HOSPITALIZACION", "HOSPITALIZACIÓN", SeedConstants.SedeId_Hospitalizacion, 30m);

            var handler = CreateHandler(db);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicio.Id.ToString(),
                Cantidad = 2m,
                Precio = 10m,
                TipoServicio = "FARMA",
                UsuarioCarga = "enfermero_hosp",
                AreaClinicaId = area.Id
            };

            // Act: Cargar 2 unidades (2 * 2 insumos de receta = 4 insumos descontados)
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            // Stock descontado de SedeId_Hospitalizacion: 30 - 4 = 26
            var stockEnBD = await db.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Hospitalizacion);
            Assert.NotNull(stockEnBD);
            Assert.Equal(26m, stockEnBD.StockActual);
        }

        [Fact]
        public async Task CargarMedicamento_EnAreaUCI_DescuentaStockExclusivoDeSedeUCI()
        {
            // Arrange
            var (db, insumo, servicio, paciente, sede, area) = await SetupAreaEnvironment(
                "UCI", "UNIDAD DE CUIDADOS INTENSIVOS", SeedConstants.SedeId_UCI, 20m);

            var handler = CreateHandler(db);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicio.Id.ToString(),
                Cantidad = 1m,
                Precio = 10m,
                TipoServicio = "FARMA",
                UsuarioCarga = "enfermero_uci",
                AreaClinicaId = area.Id
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            // Stock descontado de SedeId_UCI: 20 - 2 = 18
            var stockEnBD = await db.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_UCI);
            Assert.NotNull(stockEnBD);
            Assert.Equal(18m, stockEnBD.StockActual);
        }

        [Fact]
        public async Task CargarMedicamento_EnAreaCirugia_DescuentaStockExclusivoDeSedeCirugia()
        {
            // Arrange
            var (db, insumo, servicio, paciente, sede, area) = await SetupAreaEnvironment(
                "CIRUGIA", "QUIRÓFANO Y CIRUGÍA", SeedConstants.SedeId_Cirugia, 40m);

            var handler = CreateHandler(db);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicio.Id.ToString(),
                Cantidad = 3m,
                Precio = 10m,
                TipoServicio = "FARMA",
                UsuarioCarga = "instrumentista_quirofano",
                AreaClinicaId = area.Id
            };

            // Act: Cargar 3 unidades (3 * 2 insumos = 6 descontados)
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            // Stock descontado de SedeId_Cirugia: 40 - 6 = 34
            var stockEnBD = await db.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Cirugia);
            Assert.NotNull(stockEnBD);
            Assert.Equal(34m, stockEnBD.StockActual);
        }

        [Fact]
        public async Task CargarInsumoDirecto_EnAreaHospitalizacion_DescuentaStockDirectoDeSedeHospitalizacion()
        {
            // Arrange: Insumo directo sin receta en ServiciosInsumoRecetas
            var options = GetInMemoryOptions();
            var db = new SatHospitalarioDbContext(options);

            var sede = new Sede("Sede Hospitalizacion", "HOSPITALIZACION", true);
            typeof(Sede).GetProperty("Id")!.SetValue(sede, SeedConstants.SedeId_Hospitalizacion);
            await db.Sedes.AddAsync(sede);

            var area = new AreaClinica(SeedConstants.SedeId_Hospitalizacion, "HOSP", "HOSPITALIZACION", true);
            await db.AreasClinicas.AddAsync(area);

            var insumo = new Insumo("INS-BIS-15", "BISTURI 15", 0, UnidadMedida.UNIDAD, 10.00m, false, "Insumo");
            await db.Insumos.AddAsync(insumo);

            var stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Hospitalizacion, 500m);
            await db.StocksSedes.AddAsync(stockSede);

            var paciente = new PacienteAdmision("V-22222222", "PACIENTE TEST", "04140000000", 100, DateTime.Now.AddYears(-25), "Merida");
            await db.PacientesAdmision.AddAsync(paciente);
            await db.SaveChangesAsync();

            var handler = CreateHandler(db);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = insumo.Id.ToString(), // Direct Insumo GUID
                Descripcion = insumo.Nombre,
                Cantidad = 5m,
                Precio = 10m,
                TipoServicio = "Insumo",
                UsuarioCarga = "enfermera_hospitalizacion",
                AreaClinicaId = area.Id,
                OrigenCarga = "Hospitalizacion"
            };

            // Act: Cargar 5 unidades de insumo directo
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            // Stock descontado de SedeId_Hospitalizacion: 500 - 5 = 495
            var stockEnBD = await db.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Hospitalizacion);
            Assert.NotNull(stockEnBD);
            Assert.Equal(495m, stockEnBD.StockActual);
        }
    }
}
