using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Tests.Unit.Common.Builders;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    /// <summary>
    /// SettleARCommandHandlerTests.
    /// Verifica la liquidación de cuentas por cobrar bajo la arquitectura USD-First usando base de datos SQLite In-Memory.
    /// </summary>
    public class SettleARCommandHandlerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<INotificationService> _notificationMock;
        private readonly SettleARCommandHandler _handler;

        public SettleARCommandHandlerTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _notificationMock = new Mock<INotificationService>();
            _handler = new SettleARCommandHandler(_context, _notificationMock.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private async Task<(Guid PacienteId, Guid CuentaId)> SeedPacienteAndCuentaAsync()
        {
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();

            var p = new PacienteAdmision("123", "Test P", "555");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(p, pacienteId);
            _context.PacientesAdmision.Add(p);

            var c = new CuentaServicios(pacienteId, "Adm", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(c, cuentaId);
            _context.CuentasServicios.Add(c);

            await _context.SaveChangesAsync();
            return (pacienteId, cuentaId);
        }

        [Fact]
        public async Task Should_SuccessfullySettle_When_ValidPayments_And_NoPreviousReceipt()
        {
            // Arrange
            var ids = await SeedPacienteAndCuentaAsync();
            var arId = Guid.NewGuid();
            var ar = new ARBuilder()
                .WithId(arId)
                .WithPacienteId(ids.PacienteId)
                .WithCuentaId(ids.CuentaId)
                .WithTotal(100)
                .Build();
            var tasaActual = new TasaCambio(50);

            _context.CuentasPorCobrar.Add(ar);
            _context.TasaCambio.Add(tasaActual);
            await _context.SaveChangesAsync();

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

            var updatedAr = await _context.CuentasPorCobrar.AsNoTracking().FirstAsync(a => a.Id == arId);
            updatedAr.Estado.Should().Be(EstadoConstants.Cobrada);
            updatedAr.MontoPagadoBase.Should().Be(updatedAr.MontoTotalBase);

            var paymentDetails = await _context.DetallesPago.ToListAsync();
            paymentDetails.Should().ContainSingle();
            paymentDetails[0].MetodoPago.Should().Be("Zelle");
            paymentDetails[0].ReferenciaBancaria.Should().Be("TX-ZELLE-1");
            paymentDetails[0].EquivalenteAbonadoBase.Should().Be(100);
        }

        [Fact]
        public async Task Should_StoreExactExchangeRateAndEnteredAmount_When_ProcessingMulticurrencyPayment()
        {
            // Arrange
            var ids = await SeedPacienteAndCuentaAsync();
            var arId = Guid.NewGuid();
            var ar = new ARBuilder()
                .WithId(arId)
                .WithPacienteId(ids.PacienteId)
                .WithCuentaId(ids.CuentaId)
                .WithTotal(110) // Total $110 USD
                .Build();
            var tasaActual = new TasaCambio(50); // Daily rate = 50 VES per USD

            _context.CuentasPorCobrar.Add(ar);
            _context.TasaCambio.Add(tasaActual);

            // Seed active catalog methods for Group 1 (USD) and Group 2 (VES)
            _context.CatalogoMetodosPago.AddRange(
                new CatalogoMetodoPago("EFECTIVO BS", "Efectivo BS", 2, esVuelto: false, orden: 1),
                new CatalogoMetodoPago("ZELLE USD", "Zelle", 1, esVuelto: false, orden: 2)
            );
            await _context.SaveChangesAsync();

            var command = new SettleARCommand
            {
                ArId = arId,
                UsuarioCarga = "cajero_test",
                Payments = new List<PaymentItem>
                {
                    // Payment 1: Zelle (USD, Group 1) -> $10.00 USD
                    new() { Method = "Zelle", Amount = 10, AmountMoneda = 10, TasaAplicada = 1, Reference = "REF-ZELLE" },
                    // Payment 2: Efectivo BS (VES, Group 2) -> 5000.00 Bs
                    new() { Method = "Efectivo BS", Amount = 100, AmountMoneda = 5000, TasaAplicada = 50, Reference = "REF-VES" }
                },
                Observaciones = "Multi-currency settlement test"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var updatedAr = await _context.CuentasPorCobrar.AsNoTracking().FirstAsync(a => a.Id == arId);
            updatedAr.Estado.Should().Be(EstadoConstants.Cobrada);
            updatedAr.MontoPagadoBase.Should().Be(110m);

            var paymentDetails = await _context.DetallesPago.OrderBy(x => x.MetodoPago).ToListAsync();
            paymentDetails.Count.Should().Be(2);

            // Payment 1: BS (starts with E)
            var bsPayment = paymentDetails[0];
            bsPayment.MetodoPago.Should().Be("Efectivo BS");
            bsPayment.ReferenciaBancaria.Should().Be("REF-VES");
            bsPayment.MontoAbonadoMoneda.Should().Be(5000m);
            bsPayment.EquivalenteAbonadoBase.Should().Be(100m); // 5000 / 50
            bsPayment.TasaCambioAplicada.Should().Be(50m);
            bsPayment.UsuarioCarga.Should().Be("cajero_test");
            bsPayment.FechaPago.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));

            // Payment 2: Zelle (starts with Z)
            var zellePayment = paymentDetails[1];
            zellePayment.MetodoPago.Should().Be("Zelle");
            zellePayment.ReferenciaBancaria.Should().Be("REF-ZELLE");
            zellePayment.MontoAbonadoMoneda.Should().Be(10m);
            zellePayment.EquivalenteAbonadoBase.Should().Be(10m); // 10 USD
            zellePayment.TasaCambioAplicada.Should().Be(1m);
            zellePayment.UsuarioCarga.Should().Be("cajero_test");
            zellePayment.FechaPago.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }


        [Fact]
        public async Task Should_ReturnTrue_When_Account_Already_Cobrada()
        {
            // Arrange
            var ids = await SeedPacienteAndCuentaAsync();
            var arId = Guid.NewGuid();
            var ar = new ARBuilder()
                .WithId(arId)
                .WithPacienteId(ids.PacienteId)
                .WithCuentaId(ids.CuentaId)
                .WithEstado(EstadoConstants.Cobrada)
                .Build();

            _context.CuentasPorCobrar.Add(ar);
            await _context.SaveChangesAsync();

            var command = new SettleARCommand { ArId = arId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Fail_When_No_Active_Tasa_Defined()
        {
            // Arrange
            var ids = await SeedPacienteAndCuentaAsync();
            var arId = Guid.NewGuid();
            var ar = new ARBuilder()
                .WithId(arId)
                .WithPacienteId(ids.PacienteId)
                .WithCuentaId(ids.CuentaId)
                .Build();

            _context.CuentasPorCobrar.Add(ar);
            await _context.SaveChangesAsync();

            var command = new SettleARCommand { ArId = arId };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("No existe una tasa de cambio activa configurada.");
        }
    }
}
