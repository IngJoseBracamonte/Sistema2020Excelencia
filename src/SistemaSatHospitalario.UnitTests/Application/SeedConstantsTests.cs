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

        [Fact]
        public void RequisitosCirugiaConstants_DebenEstarDefinidasYNoSerVacias()
        {
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000001"), SeedConstants.RequisitoId_Cardiovascular);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000002"), SeedConstants.RequisitoId_Laboratorio);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000003"), SeedConstants.RequisitoId_Consentimiento);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000004"), SeedConstants.RequisitoId_Ayuno);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000005"), SeedConstants.RequisitoId_ValoracionAnestesica);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000006"), SeedConstants.RequisitoId_ReservaSangre);
            Assert.Equal(new Guid("40000000-0000-0000-0000-000000000007"), SeedConstants.RequisitoId_CamaPostoperatoria);
        }

        [Fact]
        public void CategoriasInsumoConstants_DebenEstarDefinidasYNoSerVacias()
        {
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000001"), SeedConstants.CategoriaId_Medicamento);
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000002"), SeedConstants.CategoriaId_Descartable);
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000003"), SeedConstants.CategoriaId_MaterialMedico);
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000004"), SeedConstants.CategoriaId_Reactivo);
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000005"), SeedConstants.CategoriaId_MaterialQuirurgico);
            Assert.Equal(new Guid("50000000-0000-0000-0000-000000000006"), SeedConstants.CategoriaId_Otro);
        }

        [Fact]
        public void SedesPrincipalesConstants_DebenEstarDefinidasYNoSerVacias()
        {
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000001"), SeedConstants.SedeId_Principal);
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000002"), SeedConstants.SedeId_Emergencia);
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000003"), SeedConstants.SedeId_Hospitalizacion);
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000004"), SeedConstants.SedeId_UCI);
            Assert.Equal(new Guid("10000000-0000-0000-0000-000000000005"), SeedConstants.SedeId_Cirugia);
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
