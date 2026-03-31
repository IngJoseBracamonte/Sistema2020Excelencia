using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Tests.Unit.Common;
using SistemaSatHospitalario.Tests.Unit.Common.Builders;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    /// <summary>
    /// SettleARCommandHandlerTests (Senior Standards).
    /// Verifica la liquidación de cuentas por cobrar bajo la arquitectura USD-First.
    /// </summary>
    public class SettleARCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly SettleARCommandHandler _handler;

        public SettleARCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new SettleARCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Should_SuccessfullySettle_When_ValidPayments_And_NoPreviousReceipt()
        {
            // Arrange (Escenario Senior: Sin recibo previo, debe autogenerarlo)
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).WithTotal(100).Build();
            var tasaActual = new TasaCambio(50);

            // Setup Mocks usando el helper TestAsyncEnumerable para soportar querys asíncronas
            var arSet = new List<CuentaPorCobrar> { ar }.AsQueryable();
            var mockArSet = new Mock<DbSet<CuentaPorCobrar>>();
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<CuentaPorCobrar>(arSet.Provider));
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Expression).Returns(arSet.Expression);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.ElementType).Returns(arSet.ElementType);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.GetEnumerator()).Returns(arSet.GetEnumerator());
            mockArSet.As<IAsyncEnumerable<CuentaPorCobrar>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<CuentaPorCobrar>(arSet.GetEnumerator()));
            
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(mockArSet.Object);

            var tasaSet = new List<TasaCambio> { tasaActual }.AsQueryable();
            var mockTasaSet = new Mock<DbSet<TasaCambio>>();
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TasaCambio>(tasaSet.Provider));
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.Expression).Returns(tasaSet.Expression);
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.ElementType).Returns(tasaSet.ElementType);
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.GetEnumerator()).Returns(tasaSet.GetEnumerator());
            mockTasaSet.As<IAsyncEnumerable<TasaCambio>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TasaCambio>(tasaSet.GetEnumerator()));

            _contextMock.Setup(c => c.TasaCambio).Returns(mockTasaSet.Object);

            var reciboSet = new List<ReciboFactura>().AsQueryable();
            var mockReciboSet = new Mock<DbSet<ReciboFactura>>();
            mockReciboSet.As<IQueryable<ReciboFactura>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<ReciboFactura>(reciboSet.Provider));
            mockReciboSet.As<IQueryable<ReciboFactura>>().Setup(m => m.Expression).Returns(reciboSet.Expression);
            mockReciboSet.As<IQueryable<ReciboFactura>>().Setup(m => m.ElementType).Returns(reciboSet.ElementType);
            mockReciboSet.As<IQueryable<ReciboFactura>>().Setup(m => m.GetEnumerator()).Returns(reciboSet.GetEnumerator());
            mockReciboSet.As<IAsyncEnumerable<ReciboFactura>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ReciboFactura>(reciboSet.GetEnumerator()));

            _contextMock.Setup(c => c.RecibosFactura).Returns(mockReciboSet.Object);

            var command = new SettleARCommand
            {
                ArId = arId,
                Payments = new List<PaymentItem>
                {
                    new() { Method = "Zelle", Amount = 100, AmountMoneda = 100, TasaAplicada = 50, Reference = "TX-ZELLE-1" }
                },
                Observaciones = "Senior Test Settlement"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            ar.Estado.Should().Be(EstadoConstants.Cobrada);
            ar.MontoPagadoBase.Should().Be(ar.MontoTotalBase);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Fail_When_Account_Already_Cobrada()
        {
            // Arrange
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).WithEstado(EstadoConstants.Cobrada).Build();

            var arSet = new List<CuentaPorCobrar> { ar }.AsQueryable();
            var mockArSet = new Mock<DbSet<CuentaPorCobrar>>();
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<CuentaPorCobrar>(arSet.Provider));
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Expression).Returns(arSet.Expression);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.ElementType).Returns(arSet.ElementType);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.GetEnumerator()).Returns(arSet.GetEnumerator());
            mockArSet.As<IAsyncEnumerable<CuentaPorCobrar>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<CuentaPorCobrar>(arSet.GetEnumerator()));
            
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(mockArSet.Object);

            var command = new SettleARCommand { ArId = arId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Esta cuenta ya ha sido cobrada.");
        }

        [Fact]
        public async Task Should_Fail_When_No_Active_Tasa_Defined()
        {
            // Arrange
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).Build();

            var arSet = new List<CuentaPorCobrar> { ar }.AsQueryable();
            var mockArSet = new Mock<DbSet<CuentaPorCobrar>>();
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<CuentaPorCobrar>(arSet.Provider));
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.Expression).Returns(arSet.Expression);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.ElementType).Returns(arSet.ElementType);
            mockArSet.As<IQueryable<CuentaPorCobrar>>().Setup(m => m.GetEnumerator()).Returns(arSet.GetEnumerator());
            mockArSet.As<IAsyncEnumerable<CuentaPorCobrar>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<CuentaPorCobrar>(arSet.GetEnumerator()));
            
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(mockArSet.Object);

            // Tasa vacía
            var tasaSet = new List<TasaCambio>().AsQueryable();
            var mockTasaSet = new Mock<DbSet<TasaCambio>>();
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<TasaCambio>(tasaSet.Provider));
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.Expression).Returns(tasaSet.Expression);
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.ElementType).Returns(tasaSet.ElementType);
            mockTasaSet.As<IQueryable<TasaCambio>>().Setup(m => m.GetEnumerator()).Returns(tasaSet.GetEnumerator());
            mockTasaSet.As<IAsyncEnumerable<TasaCambio>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TasaCambio>(tasaSet.GetEnumerator()));

            _contextMock.Setup(c => c.TasaCambio).Returns(mockTasaSet.Object);

            var command = new SettleARCommand { ArId = arId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("No existe una tasa de cambio activa configurada.");
        }
    }
}
