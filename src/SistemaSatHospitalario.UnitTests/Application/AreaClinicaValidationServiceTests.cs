using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class AreaClinicaValidationServiceTests
    {
        [Fact]
        public async Task ValidateAreaClinicaExistsAsync_Should_Return_True_When_Id_Matches_Sede()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var sedeUci = new Sede("UCI", "Sede UCI", false, SeedConstants.SedeId_UCI);

            var sedesList = new List<Sede> { sedeUci }.BuildMockDbSet();
            var areasList = new List<AreaClinica>().BuildMockDbSet();

            mockContext.Setup(c => c.Sedes).Returns(sedesList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);

            var service = new AreaClinicaValidationService(mockContext.Object);

            // Act
            var isValid = await service.ValidateAreaClinicaExistsAsync(SeedConstants.SedeId_UCI, CancellationToken.None);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateAreaClinicaExistsOrThrowAsync_Should_Not_Throw_When_Id_Matches_Sede()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var sedeUci = new Sede("UCI", "Sede UCI", false, SeedConstants.SedeId_UCI);

            var sedesList = new List<Sede> { sedeUci }.BuildMockDbSet();
            var areasList = new List<AreaClinica>().BuildMockDbSet();

            mockContext.Setup(c => c.Sedes).Returns(sedesList.Object);
            mockContext.Setup(c => c.AreasClinicas).Returns(areasList.Object);

            var service = new AreaClinicaValidationService(mockContext.Object);

            // Act & Assert (should not throw)
            await service.ValidateAreaClinicaExistsOrThrowAsync(SeedConstants.SedeId_UCI, CancellationToken.None);
        }
    }
}
