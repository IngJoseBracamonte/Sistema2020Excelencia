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
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CierreCajaCommandHandlerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<ICajaAdministrativaRepository> _repositoryMock;

        public CierreCajaCommandHandlerTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _repositoryMock = new Mock<ICajaAdministrativaRepository>();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private async Task SeedCatalogAndTestDataAsync(Guid cajaId)
        {
            // Seed payment methods catalog
            _context.CatalogoMetodosPago.AddRange(
                new CatalogoMetodoPago("EFECTIVO DOLAR ($)", "Dolar Efectivo", 1, esVuelto: false, orden: 1),
                new CatalogoMetodoPago("VUELTO EFECTIVO USD", "Vuelto Efectivo USD", 1, esVuelto: true, orden: 2),
                new CatalogoMetodoPago("EFECTIVO BS", "Efectivo BS", 2, esVuelto: false, orden: 3),
                new CatalogoMetodoPago("VUELTO EFECTIVO BS", "Vuelto Efectivo BS", 2, esVuelto: true, orden: 4)
            );

            // Seed Paciente and CuentaServicios to avoid FK constraint failures
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-12345678", "Paciente Test", "0412-1111111");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            _context.PacientesAdmision.Add(paciente);

            var cuenta = new CuentaServicios(pacienteId, "cajero1", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, cuentaId);
            _context.CuentasServicios.Add(cuenta);

            await _context.SaveChangesAsync();

            // Seed ReciboFactura and DetallePago to simulate sales in this box
            var recibo = new ReciboFactura(cuentaId, pacienteId, cajaId, 50m, 140m, 10m, "Emitida");
            _context.RecibosFactura.Add(recibo);
            await _context.SaveChangesAsync();

            // Add DetallePago (positive payments and negative vueltos)
            // 1. Dolar Efectivo payment of 100 USD (equivalente 100 USD base)
            var p1 = new DetallePago(recibo.Id, "Dolar Efectivo", "REF-USD-1", 100m, 100m, 1m, "cajero1");
            // 2. Vuelto Efectivo USD of -10 USD (equivalente -10 USD base)
            var p2 = new DetallePago(recibo.Id, "Vuelto Efectivo USD", "REF-VUELTO-USD", -10m, -10m, 1m, "cajero1");
            // 3. Efectivo BS payment of 2000 Bs (equivalente 40 USD base at tasa 50)
            var p3 = new DetallePago(recibo.Id, "Efectivo BS", "REF-BS-1", 2000m, 40m, 50m, "cajero1");

            _context.DetallesPago.AddRange(p1, p2, p3);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Should_SuccessfullyCerrarCajaPorAsistente_When_ValidDeclaration()
        {
            // Arrange
            var caja = new CajaDiaria(10m, 50m, "user-cajero", "cajero1");
            var cajaId = caja.Id;
            
            _context.CajasDiarias.Add(caja);
            await _context.SaveChangesAsync();

            await SeedCatalogAndTestDataAsync(cajaId);

            _repositoryMock.Setup(r => r.ObtenerCajaAbiertaPorUsuarioAsync("user-cajero", It.IsAny<CancellationToken>()))
                .ReturnsAsync(caja);

            var command = new CerrarCajaCommand
            {
                UsuarioId = "user-cajero",
                Declaracion = new List<MetodoDeclaradoDto>
                {
                    new() { MetodoPago = "Dolar Efectivo", MontoIngreso = 95m, MontoVueltos = 5m },
                    new() { MetodoPago = "Efectivo BS", MontoIngreso = 1900m, MontoVueltos = 0m }
                }
            };

            var handler = new CerrarCajaCommandHandler(_repositoryMock.Object, _context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CajaId.Should().Be(cajaId);
            result.Usuario.Should().Be("cajero1");

            // Verify box state update in domain object
            caja.Estado.Should().Be("CerradaPorAsistente");
            caja.TotalIngresado.Should().NotBeNull();
            caja.TotalCobrado.Should().NotBeNull();
            
            // Expected computation:
            // Dolar Efectivo expected net = 100 - 10 = 90 USD
            // Efectivo BS expected net = 2000 Bs = 40 USD
            // Expected base total = 130 USD
            caja.TotalCobrado.Should().Be(130m);

            // Declared computation:
            // Dolar Efectivo declared net = 95 - 5 = 90 USD
            // Efectivo BS declared net = 1900 Bs / 50 tasa = 38 USD
            // Declared base total = 90 + 38 = 128 USD
            caja.TotalIngresado.Should().Be(128m);

            // Diferencia: 128 - 130 = -2 USD
            caja.Diferencia.Should().Be(-2m);

            // Verify json declaration contains keys
            caja.DeclaracionCierreJson.Should().NotBeNullOrWhiteSpace();
            caja.DeclaracionCierreJson.Should().Contain("Dolar Efectivo");
            caja.DeclaracionCierreJson.Should().Contain("Efectivo BS");
        }

        [Fact]
        public async Task Should_SuccessfullyConsolidarCajas_When_AdminRunsConsolidarCommand()
        {
            // Arrange
            // Seed two boxes: one open, one closed por asistente, one consolidated
            var caja1 = new CajaDiaria(10m, 50m, "cajero1", "cajero1");
            caja1.CerrarPorAsistente("{}", 100m, 100m, 0m);

            var caja2 = new CajaDiaria(15m, 100m, "cajero2", "cajero2");
            
            var caja3 = new CajaDiaria(20m, 200m, "cajero3", "cajero3");
            caja3.CerrarCaja(); // Already closed/consolidated

            _context.CajasDiarias.AddRange(caja1, caja2, caja3);
            await _context.SaveChangesAsync();

            var handler = new ConsolidarCajasCommandHandler(_context);
            var command = new ConsolidarCajasCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            
            // caja1 should be consolidated (Estado = "Cerrada")
            caja1.Estado.Should().Be("Cerrada");
            // caja2 should remain active (Estado = "Abierta")
            caja2.Estado.Should().Be("Abierta");

            // Metrics:
            // CajasActivas: caja2 (Abierta) = 1
            // CierresPendientes: 0 (caja1 was consolidated)
            // CierresRealizados: caja1 + caja3 = 2
            result.CajasActivas.Should().Be(1);
            result.CierresPendientes.Should().Be(0);
            result.CierresRealizados.Should().Be(2);
        }
    }
}
