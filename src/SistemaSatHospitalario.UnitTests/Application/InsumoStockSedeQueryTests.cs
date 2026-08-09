using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class InsumoStockSedeQueryTests
    {
        [Fact]
        public void ProyeccionStock_PorSedeEspecifica_RetornaStockFisicoDeDichaSede()
        {
            // Arrange
            var insumo = new Insumo("INS-TEST-01", "Dexametasona 4mg/ml", 0, UnidadMedida.ML, 1.20m);
            var sedePrincipalId = SeedConstants.SedeId_Principal;
            var sedeEmergenciaId = Guid.NewGuid();

            var stockPrincipal = new StockSede(insumo.Id, sedePrincipalId, 50m);
            var stockEmergencia = new StockSede(insumo.Id, sedeEmergenciaId, 12m);

            insumo.StocksPorSede.Add(stockPrincipal);
            insumo.StocksPorSede.Add(stockEmergencia);

            var insumosList = new List<Insumo> { insumo };

            // Act (simulando la proyección LINQ del Controller para Sede Emergencia)
            Guid targetSedeId = sedeEmergenciaId;
            var result = insumosList.Select(i => new
            {
                i.Id,
                i.Codigo,
                i.Nombre,
                StockActual = i.StocksPorSede
                    .Where(s => s.SedeId == targetSedeId)
                    .Select(s => (decimal?)s.StockActual)
                    .FirstOrDefault() ?? 0
            }).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(12m, result[0].StockActual);
        }

        [Fact]
        public void ProyeccionStock_InsumoSinStockEnSedeSecundaria_RetornaCeroYPermaneceEnCatálogo()
        {
            // Arrange
            var insumo1 = new Insumo("INS-TEST-02", "Omeprazol 40mg Vial", 0, UnidadMedida.UNIDAD, 3.50m);
            var insumo2 = new Insumo("INS-TEST-03", "Ketoprofeno 100mg Amp", 0, UnidadMedida.UNIDAD, 2.10m);

            var sedePrincipalId = SeedConstants.SedeId_Principal;
            var sedeUciId = Guid.NewGuid();

            // insumo1 tiene stock en Principal pero NO en UCI
            insumo1.StocksPorSede.Add(new StockSede(insumo1.Id, sedePrincipalId, 100m));

            // insumo2 tiene stock en Principal y también en UCI
            insumo2.StocksPorSede.Add(new StockSede(insumo2.Id, sedePrincipalId, 80m));
            insumo2.StocksPorSede.Add(new StockSede(insumo2.Id, sedeUciId, 15m));

            var insumosList = new List<Insumo> { insumo1, insumo2 };

            // Act (proyección filtrando por sede UCI)
            Guid targetSedeId = sedeUciId;
            var result = insumosList.Select(i => new
            {
                i.Id,
                i.Codigo,
                i.Nombre,
                StockActual = i.StocksPorSede
                    .Where(s => s.SedeId == targetSedeId)
                    .Select(s => (decimal?)s.StockActual)
                    .FirstOrDefault() ?? 0
            }).ToList();

            // Assert: Ambos insumos del catálogo deben retornarse
            Assert.Equal(2, result.Count);

            var dtoInsumo1 = result.First(r => r.Codigo == "INS-TEST-02");
            Assert.Equal(0m, dtoInsumo1.StockActual); // No tiene registro en UCI -> Stock 0

            var dtoInsumo2 = result.First(r => r.Codigo == "INS-TEST-03");
            Assert.Equal(15m, dtoInsumo2.StockActual); // Tiene registro en UCI -> Stock 15
        }

        [Fact]
        public void ProyeccionStock_SinSedeEspecifica_FallbackASedePrincipal()
        {
            // Arrange
            var insumo = new Insumo("INS-TEST-04", "Atropina 1mg", 30m, UnidadMedida.UNIDAD, 0.80m);
            var sedePrincipalId = SeedConstants.SedeId_Principal;
            var sedeHospId = Guid.NewGuid();

            insumo.StocksPorSede.Add(new StockSede(insumo.Id, sedeHospId, 5m));

            var insumosList = new List<Insumo> { insumo };

            // Act (simulando targetSedeId = sedeId ?? SeedConstants.SedeId_Principal)
            Guid? nullSedeId = null;
            Guid targetSedeId = nullSedeId ?? SeedConstants.SedeId_Principal;

            var result = insumosList.Select(i => new
            {
                i.Id,
                StockActual = i.StocksPorSede
                    .Where(s => s.SedeId == targetSedeId)
                    .Select(s => (decimal?)s.StockActual)
                    .FirstOrDefault() ?? 0
            }).ToList();

            // Assert
            Assert.Equal(30m, result[0].StockActual);
        }

        [Fact]
        public void ProyeccionStock_PorCodigo_RetornaInsumoYStockCorrecto()
        {
            // Arrange
            var codigoBusqueda = "INS-LAP-01";
            var insumo = new Insumo(codigoBusqueda, "KIT DE LAPARATOMIA", 100m, UnidadMedida.UNIDAD, 10.00m);
            var sedePrincipalId = SeedConstants.SedeId_Principal;
            insumo.StocksPorSede.Add(new StockSede(insumo.Id, sedePrincipalId, 100m));

            var insumosList = new List<Insumo> { insumo };

            // Act: Simulación de consulta por código de insumo en API REST
            var targetInsumo = insumosList.FirstOrDefault(i => i.Codigo == codigoBusqueda && !i.IsDeleted);
            Assert.NotNull(targetInsumo);

            var stockPrincipal = targetInsumo.StocksPorSede
                .Where(s => s.SedeId == sedePrincipalId)
                .Select(s => (decimal?)s.StockActual)
                .FirstOrDefault() ?? targetInsumo.StockActual;

            var dto = new
            {
                targetInsumo.Id,
                targetInsumo.Codigo,
                targetInsumo.Nombre,
                StockActual = stockPrincipal,
                UnidadMedidaBase = targetInsumo.UnidadMedidaBase.ToString()
            };

            // Assert
            Assert.Equal("INS-LAP-01", dto.Codigo);
            Assert.Equal("KIT DE LAPARATOMIA", dto.Nombre);
            Assert.Equal(100m, dto.StockActual);
            Assert.Equal("UNIDAD", dto.UnidadMedidaBase);
        }
    }
}
