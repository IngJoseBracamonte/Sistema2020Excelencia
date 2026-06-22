using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CierreCajaReportsAndRealtimeQueriesTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<IExcelService> _excelServiceMock;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public CierreCajaReportsAndRealtimeQueriesTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _excelServiceMock = new Mock<IExcelService>();
            _identityServiceMock = new Mock<IIdentityService>();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private async Task SeedDataAsync(Guid cajaId)
        {
            // Seed payment methods
            _context.CatalogoMetodosPago.AddRange(
                new CatalogoMetodoPago("EFECTIVO DOLAR ($)", "Dolar Efectivo", 1, esVuelto: false, orden: 1),
                new CatalogoMetodoPago("VUELTO EFECTIVO USD", "Vuelto Efectivo USD", 1, esVuelto: true, orden: 2),
                new CatalogoMetodoPago("EFECTIVO BS", "Efectivo BS", 2, esVuelto: false, orden: 3),
                new CatalogoMetodoPago("VUELTO EFECTIVO BS", "Vuelto Efectivo BS", 2, esVuelto: true, orden: 4)
            );

            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-12345678", "Paciente Test", "0412-1111111");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            _context.PacientesAdmision.Add(paciente);

            var cuenta = new CuentaServicios(pacienteId, "cajero1", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, cuentaId);
            _context.CuentasServicios.Add(cuenta);

            await _context.SaveChangesAsync();

            var recibo = new ReciboFactura(cuentaId, pacienteId, cajaId, 50m, 140m, 10m, "Emitida");
            _context.RecibosFactura.Add(recibo);
            await _context.SaveChangesAsync();

            var p1 = new DetallePago(recibo.Id, "Dolar Efectivo", "REF-USD-1", 100m, 100m, 1m, "cajero1");
            var p2 = new DetallePago(recibo.Id, "Vuelto Efectivo USD", "REF-VUELTO-USD", -10m, -10m, 1m, "cajero1");

            _context.DetallesPago.AddRange(p1, p2);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetCajaSummaries_Should_GenerateLiveBreakdown_For_OpenCajas()
        {
            // Arrange
            var caja = new CajaDiaria(10m, 50m, "user-cajero", "cajero1");
            _context.CajasDiarias.Add(caja);
            await _context.SaveChangesAsync();

            await SeedDataAsync(caja.Id);

            var query = new GetCajaSummariesQuery
            {
                Desde = DateTime.UtcNow.AddDays(-1),
                Hasta = DateTime.UtcNow
            };

            var handler = new GetCajaSummariesQueryHandler(_context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CajasActivas.Should().Be(1);
            
            var details = result.Cierres.FirstOrDefault(c => c.Id == caja.Id);
            details.Should().NotBeNull();
            details!.Estado.Should().Be("Abierta");
            details.TotalCobrado.Should().Be(90m); // 100 - 10 = 90 USD
            details.TotalIngresado.Should().Be(90m);
            details.Diferencia.Should().Be(0m);
            details.DeclaracionCierreJson.Should().NotBeNullOrWhiteSpace();
            details.DeclaracionCierreJson.Should().Contain("Dolar Efectivo");
        }

        [Fact]
        public async Task ExportCashierAuditQuery_Should_Call_GenerateDetailedCashierReport()
        {
            // Arrange
            var caja = new CajaDiaria(10m, 50m, "user-cajero", "cajero1");
            _context.CajasDiarias.Add(caja);
            await _context.SaveChangesAsync();

            await SeedDataAsync(caja.Id);

            var mockUser = new UserDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Username = "user-cajero", FullName = "Cajero Test" };
            _identityServiceMock.Setup(i => i.GetUsersAsync())
                .ReturnsAsync(new List<UserDto> { mockUser });

            var dummyExcelBytes = new byte[] { 1, 2, 3, 4 };
            _excelServiceMock.Setup(e => e.GenerateDetailedCashierReport(It.IsAny<IEnumerable<CajeroReportDto>>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(dummyExcelBytes);

            var query = new ExportCashierAuditQuery
            {
                Date = DateTime.Today,
                IsAuditMode = true
            };

            var handler = new ExportCashierAuditQueryHandler(_context, _excelServiceMock.Object, _identityServiceMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(dummyExcelBytes);
            _excelServiceMock.Verify(e => e.GenerateDetailedCashierReport(
                It.Is<IEnumerable<CajeroReportDto>>(x => x.Any(c => c.Username == "cajero1" && c.TotalCobrado == 90m)),
                It.IsAny<decimal>(),
                It.IsAny<decimal>()
            ), Times.Once);
        }
    }
}
