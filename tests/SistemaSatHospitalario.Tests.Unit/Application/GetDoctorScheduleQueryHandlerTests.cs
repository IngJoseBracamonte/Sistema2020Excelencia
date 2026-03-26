using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class GetDoctorScheduleQueryHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly GetDoctorScheduleQueryHandler _handler;

        public GetDoctorScheduleQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new SatHospitalarioDbContext(options);
            _handler = new GetDoctorScheduleQueryHandler(_context);
        }

        [Fact]
        public async Task Should_Identify_OwnAppointment_When_PacienteIdMatches()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var pacienteId = 123;
            var fecha = DateTime.Today;

            _context.CitasMedicas.AddRange(
                new CitaMedica(medicoId, pacienteId, Guid.NewGuid(), fecha.AddHours(10)), // Tu Cita
                new CitaMedica(medicoId, 456, Guid.NewGuid(), fecha.AddHours(11)) // Otra Cita
            );
            await _context.SaveChangesAsync(CancellationToken.None);

            var query = new GetDoctorScheduleQuery(medicoId, fecha, null, pacienteId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var slot10 = result.Turnos.First(s => s.Hora == fecha.AddHours(10));
            slot10.Ocupado.Should().BeFalse(); // Es mi cita, no debe bloquearme el wizard (UX choice)
            slot10.Comentario.Should().Contain("Tu Cita (Agregada)");

            var slot11 = result.Turnos.First(s => s.Hora == fecha.AddHours(11));
            slot11.Ocupado.Should().BeTrue(); // Cita ajena (Paciente 456)
            slot11.Comentario.Should().NotContain("Tu Cita");
        }
    }
}
