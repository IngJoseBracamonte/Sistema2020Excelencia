using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetHistorialesQueryTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Guid _insumoId = Guid.NewGuid();

        public GetHistorialesQueryTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();

            var insumo = new Insumo("INS-TEST", "Paracetamol 500mg", 100, UnidadMedida.UNIDAD, 1.00m);
            typeof(Insumo).GetProperty("Id")!.SetValue(insumo, _insumoId);

            var movimientos = new List<MovimientoInsumo>
            {
                new MovimientoInsumo(_insumoId, SeedConstants.SedeId_Principal, "Ingreso", 100, UnidadMedida.UNIDAD, 100, "admin", "Carga inicial"),
                new MovimientoInsumo(_insumoId, SeedConstants.SedeId_Principal, "EnvioSubArea", -20, UnidadMedida.UNIDAD, 20, "enfermera", "Envío directo a Laboratorio"),
                new MovimientoInsumo(_insumoId, SeedConstants.SedeId_Principal, "Descarte", -5, UnidadMedida.UNIDAD, 5, "supervisor", "Vencimiento lote 123")
            };

            // Set Insumo navigation property
            foreach (var m in movimientos)
            {
                typeof(MovimientoInsumo).GetProperty("Insumo")!.SetValue(m, insumo);
            }

            var mockMovsDbSet = movimientos.BuildMockDbSet<MovimientoInsumo>();
            _mockContext.Setup(c => c.MovimientosInsumo).Returns(mockMovsDbSet.Object);
        }

        [Fact]
        public async Task GetHistorialMovimientosQuery_FiltroTipoMovimientoEnvioSubArea_RetornaSoloEnviosASubArea()
        {
            // Arrange
            var handler = new GetHistorialMovimientosQueryHandler(_mockContext.Object);
            var query = new GetHistorialMovimientosQuery
            {
                TipoMovimiento = "EnvioSubArea"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("EnvioSubArea", result[0].TipoMovimiento);
            Assert.Contains("Laboratorio", result[0].Motivo);
        }

        [Fact]
        public async Task GetHistorialMovimientosQuery_FiltroTipoMovimientoDescarte_RetornaSoloDescartes()
        {
            // Arrange
            var handler = new GetHistorialMovimientosQueryHandler(_mockContext.Object);
            var query = new GetHistorialMovimientosQuery
            {
                TipoMovimiento = "Descarte"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Descarte", result[0].TipoMovimiento);
            Assert.Contains("Vencimiento", result[0].Motivo);
        }
    }
}
