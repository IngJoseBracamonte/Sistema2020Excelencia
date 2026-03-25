using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class AgendarTurnoCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly AgendarTurnoCommandHandler _handler;

        public AgendarTurnoCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            
            // Setup Mocks for DbSets (Minimal to compile and basic flow)
            var citas = new List<CitaMedica>().AsQueryable();
            var mockSetCitas = CreateMockSet(citas);
            _contextMock.Setup(m => m.CitasMedicas).Returns(mockSetCitas.Object);

            var bloqueos = new List<BloqueoHorario>().AsQueryable();
            var mockSetBloqueos = CreateMockSet(bloqueos);
            _contextMock.Setup(m => m.BloqueosHorarios).Returns(mockSetBloqueos.Object);

            var reservas = new List<ReservaTemporal>().AsQueryable();
            var mockSetReservas = CreateMockSet(reservas);
            _contextMock.Setup(m => m.ReservasTemporales).Returns(mockSetReservas.Object);

            _handler = new AgendarTurnoCommandHandler(_contextMock.Object);
        }

        private Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
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
            _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
