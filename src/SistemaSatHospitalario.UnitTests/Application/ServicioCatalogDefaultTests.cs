using System;
using Xunit;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ServicioCatalogDefaultTests
    {
        [Fact]
        public void CatalogItemDto_ShouldDefaultToServicioEditorType()
        {
            var dto = new CatalogItemDto();
            Assert.Equal("SERVICIO", dto.EditorType);
        }

        [Fact]
        public void CatalogItemDto_CalculatePrices_ShouldPreserveServicioUsdBase()
        {
            var dto = new CatalogItemDto
            {
                PrecioUsd = 45.00m,
                EditorType = "SERVICIO"
            };

            dto.CalculatePrices(36.5m);

            Assert.Equal(45.00m, dto.PrecioUsd);
            Assert.Equal(1642.50m, dto.PrecioBs);
            Assert.Equal("SERVICIO", dto.EditorType);
        }
    }
}
