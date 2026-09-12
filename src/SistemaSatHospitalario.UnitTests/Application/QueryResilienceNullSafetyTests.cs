using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class QueryResilienceNullSafetyTests
    {
        [Fact]
        public async Task GetUnifiedCatalog_ShouldNotThrow_WhenInsumoUnidadMedidaNavIsNull()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLegacyRepo = new Mock<ILegacyLabRepository>();
            var mockLogger = new Mock<ILogger<GetUnifiedCatalogQueryHandler>>();

            // Insumo sin UnidadMedidaNav cargada (null)
            var insumoSinUnidad = new Insumo(
                codigo: "INS-TEST-01",
                nombre: "Insumo Test Sin Unidad",
                stockActual: 10m,
                unidadMedidaBase: UnidadMedidaEnum.UNIDAD,
                costoUnitarioBaseUSD: 5.5m,
                permiteFraccionamiento: true
            );

            var insumosList = new List<Insumo> { insumoSinUnidad }.BuildMockDbSet();
            var serviciosList = new List<ServicioClinico>().BuildMockDbSet();
            var sugerenciasList = new List<ServicioSugerencia>().BuildMockDbSet();
            var preciosConvenioList = new List<PrecioServicioConvenio>().BuildMockDbSet();
            var recetasDbSet = new List<ServicioInsumoReceta>().BuildMockDbSet();
            var tasasDbSet = new List<TasaCambio> { new TasaCambio(50.0m) }.BuildMockDbSet();

            mockContext.Setup(c => c.Insumos).Returns(insumosList.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosList.Object);
            mockContext.Setup(c => c.ServiciosSugerencias).Returns(sugerenciasList.Object);
            mockContext.Setup(c => c.PreciosServicioConvenio).Returns(preciosConvenioList.Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetasDbSet.Object);
            mockContext.Setup(c => c.TasaCambio).Returns(tasasDbSet.Object);

            var handler = new GetUnifiedCatalogQueryHandler(mockContext.Object, mockLegacyRepo.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetUnifiedCatalogQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var item = result.FirstOrDefault(x => x.Codigo == "INS-TEST-01");
            Assert.NotNull(item);
            Assert.Equal("Insumo Test Sin Unidad", item.Descripcion);
            Assert.NotEmpty(item.Receta);
            Assert.Equal("UND", item.Receta[0].UnidadMedidaConsumo);
        }

        [Fact]
        public async Task GetPabellonCalendario_ShouldNotThrow_WhenNavigationsAreNull()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var orden = new OrdenCirugia(
                cuentaServicioId: Guid.NewGuid(),
                pacienteId: Guid.NewGuid(),
                descripcionCirugia: "Cirugía de Emergencia",
                precioBaseUsd: 100m,
                medicoId: Guid.NewGuid(),
                fechaHoraProgramada: DateTime.UtcNow,
                usuarioCreacion: "admin"
            );
            // orden.Paciente y orden.Medico y orden.CuentaServicio son null

            var ordenesDbSet = new List<OrdenCirugia> { orden }.BuildMockDbSet();
            mockContext.Setup(c => c.OrdenesCirugia).Returns(ordenesDbSet.Object);

            var handler = new GetPabellonCalendarioQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(new GetPabellonCalendarioQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Sin Nombre", result[0].PacienteNombre);
            Assert.Equal("Sin Asignar", result[0].MedicoNombre);
            Assert.Equal("Hospitalizacion", result[0].IngresoCobertura.Tipo);
        }

        [Fact]
        public async Task GetTasaCambio_ShouldReturnFallback_WhenNoTasasExist()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var tasasVacias = new List<TasaCambio>().BuildMockDbSet();
            mockContext.Setup(c => c.TasaCambio).Returns(tasasVacias.Object);

            var handler = new GetTasaCambioQueryHandler(mockContext.Object);

            // Act
            var tasa = await handler.Handle(new GetTasaCambioQuery(), CancellationToken.None);

            // Assert
            Assert.True(tasa > 0);
            Assert.Equal(36.5m, tasa);
        }
    }
}
