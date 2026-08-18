using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetUnifiedCatalogQueryTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SatHospitalarioDbContext(options);
            return context;
        }

        [Fact]
        public async Task Should_IncludeActiveInsumos_In_UnifiedCatalog_When_Queried()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // 1. Tasa de cambio
            context.TasaCambio.Add(new TasaCambio(36.5m));

            // 2. Servicio nativo
            var consulta = new ServicioClinico("MED-001", "Consulta General", 50.00m, "MEDICO")
            {
                Activo = true,
                Category = ServiceCategory.Consultation,
                TipoServicioId = TipoServicioConstants.Medico
            };
            context.ServiciosClinicos.Add(consulta);

            // 3. Insumos directos en inventario
            var insumo1 = new Insumo("INS-BIS-15", "BISTURI 15", 500, UnidadMedida.UNIDAD, 10.00m, permiteFraccionamiento: false, categoria: "Insumo");
            var insumo2 = new Insumo("MED-ATR-01", "ATROPINA", 500, UnidadMedida.UNIDAD, 15.00m, permiteFraccionamiento: true, categoria: "Medicamento");
            context.Insumos.AddRange(insumo1, insumo2);

            await context.SaveChangesAsync();

            var legacyRepoMock = new Mock<ILegacyLabRepository>();
            legacyRepoMock.Setup(r => r.GetAvailableProfilesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PerfilLegacy>());

            var handler = new GetUnifiedCatalogQueryHandler(
                context,
                legacyRepoMock.Object,
                NullLogger<GetUnifiedCatalogQueryHandler>.Instance
            );

            // Act
            var result = await handler.Handle(new GetUnifiedCatalogQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, item => item.Codigo == "MED-001");
            
            var bis15 = result.FirstOrDefault(item => item.Codigo == "INS-BIS-15");
            Assert.NotNull(bis15);
            Assert.Equal("BISTURI 15", bis15.Descripcion);
            Assert.Equal("MEDICAMENTO", bis15.EditorType);
            Assert.Equal(TipoServicioConstants.Insumo, bis15.TipoServicioId);
            Assert.Equal(10.00m, bis15.PrecioUsd);
            Assert.Equal(365.00m, bis15.PrecioBs);
            Assert.False(bis15.PermiteFraccionamiento);
            Assert.NotEmpty(bis15.Receta);
            Assert.Equal("INS-BIS-15", bis15.Receta[0].InsumoCodigo);

            var atropina = result.FirstOrDefault(item => item.Codigo == "MED-ATR-01");
            Assert.NotNull(atropina);
            Assert.Equal("ATROPINA", atropina.Descripcion);
            Assert.Equal("MEDICAMENTO", atropina.EditorType);
            Assert.Equal(TipoServicioConstants.Insumo, atropina.TipoServicioId);
            Assert.Equal(15.00m, atropina.PrecioUsd);
            Assert.True(atropina.PermiteFraccionamiento);
        }

        [Fact]
        public async Task Should_NotDuplicateInsumos_If_Already_Exists_In_ServiciosClinicos()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.TasaCambio.Add(new TasaCambio(36.5m));

            // Servicio clínico configurado con código "MED-ATR-01"
            var servicio = new ServicioClinico("MED-ATR-01", "ATROPINA EN SERVICIO", 20.00m, "MEDICAMENTO")
            {
                Activo = true,
                Category = ServiceCategory.Insumo,
                TipoServicioId = TipoServicioConstants.Insumo
            };
            context.ServiciosClinicos.Add(servicio);

            // Insumo en inventario con el mismo código
            var insumo = new Insumo("MED-ATR-01", "ATROPINA", 500, UnidadMedida.UNIDAD, 15.00m);
            context.Insumos.Add(insumo);

            await context.SaveChangesAsync();

            var legacyRepoMock = new Mock<ILegacyLabRepository>();
            legacyRepoMock.Setup(r => r.GetAvailableProfilesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PerfilLegacy>());

            var handler = new GetUnifiedCatalogQueryHandler(
                context,
                legacyRepoMock.Object,
                NullLogger<GetUnifiedCatalogQueryHandler>.Instance
            );

            // Act
            var result = await handler.Handle(new GetUnifiedCatalogQuery(), CancellationToken.None);

            // Assert
            var items = result.Where(i => i.Codigo == "MED-ATR-01").ToList();
            Assert.Single(items);
            Assert.Equal("ATROPINA EN SERVICIO", items[0].Descripcion);
        }

        [Fact]
        public async Task Should_Exclude_SoftDeleted_Insumos()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.TasaCambio.Add(new TasaCambio(36.5m));

            var insumoActivo = new Insumo("INS-ACTIVO", "Insumo Activo", 100, UnidadMedida.UNIDAD, 5.00m);
            var insumoBorrado = new Insumo("INS-BORRADO", "Insumo Inactivo", 100, UnidadMedida.UNIDAD, 5.00m);
            insumoBorrado.SoftDelete();

            context.Insumos.AddRange(insumoActivo, insumoBorrado);
            await context.SaveChangesAsync();

            var legacyRepoMock = new Mock<ILegacyLabRepository>();
            legacyRepoMock.Setup(r => r.GetAvailableProfilesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PerfilLegacy>());

            var handler = new GetUnifiedCatalogQueryHandler(
                context,
                legacyRepoMock.Object,
                NullLogger<GetUnifiedCatalogQueryHandler>.Instance
            );

            // Act
            var result = await handler.Handle(new GetUnifiedCatalogQuery(), CancellationToken.None);

            // Assert
            Assert.Contains(result, i => i.Codigo == "INS-ACTIVO");
            Assert.DoesNotContain(result, i => i.Codigo == "INS-BORRADO");
        }
    }
}
