using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
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
    public class SedesTests
    {
        [Fact]
        public async Task CreateSede_ShouldThrowException_IfActivePrincipalSedeAlreadyExists()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var sedesList = new List<Sede>
            {
                new Sede("S01", "Sede Principal Existente", true)
            };
            var sedesMock = sedesList.BuildMockDbSet<Sede>();

            mockContext.Setup(c => c.Sedes).Returns(sedesMock.Object);

            var handler = new CreateSedeCommandHandler(mockContext.Object);
            var command = new CreateSedeCommand
            {
                Codigo = "S02",
                Nombre = "Intento Principal",
                EsPrincipal = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateSede_ShouldThrowException_IfActivePrincipalSedeAlreadyExists()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var existingPrincipal = new Sede("S01", "Sede Principal Existente", true);
            var targetSede = new Sede("S02", "Sede Secundaria", false);

            var sedesList = new List<Sede> { existingPrincipal, targetSede };
            var sedesMock = sedesList.BuildMockDbSet<Sede>();

            mockContext.Setup(c => c.Sedes).Returns(sedesMock.Object);

            var handler = new UpdateSedeCommandHandler(mockContext.Object);
            var command = new UpdateSedeCommand
            {
                Id = targetSede.Id,
                Codigo = "S02",
                Nombre = "Sede Secundaria Modificada",
                EsPrincipal = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteSede_ShouldDeactivateSedeAndCascadeToAreasClinicas()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var sede = new Sede("S01", "Sede de prueba", false);
            var area1 = new AreaClinica(sede.Id, "A01", "Quirófano");
            var area2 = new AreaClinica(sede.Id, "A02", "Emergencia");

            var sedesList = new List<Sede> { sede };
            var sedesMock = sedesList.BuildMockDbSet<Sede>();

            var areasList = new List<AreaClinica> { area1, area2 };
            var areasMock = areasList.BuildMockDbSet<AreaClinica>();

            mockContext.Setup(c => c.Sedes).Returns(sedesMock.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasMock.Object);

            var handler = new DeleteSedeCommandHandler(mockContext.Object);
            var command = new DeleteSedeCommand { Id = sede.Id };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(sede.Activo);
            Assert.False(area1.Activo);
            Assert.False(area2.Activo);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
