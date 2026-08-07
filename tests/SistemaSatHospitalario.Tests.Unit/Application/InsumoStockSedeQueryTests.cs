using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class InsumoStockSedeQueryTests
    {
        private readonly DbContextOptions<SatHospitalarioDbContext> _options;

        public InsumoStockSedeQueryTests()
        {
            _options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetInsumos_WithoutSedeId_ShouldDefaultToSedePrincipalStock()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            // El constructor de Insumo asigna stockActual (50.0m) a SedeId_Principal automáticamente
            var insumo = new Insumo("INS-001", "Paracetamol 500mg", 50.0m, UnidadMedida.UNIDAD, 0.50m);
            var sedeEmergenciaId = Guid.NewGuid();

            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            var stockEmergencia = new StockSede(insumo.Id, sedeEmergenciaId, 12.0m);
            context.StocksSedes.Add(stockEmergencia);
            await context.SaveChangesAsync();

            // Act - Query without explicit sedeId (targetSedeId falls back to SedeId_Principal)
            Guid? targetSedeId = null;
            var effectiveSedeId = targetSedeId ?? SeedConstants.SedeId_Principal;

            var result = await context.Insumos
                .Where(i => i.Id == insumo.Id)
                .Select(i => new
                {
                    i.Id,
                    i.Nombre,
                    StockActual = i.StocksPorSede
                        .Where(s => s.SedeId == effectiveSedeId)
                        .Select(s => (decimal?)s.StockActual)
                        .FirstOrDefault() ?? 0
                })
                .FirstOrDefaultAsync();

            // Assert
            result.Should().NotBeNull();
            result!.StockActual.Should().Be(50.0m);
        }

        [Fact]
        public async Task GetInsumos_WithSpecificSedeId_ShouldReturnStockForThatSede()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var insumo = new Insumo("INS-002", "Ibuprofeno 400mg", 100.0m, UnidadMedida.UNIDAD, 0.75m);
            var sedeEmergenciaId = Guid.NewGuid();

            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            var stockEmergencia = new StockSede(insumo.Id, sedeEmergenciaId, 25.0m);
            context.StocksSedes.Add(stockEmergencia);
            await context.SaveChangesAsync();

            // Act - Query specifically for Sede Emergencia
            Guid? targetSedeId = sedeEmergenciaId;
            var effectiveSedeId = targetSedeId ?? SeedConstants.SedeId_Principal;

            var result = await context.Insumos
                .Where(i => i.Id == insumo.Id)
                .Select(i => new
                {
                    i.Id,
                    i.Nombre,
                    StockActual = i.StocksPorSede
                        .Where(s => s.SedeId == effectiveSedeId)
                        .Select(s => (decimal?)s.StockActual)
                        .FirstOrDefault() ?? 0
                })
                .FirstOrDefaultAsync();

            // Assert
            result.Should().NotBeNull();
            result!.StockActual.Should().Be(25.0m);
        }

        [Fact]
        public async Task GetInsumos_WithSedeIdHavingNoStockRecord_ShouldReturnZero()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var insumo = new Insumo("INS-003", "Omeprazol 20mg", 30.0m, UnidadMedida.UNIDAD, 1.20m);
            var sedeHospitalizacionId = Guid.NewGuid();

            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            // Act - Query for Sede Hospitalizacion which has no stock record
            Guid? targetSedeId = sedeHospitalizacionId;
            var effectiveSedeId = targetSedeId ?? SeedConstants.SedeId_Principal;

            var result = await context.Insumos
                .Where(i => i.Id == insumo.Id)
                .Select(i => new
                {
                    i.Id,
                    i.Nombre,
                    StockActual = i.StocksPorSede
                        .Where(s => s.SedeId == effectiveSedeId)
                        .Select(s => (decimal?)s.StockActual)
                        .FirstOrDefault() ?? 0
                })
                .FirstOrDefaultAsync();

            // Assert
            result.Should().NotBeNull();
            result!.StockActual.Should().Be(0.0m);
        }
    }
}
