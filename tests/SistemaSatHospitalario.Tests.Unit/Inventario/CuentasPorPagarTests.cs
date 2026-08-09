using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Inventario;
using SistemaSatHospitalario.Core.Application.Queries.Inventario;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Moq;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Inventario
{
    public class CuentasPorPagarTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<ICurrentUserService> _userServiceMock;

        public CuentasPorPagarTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _userServiceMock = new Mock<ICurrentUserService>();
            _userServiceMock.Setup(u => u.UserName).Returns("admin");
        }

        [Fact]
        public void OrdenCompraInventario_Creacion_DebeEstablecerValoresInicialesCorrectos()
        {
            // Arrange & Act
            var orden = new OrdenCompraInventario("FAC-2026-001", "Droguería Central C.A.", DateTime.UtcNow, 1000.00m, 50.00m);

            // Assert
            orden.NumeroFactura.Should().Be("FAC-2026-001");
            orden.ProveedorNombre.Should().Be("Droguería Central C.A.");
            orden.MontoTotalUSD.Should().Be(1000.00m);
            orden.MontoTotalBs.Should().Be(50000.00m);
            orden.TotalAbonadoUSD.Should().Be(0m);
            orden.SaldoPendienteUSD.Should().Be(1000.00m);
            orden.Estado.Should().Be("PorPagar");
        }

        [Fact]
        public void RegistrarAbono_Parcial_DebeActualizarSaldoYPermanecerPorPagar()
        {
            // Arrange
            var orden = new OrdenCompraInventario("FAC-2026-002", "FarmaCorp", DateTime.UtcNow, 500.00m, 50.00m);

            // Act
            var pago = orden.RegistrarAbono(200.00m, 50.00m, "Transferencia", "REF-12345", "admin", "Abono 1");

            // Assert
            pago.MontoAbonadoUSD.Should().Be(200.00m);
            pago.MontoAbonadoBs.Should().Be(10000.00m);
            orden.TotalAbonadoUSD.Should().Be(200.00m);
            orden.SaldoPendienteUSD.Should().Be(300.00m);
            orden.Estado.Should().Be("PorPagar");
        }

        [Fact]
        public void RegistrarAbono_Total_DebeCambiarEstadoAPagadoAutomaticamente()
        {
            // Arrange
            var orden = new OrdenCompraInventario("FAC-2026-003", "Biolab S.A.", DateTime.UtcNow, 500.00m, 50.00m);
            orden.RegistrarAbono(200.00m, 50.00m, "Zelle", "REF-001", "admin");

            // Act: Abonar el saldo restante ($300)
            orden.RegistrarAbono(300.00m, 50.00m, "Efectivo USD", "EFECTIVO-002", "admin");

            // Assert
            orden.TotalAbonadoUSD.Should().Be(500.00m);
            orden.SaldoPendienteUSD.Should().Be(0m);
            orden.Estado.Should().Be("Pagado");
        }

        [Fact]
        public void RegistrarAbono_Excesivo_DebeLanzarExcepcion()
        {
            // Arrange
            var orden = new OrdenCompraInventario("FAC-2026-004", "Proveedor X", DateTime.UtcNow, 100.00m, 50.00m);

            // Act
            Action act = () => orden.RegistrarAbono(150.00m, 50.00m, "Transferencia", "REF-999", "admin");

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*supera el saldo pendiente*");
        }

        [Fact]
        public async Task RegistrarPagoProveedorCommandHandler_DebePersistirPagoYActualizarOrden()
        {
            // Arrange
            var orden = new OrdenCompraInventario("FAC-2026-100", "Proveedor Med", DateTime.UtcNow, 400.00m, 50.00m);
            _context.OrdenesCompraInventario.Add(orden);
            await _context.SaveChangesAsync();

            var handler = new RegistrarPagoProveedorCommandHandler(_context, _userServiceMock.Object);
            var command = new RegistrarPagoProveedorCommand
            {
                OrdenCompraId = orden.Id,
                MontoAbonadoUSD = 400.00m,
                TasaCambio = 50.00m,
                MetodoPago = "Transferencia",
                Referencia = "REF-FULL",
                Observaciones = "Pago total"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.MontoAbonadoUSD.Should().Be(400.00m);
            result.NumeroFactura.Should().Be("FAC-2026-100");

            var updatedOrden = await _context.OrdenesCompraInventario.FirstAsync(o => o.Id == orden.Id);
            updatedOrden.Estado.Should().Be("Pagado");
            updatedOrden.SaldoPendienteUSD.Should().Be(0m);
            updatedOrden.TotalAbonadoUSD.Should().Be(400.00m);
        }

        [Fact]
        public async Task GetOrdenesCompraQueryHandler_DebeFiltrarPorEstadoYBuscador()
        {
            // Arrange
            var o1 = new OrdenCompraInventario("FAC-001", "Droguería Alfa", DateTime.UtcNow, 200.00m, 50.00m);
            var o2 = new OrdenCompraInventario("FAC-002", "Droguería Beta", DateTime.UtcNow, 300.00m, 50.00m);
            o2.RegistrarAbono(300.00m, 50.00m, "Zelle", "REF-1", "admin"); // Pagado

            _context.OrdenesCompraInventario.AddRange(o1, o2);
            await _context.SaveChangesAsync();

            var handler = new GetOrdenesCompraQueryHandler(_context);

            // Act 1: Filtrar PorPagar
            var resPorPagar = await handler.Handle(new GetOrdenesCompraQuery { Estado = "PorPagar" }, CancellationToken.None);
            resPorPagar.Should().HaveCount(1);
            resPorPagar.First().NumeroFactura.Should().Be("FAC-001");

            // Act 2: Buscar por proveedor "Beta"
            var resSearch = await handler.Handle(new GetOrdenesCompraQuery { Busqueda = "Beta" }, CancellationToken.None);
            resSearch.Should().HaveCount(1);
            resSearch.First().ProveedorNombre.Should().Be("Droguería Beta");
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
