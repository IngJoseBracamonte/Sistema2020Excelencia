using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CerrarCajaMultiMonedaTests
    {
        private readonly Mock<ICajaAdministrativaRepository> _mockRepo;
        private readonly Mock<IApplicationDbContext> _mockContext;

        public CerrarCajaMultiMonedaTests()
        {
            _mockRepo = new Mock<ICajaAdministrativaRepository>();
            _mockContext = new Mock<IApplicationDbContext>();
        }

        [Fact]
        public async Task Handle_Should_ConvertNonUsdVueltosToBaseUsd_When_CalculatingTotalVueltoUSD()
        {
            // Arrange
            var usuarioId = "cajero-01";
            var caja = new CajaDiaria(100m, 5000m, usuarioId, "Ana Lopez");
            _mockRepo.Setup(r => r.ObtenerCajaAbiertaPorUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(caja);

            // Catalogo de métodos de pago
            var metodoDolar = new CatalogoMetodoPago("EFECTIVO DÓLAR ($)", "Dolar Efectivo", 1, esVuelto: false, orden: 1);
            var metodoPagoMovil = new CatalogoMetodoPago("PAGO MÓVIL (BS.)", "Pago Movil", 2, esVuelto: false, orden: 2);
            var catalogoMetodos = new List<CatalogoMetodoPago> { metodoDolar, metodoPagoMovil };
            _mockContext.Setup(c => c.CatalogoMetodosPago).Returns(catalogoMetodos.BuildMockDbSet().Object);

            // Recibo con tasa 50.0 Bs/USD
            var recibo = new ReciboFactura(Guid.NewGuid(), Guid.NewGuid(), caja.Id, 50.0m, 100m, 0m, EstadoConstants.Emitida);
            var recibosList = new List<ReciboFactura> { recibo };
            _mockContext.Setup(c => c.RecibosFactura).Returns(recibosList.BuildMockDbSet().Object);

            var handler = new CerrarCajaCommandHandler(_mockRepo.Object, _mockContext.Object);

            // Declaración:
            // 1. Dólares: Ingreso $100, Vuelto $10 USD
            // 2. Pago Móvil: Ingreso 2500 Bs, Vuelto 500 Bs (Equivalente a 500 / 50 = $10 USD)
            var command = new CerrarCajaCommand
            {
                UsuarioId = usuarioId,
                Declaracion = new List<MetodoDeclaradoDto>
                {
                    new MetodoDeclaradoDto { MetodoPago = "Dolar Efectivo", MontoIngreso = 100m, MontoVueltos = 10m },
                    new MetodoDeclaradoDto { MetodoPago = "Pago Movil", MontoIngreso = 2500m, MontoVueltos = 500m }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(caja.Id, result.CajaId);
            
            // Total Vueltos USD debe ser: $10 USD (Efectivo) + (500 Bs / 50 Tasa = $10 USD) = $20 USD
            Assert.Equal(20.0m, result.TotalVueltoUSD);

            // Total Ingresos BS debe ser exactamente los 2500 Bs declarados para métodos en BS
            Assert.Equal(2500.0m, result.TotalIngresosBS);
        }
    }
}
