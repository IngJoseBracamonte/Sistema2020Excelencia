using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class BlockingTests
    {
        [Fact]
        public async Task Handle_ShouldCreateBloqueo()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var bloqueos = new List<BloqueoHorario>().BuildMockDbSet<BloqueoHorario>();
            var citas = new List<CitaMedica>().BuildMockDbSet<CitaMedica>();

            mockContext.Setup(c => c.BloqueosHorarios).Returns(bloqueos.Object);
            mockContext.Setup(c => c.CitasMedicas).Returns(citas.Object);

            var handler = new BloquearHorarioCommandHandler(mockContext.Object);
            var medicoId = Guid.NewGuid();
            var hora = DateTime.Now.AddDays(1);
            var command = new BloquearHorarioCommand
            {
                MedicoId = medicoId,
                HoraPautada = hora,
                Motivo = "Reunión de staff"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            bloqueos.Verify(m => m.Add(It.Is<BloqueoHorario>(b => 
                b.Motivo == "Reunión de staff" && 
                b.MedicoId == medicoId)), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_IfCitaExistsAtSameTime()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var medicoId = Guid.NewGuid();
            var hora = new DateTime(2026, 3, 27, 10, 0, 0);

            var citas = new List<CitaMedica> 
            { 
                new CitaMedica(medicoId, Guid.NewGuid(), Guid.NewGuid(), hora) 
            }.BuildMockDbSet<CitaMedica>();
            
            var bloqueos = new List<BloqueoHorario>().BuildMockDbSet<BloqueoHorario>();

            mockContext.Setup(c => c.CitasMedicas).Returns(citas.Object);
            mockContext.Setup(c => c.BloqueosHorarios).Returns(bloqueos.Object);

            var handler = new BloquearHorarioCommandHandler(mockContext.Object);
            var command = new BloquearHorarioCommand
            {
                MedicoId = medicoId,
                HoraPautada = hora,
                Motivo = "Intento de bloqueo con cita previa"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            bloqueos.Verify(m => m.Add(It.IsAny<BloqueoHorario>()), Times.Never);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
