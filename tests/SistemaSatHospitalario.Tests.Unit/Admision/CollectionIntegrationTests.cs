using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Admision
{
    public class CollectionIntegrationTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;

        public CollectionIntegrationTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();
        }

        private (Guid PacienteId, Guid CuentaId, Guid CajaId) _seed;

        private async Task SeedInfrastructureAsync()
        {
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();
            var cajaId = Guid.NewGuid();

            var p = new PacienteAdmision("123", "Test P", "555");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(p, pacienteId);
            _context.PacientesAdmision.Add(p);

            var c = new CuentaServicios(pacienteId, "Adm", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(c, cuentaId);
            _context.CuentasServicios.Add(c);

            var caja = new CajaDiaria(100, 1000, "1", "admin");
            typeof(CajaDiaria).GetProperty("Id")?.SetValue(caja, cajaId);
            _context.CajasDiarias.Add(caja);

            _context.TasaCambio.Add(new TasaCambio(50.00m));
            
            await _context.SaveChangesAsync();
            _seed = (pacienteId, cuentaId, cajaId);
        }

        [Fact]
        public async Task SettleAR_ShouldBeAtomicAndReflectInDashboard()
        {
            // Arrange
            await SeedInfrastructureAsync();
            
            var ar = new CuentaPorCobrar(_seed.CuentaId, _seed.PacienteId, 100.00m, 0.00m);
            _context.CuentasPorCobrar.Add(ar);
            await _context.SaveChangesAsync();

            var handler = new SettleARCommandHandler(_context);
            var command = new SettleARCommand
            {
                ArId = ar.Id,
                Payments = new List<PaymentItem>
                {
                    new() { Method = "Efectivo", Amount = 100.00m, AmountMoneda = 100.00m, Reference = "T-1" }
                },
                Observaciones = "Test"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var updatedAr = await _context.CuentasPorCobrar.AsNoTracking().FirstAsync(a => a.Id == ar.Id);
            updatedAr.Estado.Should().Be(EstadoConstants.Cobrada);
        }

        [Fact]
        public async Task Dashboard_ShouldRespectTimezoneOffset()
        {
            // Arrange
            await SeedInfrastructureAsync();
            var hospitalNow = DateTime.UtcNow.AddHours(-4);
            var todayLocal = hospitalNow.Date;
            
            // Caso: Pago a las 23:00 local de HOY (en UTC ya es mañana)
            var fechaPagoUtcToday = todayLocal.AddHours(23).AddHours(4); 

            var recibo = new ReciboFactura(_seed.CuentaId, _seed.CajaId, null, 50.00m, 10.00m);
            _context.RecibosFactura.Add(recibo);
            await _context.SaveChangesAsync();
            
            var detalle = new DetallePago(recibo.Id, "Efectivo", "REF", 10.00m, 10.00m);
            typeof(DetallePago).GetProperty(nameof(DetallePago.FechaPago))?.SetValue(detalle, fechaPagoUtcToday);
            _context.DetallesPago.Add(detalle);
            await _context.SaveChangesAsync();

            // Act
            var handler = new GetBusinessInsightsQueryHandler(_context);
            var insights = await handler.Handle(new GetBusinessInsightsQuery { UserRole = "Admin" }, CancellationToken.None);

            // Assert
            insights.TotalVentasHoy.Should().Be(10.00m);
        }

        [Fact]
        public async Task SettleAR_Idempotencia_ShouldNotErrorOnRetry()
        {
            // Arrange
            await SeedInfrastructureAsync();

            var ar = new CuentaPorCobrar(_seed.CuentaId, _seed.PacienteId, 50.00m, 0.00m);
            _context.CuentasPorCobrar.Add(ar);
            await _context.SaveChangesAsync();

            var handler = new SettleARCommandHandler(_context);
            var command = new SettleARCommand
            {
                ArId = ar.Id,
                Payments = new List<PaymentItem> { new() { Amount = 50.00m, Method = "Efectivo", Reference = "R1" } },
                Observaciones = "T"
            };

            // Act 1: Primer cobro
            await handler.Handle(command, CancellationToken.None);

            // Act 2: Reintento
            var result2 = await handler.Handle(command, CancellationToken.None);

            // Assert
            result2.Should().BeTrue();
            var finalAr = await _context.CuentasPorCobrar.AsNoTracking().FirstAsync(a => a.Id == ar.Id);
            finalAr.Estado.Should().Be(EstadoConstants.Cobrada);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
