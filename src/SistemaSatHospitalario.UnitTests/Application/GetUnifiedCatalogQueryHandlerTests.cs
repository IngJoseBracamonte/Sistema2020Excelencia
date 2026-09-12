using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetUnifiedCatalogQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ILegacyLabRepository> _mockLegacyRepository;
        private readonly Mock<ILogger<GetUnifiedCatalogQueryHandler>> _mockLogger;

        public GetUnifiedCatalogQueryHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockLegacyRepository = new Mock<ILegacyLabRepository>();
            _mockLogger = new Mock<ILogger<GetUnifiedCatalogQueryHandler>>();
        }

        [Fact]
        public async Task Handle_Should_Include_Inactive_Items_When_IncluirInactivos_Is_True()
        {
            // Arrange
            var itemActivo = new ServicioClinico("ACT-001", "Servicio Activo", 100m, "CONSULTA");
            var itemInactivo = new ServicioClinico("INACT-001", "Servicio Inactivo", 50m, "CONSULTA");
            itemInactivo.Desactivar("UsuarioTest");

            var serviciosList = new List<ServicioClinico> { itemActivo, itemInactivo }.BuildMockDbSet();
            var tasas = new List<TasaCambio> { new TasaCambio(36.5m) }.BuildMockDbSet();
            var preciosConvenio = new List<PrecioServicioConvenio>().BuildMockDbSet();
            var recetas = new List<ServicioInsumoReceta>().BuildMockDbSet();
            var insumos = new List<Insumo>().BuildMockDbSet();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            _mockContext.Setup(c => c.TasaCambio).Returns(tasas.Object);
            _mockContext.Setup(c => c.PreciosServicioConvenio).Returns(preciosConvenio.Object);
            _mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetas.Object);
            _mockContext.Setup(c => c.Insumos).Returns(insumos.Object);

            var handler = new GetUnifiedCatalogQueryHandler(_mockContext.Object, _mockLegacyRepository.Object, _mockLogger.Object);
            var query = new GetUnifiedCatalogQuery { IncluirInactivos = true };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Codigo == "ACT-001" && r.Activo);
            Assert.Contains(result, r => r.Codigo == "INACT-001" && !r.Activo);
        }

        [Fact]
        public async Task Handle_Should_Exclude_Inactive_Items_When_IncluirInactivos_Is_False()
        {
            // Arrange
            var itemActivo = new ServicioClinico("ACT-002", "Servicio Activo", 100m, "CONSULTA");
            var itemInactivo = new ServicioClinico("INACT-002", "Servicio Inactivo", 50m, "CONSULTA");
            itemInactivo.Desactivar("UsuarioTest");

            var serviciosList = new List<ServicioClinico> { itemActivo, itemInactivo }.BuildMockDbSet();
            var tasas = new List<TasaCambio> { new TasaCambio(36.5m) }.BuildMockDbSet();
            var preciosConvenio = new List<PrecioServicioConvenio>().BuildMockDbSet();
            var recetas = new List<ServicioInsumoReceta>().BuildMockDbSet();
            var insumos = new List<Insumo>().BuildMockDbSet();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            _mockContext.Setup(c => c.TasaCambio).Returns(tasas.Object);
            _mockContext.Setup(c => c.PreciosServicioConvenio).Returns(preciosConvenio.Object);
            _mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetas.Object);
            _mockContext.Setup(c => c.Insumos).Returns(insumos.Object);

            var handler = new GetUnifiedCatalogQueryHandler(_mockContext.Object, _mockLegacyRepository.Object, _mockLogger.Object);
            var query = new GetUnifiedCatalogQuery { IncluirInactivos = false };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ACT-002", result[0].Codigo);
            Assert.True(result[0].Activo);
        }
    }
}
