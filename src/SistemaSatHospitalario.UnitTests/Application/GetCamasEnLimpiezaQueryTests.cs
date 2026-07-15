using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetCamasEnLimpiezaQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnOnlyActiveCleanings_AndMapToDtoCorrectly()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var sede = new Sede("S01", "Sede Central", true);
            var cama1 = new AreaClinica(sede.Id, "C01", "Cama 101");
            var cama2 = new AreaClinica(sede.Id, "C02", "Cama 102");

            // Setup active cleaning (FechaFin is null)
            var activeCleaning = new HistorialLimpiezaCama(cama1.Id, DateTime.UtcNow.AddMinutes(-30), "nurse_admin", "Active cleaning");
            // Use reflection or constructor to set navigation property for Mock queryable mapping
            typeof(HistorialLimpiezaCama).GetProperty("Cama")?.SetValue(activeCleaning, cama1);

            // Setup finished cleaning (FechaFin has value)
            var finishedCleaning = new HistorialLimpiezaCama(cama2.Id, DateTime.UtcNow.AddHours(-2), "nurse_admin", "Finished cleaning");
            finishedCleaning.FinalizarLimpieza(DateTime.UtcNow.AddHours(-1), "cleaner_user");
            typeof(HistorialLimpiezaCama).GetProperty("Cama")?.SetValue(finishedCleaning, cama2);

            var list = new List<HistorialLimpiezaCama> { activeCleaning, finishedCleaning };
            var mockSet = list.BuildMockDbSet();

            mockContext.Setup(c => c.HistorialesLimpiezasCamas).Returns(mockSet.Object);

            var query = new GetCamasEnLimpiezaQuery();
            var handler = new GetCamasEnLimpiezaQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var activeDto = Assert.Single(result);
            Assert.Equal(activeCleaning.Id, activeDto.HistorialId);
            Assert.Equal(cama1.Id, activeDto.CamaId);
            Assert.Equal("C01", activeDto.CamaCodigo);
            Assert.Equal("Cama 101", activeDto.CamaNombre);
            Assert.Equal("Active cleaning", activeDto.Observaciones);
        }
    }
}
