using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class InventoryServiceTests
    {
        private readonly DbContextOptions<SatHospitalarioDbContext> _options;

        public InventoryServiceTests()
        {
            _options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Theory]
        [InlineData(UnidadMedida.L, UnidadMedida.ML, 1000.0)]
        [InlineData(UnidadMedida.ML, UnidadMedida.L, 0.001)]
        [InlineData(UnidadMedida.KG, UnidadMedida.G, 1000.0)]
        [InlineData(UnidadMedida.G, UnidadMedida.G, 1.0)]
        [InlineData(UnidadMedida.DG, UnidadMedida.G, 0.1)]
        [InlineData(UnidadMedida.MG, UnidadMedida.G, 0.001)]
        [InlineData(UnidadMedida.UNIDAD, UnidadMedida.UNIDAD, 1.0)]
        public void GetConversionFactor_ShouldCalculateCorrectly(UnidadMedida origen, UnidadMedida destino, decimal expected)
        {
            var result = InventoryService.GetConversionFactor(origen, destino);
            result.Should().Be(expected);
        }

        [Fact]
        public async Task RecordMovement_ShouldAddStockAndLogMovement()
        {
            using var context = new SatHospitalarioDbContext(_options);
            var insumo = new Insumo("INS-TEST", "Test Item", 10.0m, UnidadMedida.G, 1.50m);
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            var service = new InventoryService(context, NullLogger<InventoryService>.Instance);

            await service.RecordMovementAsync(insumo.Id, Guid.Empty, "Ingreso", 5.0m, UnidadMedida.G, "Operator", "Reason", CancellationToken.None);

            using var assertContext = new SatHospitalarioDbContext(_options);
            var updatedInsumo = await assertContext.Insumos
                .Include(i => i.StocksPorSede)
                .FirstOrDefaultAsync(i => i.Id == insumo.Id);
            updatedInsumo.Should().NotBeNull();
            updatedInsumo!.StockActual.Should().Be(15.0m);

            var movement = await assertContext.MovimientosInsumo.FirstOrDefaultAsync(m => m.InsumoId == insumo.Id);
            movement.Should().NotBeNull();
            movement!.TipoMovimiento.Should().Be("Ingreso");
            movement.CantidadOriginal.Should().Be(5.0m);
            movement.CantidadBase.Should().Be(5.0m);
            movement.Usuario.Should().Be("Operator");
        }

        [Fact]
        public async Task SaveChanges_ShouldThrowException_WhenModifyingOrDeletingMovimientoInsumo()
        {
            using var context = new SatHospitalarioDbContext(_options);
            var insumo = new Insumo("INS-TEST-2", "Test Item 2", 10.0m, UnidadMedida.G, 1.50m);
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            var service = new InventoryService(context, NullLogger<InventoryService>.Instance);
            await service.RecordMovementAsync(insumo.Id, Guid.Empty, "Ingreso", 5.0m, UnidadMedida.G, "Operator", "Reason", CancellationToken.None);

            using var actContext = new SatHospitalarioDbContext(_options);
            var movement = await actContext.MovimientosInsumo.FirstOrDefaultAsync();
            movement.Should().NotBeNull();

            actContext.Entry(movement!).State = EntityState.Modified;
            Func<Task> actModify = async () => await actContext.SaveChangesAsync();
            await actModify.Should().ThrowAsync<InvalidOperationException>();

            actContext.Entry(movement!).State = EntityState.Deleted;
            Func<Task> actDelete = async () => await actContext.SaveChangesAsync();
            await actDelete.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
