using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaSatHospitalario.Core.Application.Commands.Inventario;
using SistemaSatHospitalario.Core.Application.Queries.Inventario;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class ReposicionStockTests
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
        public async Task Should_ProcessReposicionStock_BetweenSedes_WithNoStockDrift()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var insumo = new Insumo("INS-GUANTES", "Guantes Estériles Talla 7.5", 100, UnidadMedida.UNIDAD, 1.20m);
            context.Insumos.Add(insumo);

            // Add stock for Sede_Emergencia and Sede_Hospitalizacion
            var stockEmergencia = new StockSede(insumo.Id, SeedConstants.SedeId_Emergencia, 20);
            var stockHospitalizacion = new StockSede(insumo.Id, SeedConstants.SedeId_Hospitalizacion, 10);
            context.StocksSedes.AddRange(stockEmergencia, stockHospitalizacion);
            await context.SaveChangesAsync();

            var handler = new ProcesarReposicionStockCommandHandler(context, NullLogger<ProcesarReposicionStockCommandHandler>.Instance);

            var command = new ProcesarReposicionStockCommand
            {
                InsumoId = insumo.Id,
                SedeOrigenId = SeedConstants.SedeId_Emergencia,
                SedeDestinoId = SeedConstants.SedeId_Hospitalizacion,
                Cantidad = 5,
                Motivo = "CambioTalla",
                UsuarioId = "supervisor_inventario",
                Observaciones = "Devolución y reposición de 5 pares"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var stockOrigen = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Emergencia);
            var stockDestino = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Hospitalizacion);

            stockOrigen.StockActual.Should().Be(15); // 20 - 5
            stockDestino.StockActual.Should().Be(15); // 10 + 5

            var transferencia = await context.TransferenciasReposicionStock.FirstOrDefaultAsync(t => t.InsumoId == insumo.Id);
            transferencia.Should().NotBeNull();
            transferencia!.Cantidad.Should().Be(5);
            transferencia.Motivo.Should().Be("CambioTalla");
        }

        [Fact]
        public async Task Should_ThrowException_When_StockOrigenIsInsufficient()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var insumo = new Insumo("INS-JERINGA", "Jeringa 10ml", 50, UnidadMedida.UNIDAD, 0.50m);
            context.Insumos.Add(insumo);

            var stockOrigen = new StockSede(insumo.Id, SeedConstants.SedeId_Emergencia, 3);
            context.StocksSedes.Add(stockOrigen);
            await context.SaveChangesAsync();

            var handler = new ProcesarReposicionStockCommandHandler(context, NullLogger<ProcesarReposicionStockCommandHandler>.Instance);

            var command = new ProcesarReposicionStockCommand
            {
                InsumoId = insumo.Id,
                SedeOrigenId = SeedConstants.SedeId_Emergencia,
                SedeDestinoId = SeedConstants.SedeId_Hospitalizacion,
                Cantidad = 10, // Exceeds available 3
                Motivo = "Reposicion",
                UsuarioId = "supervisor_inventario"
            };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Stock insuficiente en sede origen*");
        }

        [Fact]
        public async Task Should_ThrowArgumentException_When_SedeOrigenEqualsSedeDestino()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var handler = new ProcesarReposicionStockCommandHandler(context, NullLogger<ProcesarReposicionStockCommandHandler>.Instance);

            var command = new ProcesarReposicionStockCommand
            {
                InsumoId = Guid.NewGuid(),
                SedeOrigenId = SeedConstants.SedeId_Emergencia,
                SedeDestinoId = SeedConstants.SedeId_Emergencia, // Same sede
                Cantidad = 5,
                Motivo = "Reposicion"
            };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*no pueden ser iguales*");
        }

        [Fact]
        public async Task Should_QueryHistorial_FilteredBySedeAndInsumo()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var insumo = new Insumo("INS-CATETER", "Catéter 18G", 100, UnidadMedida.UNIDAD, 2.50m);
            context.Insumos.Add(insumo);

            var sedeOrigen = new Sede("ALM", "Almacén Central", true, Guid.NewGuid());
            var sedeDestino = new Sede("EME", "Emergencia", false, Guid.NewGuid());
            context.Sedes.AddRange(sedeOrigen, sedeDestino);

            var transferencia = new TransferenciaReposicionStock(
                insumo.Id,
                sedeOrigen.Id,
                sedeDestino.Id,
                12,
                "Reposicion",
                "supervisor",
                "Reabastecimiento semanal");
            context.TransferenciasReposicionStock.Add(transferencia);
            await context.SaveChangesAsync();

            var handler = new GetReposicionesHistorialQueryHandler(context);

            // Act
            var results = await handler.Handle(new GetReposicionesHistorialQuery
            {
                InsumoId = insumo.Id,
                SedeId = sedeOrigen.Id
            }, CancellationToken.None);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(1);
            results[0].InsumoCodigo.Should().Be("INS-CATETER");
            results[0].Cantidad.Should().Be(12);
        }
    }
}
