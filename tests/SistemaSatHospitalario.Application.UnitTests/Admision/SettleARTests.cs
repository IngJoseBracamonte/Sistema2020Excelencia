using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Application.UnitTests.Admision.Builders;
using MockQueryable.Moq;

namespace SistemaSatHospitalario.Application.UnitTests.Admision
{
    public class SettleARTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly SettleARCommandHandler _handler;

        public SettleARTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new SettleARCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Should_SuccessfullySettle_WhenPaymentsAreValid()
        {
            // Arrange
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).WithTotal(100).WithEstado(EstadoConstants.Pendiente).Build();
            
            var tasa = new TasaCambio { Monto = 50.00m, Activo = true, Fecha = DateTime.UtcNow };

            // Mock DbSet for CuentasPorCobrar
            var arList = new List<CuentaPorCobrar> { ar }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(arList.Object);

            // Mock DbSet for TasaCambio
            var tasaList = new List<TasaCambio> { tasa }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.TasaCambio).Returns(tasaList.Object);

            // Mock DbSet for RecibosFactura (Empty initially)
            var reciboList = new List<ReciboFactura>().AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.RecibosFactura).Returns(reciboList.Object);

            var command = new SettleARCommand
            {
                ArId = arId,
                Payments = new List<PaymentItem>
                {
                    new() { Method = "Efectivo", Amount = 100, AmountMoneda = 100, TasaAplicada = 50, Reference = "REF1" }
                },
                Observaciones = "Test settlement"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(EstadoConstants.Cobrada, ar.Estado);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ThrowException_WhenAccountIsAlreadySettled()
        {
            // Arrange
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).WithEstado(EstadoConstants.Cobrada).Build();
            
            var arList = new List<CuentaPorCobrar> { ar }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(arList.Object);

            var command = new SettleARCommand { ArId = arId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("ya ha sido cobrada", ex.Message);
        }

        [Fact]
        public async Task Should_ThrowException_WhenNoActiveTasaExists()
        {
            // Arrange
            var arId = Guid.NewGuid();
            var ar = new ARBuilder().WithId(arId).Build();
            
            var arList = new List<CuentaPorCobrar> { ar }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.CuentasPorCobrar).Returns(arList.Object);

            // Tasa vacía
            var tasaList = new List<TasaCambio>().AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.TasaCambio).Returns(tasaList.Object);

            var command = new SettleARCommand { ArId = arId };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Contains("No existe una tasa de cambio activa", ex.Message);
        }
    }
}
