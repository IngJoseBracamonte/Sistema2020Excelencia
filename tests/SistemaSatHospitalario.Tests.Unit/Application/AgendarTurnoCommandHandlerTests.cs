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
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using Moq;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class AgendarTurnoCommandHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<ILegacyLabRepository> _legacyRepositoryMock;
        private readonly AgendarTurnoCommandHandler _handler;

        public AgendarTurnoCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new SatHospitalarioDbContext(options);
            _legacyRepositoryMock = new Mock<ILegacyLabRepository>();
            _handler = new AgendarTurnoCommandHandler(_context, _legacyRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Schedule_When_NoColissionExists()
        {
            var pacienteLegacy = new DatosPersonalesLegacy { IdPersona = 1, Nombre = "Test", Apellidos = "Legacy" };
            _legacyRepositoryMock.Setup(r => r.GetPatientByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(pacienteLegacy);

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
