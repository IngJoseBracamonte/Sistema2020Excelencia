using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class AgendarTurnoCommandHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly AgendarTurnoCommandHandler _handler;

        public AgendarTurnoCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new SatHospitalarioDbContext(options);
            _handler = new AgendarTurnoCommandHandler(_context);
        }

        [Fact]
        public async Task Should_Schedule_When_NoColissionExists()
        {
            // Arrange
            var command = new AgendarTurnoCommand
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = 1, // Legacy ID
                CuentaServicioId = Guid.NewGuid(),
                FechaHoraToma = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _context.CitasMedicas.Should().ContainSingle();
        }
    }
}
