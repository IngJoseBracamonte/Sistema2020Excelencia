using System;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class SeedConstantsTests
    {
        [Fact]
        public void SedeId_Cirugia_DebeEstarDefinidaYNoSerVacia()
        {
            Assert.NotEqual(Guid.Empty, SeedConstants.SedeId_Cirugia);
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000005"), SeedConstants.SedeId_Cirugia);
        }

        [Fact]
        public void AreaId_Cirugia_DebeEstarDefinidaYNoSerVacia()
        {
            Assert.NotEqual(Guid.Empty, SeedConstants.AreaId_Cirugia);
            Assert.Equal(new Guid("30000000-0000-0000-0000-000000000006"), SeedConstants.AreaId_Cirugia);
        }

        [Theory]
        [InlineData("CIRUGIA", null)]
        [InlineData("CIRUGÍA", null)]
        [InlineData("QUIROFANO", null)]
        [InlineData("QUIRÓFANO", null)]
        public void ResolveSedeInventario_ConCirugiaOQuirofano_DebeRetornarSedeIdCirugia(string tipoIngreso, string? subAreaClinica)
        {
            var sedeId = SeedConstants.ResolveSedeInventario(tipoIngreso, subAreaClinica);
            Assert.Equal(SeedConstants.SedeId_Cirugia, sedeId);
        }
    }
}
