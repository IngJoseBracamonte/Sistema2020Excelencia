using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;
using MockQueryable.Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class HonorariumTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly IMemoryCache _cache;

        public HonorariumTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldPrioritizeExplicitCategory()
        {
            // Arrange
            var service = new ServicioClinico("S1", "Desc 1", 10, "Tipo 1") 
            { 
                HonorariumCategory = "RX" 
            };
            var serviceId = service.Id;
            
            var serviceList = new List<ServicioClinico> { service };
            var servicesMock = serviceList.BuildMockDbSet<ServicioClinico>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(servicesMock.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(new List<HonorariumMappingRule>().BuildMockDbSet<HonorariumMappingRule>().Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("OTRO", serviceId);

            // Assert
            Assert.Equal("RX", result);
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldFallbackToRulesIfNoExplicitCategory()
        {
            // Arrange
            var service = new ServicioClinico("S1", "Desc 1", 10, "RX-Digital") 
            { 
                HonorariumCategory = null // Sin categoría explícita
            };
            var serviceId = service.Id;
            
            var serviceList = new List<ServicioClinico> { service };
            var servicesMock = serviceList.BuildMockDbSet<ServicioClinico>();

            var ruleList = new List<HonorariumMappingRule>
            {
                new HonorariumMappingRule("RX", "RX", MappingRuleType.Contains, 1, "Admin")
            };
            var rulesMock = ruleList.BuildMockDbSet<HonorariumMappingRule>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(servicesMock.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(rulesMock.Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("RX-Digital", serviceId);

            // Assert
            Assert.Equal("RX", result);
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldReturnOtrosIfNoMatchFound()
        {
            // Arrange
            var emptyServices = new List<ServicioClinico>().BuildMockDbSet<ServicioClinico>();
            var emptyRules = new List<HonorariumMappingRule>().BuildMockDbSet<HonorariumMappingRule>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(emptyServices.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(emptyRules.Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("MISTERIOSO", Guid.NewGuid());

            // Assert
            Assert.Equal(HonorarioConstants.CategoriaOtros, result);
        }
    }
}
