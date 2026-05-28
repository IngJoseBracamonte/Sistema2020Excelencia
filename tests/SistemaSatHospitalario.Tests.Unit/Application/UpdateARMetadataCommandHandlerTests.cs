using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class UpdateARMetadataCommandHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly UpdateARMetadataCommandHandler _handler;

        public UpdateARMetadataCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _handler = new UpdateARMetadataCommandHandler(_context);
        }

        [Fact]
        public async Task Should_UpdateMetadata_When_NoGuaranteesProvided()
        {
            // Arrange
            var cxcId = Guid.NewGuid();
            var cxc = new CuentaPorCobrar(Guid.NewGuid(), Guid.NewGuid(), 500m, 100m);
            // set private property Id using reflection since it has a private setter
            typeof(CuentaPorCobrar).GetProperty("Id")?.SetValue(cxc, cxcId);

            _context.CuentasPorCobrar.Add(cxc);
            await _context.SaveChangesAsync();

            var command = new UpdateARMetadataCommand
            {
                CuentaPorCobrarId = cxcId,
                QuienAutorizo = "Dr. Sanchez",
                DoctorProcedimiento = "Dr. Perez",
                InformacionAdicional = "Informacion extra de test"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var updatedCxc = await _context.CuentasPorCobrar.FindAsync(cxcId);
            updatedCxc.Should().NotBeNull();
            updatedCxc.QuienAutorizo.Should().Be("Dr. Sanchez");
            updatedCxc.DoctorProcedimiento.Should().Be("Dr. Perez");
            updatedCxc.InformacionAdicional.Should().Be("Informacion extra de test");
            updatedCxc.GarantiaGenerada.Should().BeFalse();
            updatedCxc.GarantiasItems.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_UpdateMetadata_And_PersistGuarantees_When_GuaranteesProvided()
        {
            // Arrange
            var cxcId = Guid.NewGuid();
            var cxc = new CuentaPorCobrar(Guid.NewGuid(), Guid.NewGuid(), 500m, 100m);
            typeof(CuentaPorCobrar).GetProperty("Id")?.SetValue(cxc, cxcId);

            _context.CuentasPorCobrar.Add(cxc);
            await _context.SaveChangesAsync();

            var command = new UpdateARMetadataCommand
            {
                CuentaPorCobrarId = cxcId,
                QuienAutorizo = "Dr. Sanchez",
                DoctorProcedimiento = "Dr. Perez",
                InformacionAdicional = "Con garantías prendarias",
                GarantiasItems = new List<GarantiaItemDto>
                {
                    new GarantiaItemDto { Descripcion = "Vehiculo Suzuki", ValorEstimado = 1500m },
                    new GarantiaItemDto { Descripcion = "Reloj de Oro", ValorEstimado = 300m }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var updatedCxc = await _context.CuentasPorCobrar.FindAsync(cxcId);
            updatedCxc.GarantiaGenerada.Should().BeTrue();

            var dbItems = await _context.GarantiasItems
                .Where(x => x.CuentaPorCobrarId == cxcId)
                .ToListAsync();

            dbItems.Should().HaveCount(2);
            dbItems.Should().Contain(x => x.Descripcion == "Vehiculo Suzuki" && x.ValorEstimado == 1500m);
            dbItems.Should().Contain(x => x.Descripcion == "Reloj de Oro" && x.ValorEstimado == 300m);
        }

        [Fact]
        public async Task Should_ClearExistingGuarantees_Before_InsertingNewOnes()
        {
            // Arrange
            var cxcId = Guid.NewGuid();
            var cxc = new CuentaPorCobrar(Guid.NewGuid(), Guid.NewGuid(), 500m, 100m);
            typeof(CuentaPorCobrar).GetProperty("Id")?.SetValue(cxc, cxcId);

            _context.CuentasPorCobrar.Add(cxc);
            
            // Add an existing guarantee
            var existingItem = new GarantiaItem(cxcId, "Antigua Garantia", 500m);
            _context.GarantiasItems.Add(existingItem);
            
            await _context.SaveChangesAsync();

            var command = new UpdateARMetadataCommand
            {
                CuentaPorCobrarId = cxcId,
                QuienAutorizo = "Autorizador",
                GarantiasItems = new List<GarantiaItemDto>
                {
                    new GarantiaItemDto { Descripcion = "Nueva Garantia", ValorEstimado = 1000m }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var dbItems = await _context.GarantiasItems
                .Where(x => x.CuentaPorCobrarId == cxcId)
                .ToListAsync();

            dbItems.Should().HaveCount(1);
            dbItems.First().Descripcion.Should().Be("Nueva Garantia");
            dbItems.First().ValorEstimado.Should().Be(1000m);
        }
    }
}
