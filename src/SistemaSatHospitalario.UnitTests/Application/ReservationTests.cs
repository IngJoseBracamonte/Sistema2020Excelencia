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
    public class ReservationTests
    {
        [Fact]
        public async Task Handle_ShouldCreateReservaTemporal_WithComentario()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            
            var reservas = new List<ReservaTemporal>().BuildMockDbSet<ReservaTemporal>();
            var citas = new List<CitaMedica>().BuildMockDbSet<CitaMedica>();
            
            mockContext.Setup(c => c.ReservasTemporales).Returns(reservas.Object);
            mockContext.Setup(c => c.CitasMedicas).Returns(citas.Object);

            var handler = new ReservarTurnoTemporalCommandHandler(mockContext.Object);
            var medicoId = Guid.NewGuid();
            var hora = DateTime.Now.AddHours(1);
            var command = new ReservarTurnoTemporalCommand
            {
                MedicoId = medicoId,
                HoraPautada = hora,
                UsuarioId = "test-user",
                Comentario = "Paciente con requerimientos especiales"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            reservas.Verify(m => m.Add(It.Is<ReservaTemporal>(r => 
                r.Comentario == "Paciente con requerimientos especiales" && 
                r.MedicoId == medicoId)), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_IfCitaAlreadyExists()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var medicoId = Guid.NewGuid();
            var hora = new DateTime(2026, 3, 26, 10, 0, 0);
            
            var citas = new List<CitaMedica> 
            { 
                new CitaMedica(medicoId, Guid.NewGuid(), Guid.NewGuid(), hora) 
            }.BuildMockDbSet<CitaMedica>();
            
            var reservas = new List<ReservaTemporal>().BuildMockDbSet<ReservaTemporal>();

            mockContext.Setup(c => c.CitasMedicas).Returns(citas.Object);
            mockContext.Setup(c => c.ReservasTemporales).Returns(reservas.Object);

            var handler = new ReservarTurnoTemporalCommandHandler(mockContext.Object);
            var command = new ReservarTurnoTemporalCommand
            {
                MedicoId = medicoId,
                HoraPautada = hora,
                UsuarioId = "test-user"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                handler.Handle(command, CancellationToken.None));
        }
    }
}
