using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class AgendarTurnoCommandHandlerTests
    {
        private readonly Mock<ITurnoMedicoRepository> _turnoRepositoryMock;
        private readonly Mock<IAuditoriaIncidenciaRepository> _auditoriaRepositoryMock;
        private readonly AgendarTurnoCommandHandler _handler;

        public AgendarTurnoCommandHandlerTests()
        {
            _turnoRepositoryMock = new Mock<ITurnoMedicoRepository>();
            _auditoriaRepositoryMock = new Mock<IAuditoriaIncidenciaRepository>();
            _handler = new AgendarTurnoCommandHandler(_turnoRepositoryMock.Object, _auditoriaRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_ScheduleWithoutAuditing_When_NoIncidenceExists()
        {
            // Arrange
            var command = new AgendarTurnoCommand
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                FechaHoraToma = DateTime.UtcNow.AddDays(1)
            };

            _turnoRepositoryMock
                .Setup(r => r.ObtenerIncidenciaSolaParaHoraAsync(command.MedicoId, command.FechaHoraToma, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IncidenciaHorario)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _turnoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<TurnoMedico>(), It.IsAny<CancellationToken>()), Times.Once);
            _auditoriaRepositoryMock.Verify(r => r.RegistrarAsync(It.IsAny<RegistroAuditoriaIncidencia>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_ThrowException_When_IncidenceExists_And_ExplicitIgnoralIsFalse()
        {
            // Arrange
            var command = new AgendarTurnoCommand
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                FechaHoraToma = DateTime.UtcNow,
                IgnorarIncidencia = false
            };

            var incidencia = new IncidenciaHorario(command.MedicoId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(2), TipoComentarioHorario.Incidencia, "Llega Tarde", Guid.NewGuid());

            _turnoRepositoryMock
                .Setup(r => r.ObtenerIncidenciaSolaParaHoraAsync(command.MedicoId, command.FechaHoraToma, It.IsAny<CancellationToken>()))
                .ReturnsAsync(incidencia);

            // Act
            Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"El horario solicitado presenta una incidencia de tipo {incidencia.Tipo}: {incidencia.Descripcion}. Debe confirmar la omisión explicita para agendar.");
            
            _turnoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<TurnoMedico>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_ScheduleAndAudit_When_IncidenceExists_And_ExplicitIgnoralIsTrue()
        {
            // Arrange
            var incidenciaId = Guid.NewGuid();
            var command = new AgendarTurnoCommand
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                FechaHoraToma = DateTime.UtcNow,
                IgnorarIncidencia = true,
                IncidenciaIgnoradaId = incidenciaId
            };

            var incidencia = new IncidenciaHorario(command.MedicoId, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(2), TipoComentarioHorario.Informativo, "Consulta Corta", Guid.NewGuid());
            // Reflection used to bypass private setter for mock test equality
            typeof(IncidenciaHorario).GetProperty("Id").SetValue(incidencia, incidenciaId);

            _turnoRepositoryMock
                .Setup(r => r.ObtenerIncidenciaSolaParaHoraAsync(command.MedicoId, command.FechaHoraToma, It.IsAny<CancellationToken>()))
                .ReturnsAsync(incidencia);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _turnoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<TurnoMedico>(), It.IsAny<CancellationToken>()), Times.Once);
            _auditoriaRepositoryMock.Verify(r => r.RegistrarAsync(It.IsAny<RegistroAuditoriaIncidencia>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
