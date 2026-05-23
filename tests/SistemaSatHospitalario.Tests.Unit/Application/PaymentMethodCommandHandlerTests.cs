using System;
using System.Collections.Generic;
using System.Linq;
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
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class PaymentMethodCommandHandlerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<ICajaAdministrativaRepository> _cajaRepositoryMock;
        private readonly Mock<IBillingRepository> _billingRepositoryMock;

        public PaymentMethodCommandHandlerTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _cajaRepositoryMock = new Mock<ICajaAdministrativaRepository>();
            _billingRepositoryMock = new Mock<IBillingRepository>();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private async Task<(Guid PacienteId, Guid CuentaId, Guid ReciboId)> SeedBaseReceiptAsync()
        {
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();
            var reciboId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-12345678", "Paciente Test", "0412-1111111");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            _context.PacientesAdmision.Add(paciente);

            var cuenta = new CuentaServicios(pacienteId, "cajero1", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, cuentaId);
            _context.CuentasServicios.Add(cuenta);

            await _context.SaveChangesAsync();

            var recibo = new ReciboFactura(cuentaId, pacienteId, null, 1m, 100m);
            typeof(ReciboFactura).GetProperty("Id")?.SetValue(recibo, reciboId);
            _context.RecibosFactura.Add(recibo);

            await _context.SaveChangesAsync();

            return (pacienteId, cuentaId, reciboId);
        }

        // ==========================================
        // 1. TESTS CRUD (CatalogoMetodoPago)
        // ==========================================

        [Fact]
        public async Task Should_CreatePaymentMethodSuccessfully_When_ValidCommand()
        {
            // Arrange
            var handler = new CreatePaymentMethodCommandHandler(_context);
            var command = new CreatePaymentMethodCommand
            {
                Nombre = "Euro Cash",
                Valor = "Euro Efectivo",
                GrupoMoneda = 1, // USD
                EsVuelto = false,
                Orden = 9
            };

            // Act
            var resultId = await handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().NotBeEmpty();
            var created = await _context.CatalogoMetodosPago.FirstOrDefaultAsync(x => x.Id == resultId);
            created.Should().NotBeNull();
            created!.Nombre.Should().Be("Euro Cash");
            created.Valor.Should().Be("Euro Efectivo");
            created.GrupoMoneda.Should().Be(1);
            created.EsUSD.Should().BeTrue();
            created.EsVuelto.Should().BeFalse();
            created.Orden.Should().Be(9);
            created.Activo.Should().BeTrue();
        }

        [Fact]
        public async Task Should_ThrowInvalidOperationException_When_ValueAlreadyExists()
        {
            // Arrange
            _context.CatalogoMetodosPago.Add(new CatalogoMetodoPago("Dolar Efectivo Original", "Dolar Efectivo", 1));
            await _context.SaveChangesAsync();

            var handler = new CreatePaymentMethodCommandHandler(_context);
            var command = new CreatePaymentMethodCommand
            {
                Nombre = "Dolar Efectivo Nuevo",
                Valor = "dolar efectivo", // Duplicate check is case-insensitive
                GrupoMoneda = 1
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Ya existe un método de pago con el valor interno 'dolar efectivo'*");
        }

        [Fact]
        public async Task Should_UpdatePaymentMethodSuccessfully_When_ValidCommand()
        {
            // Arrange
            var original = new CatalogoMetodoPago("Efectivo BS Original", "Efectivo BS", 2, esVuelto: false, orden: 3);
            _context.CatalogoMetodosPago.Add(original);
            await _context.SaveChangesAsync();

            var handler = new UpdatePaymentMethodCommandHandler(_context);
            var command = new UpdatePaymentMethodCommand
            {
                Id = original.Id,
                Nombre = "Efectivo BS Modificado",
                Valor = "Efectivo BS Modificado",
                GrupoMoneda = 2,
                EsVuelto = true,
                Orden = 5,
                Activo = false
            };

            // Act
            var success = await handler.Handle(command, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            var updated = await _context.CatalogoMetodosPago.FirstOrDefaultAsync(x => x.Id == original.Id);
            updated.Should().NotBeNull();
            updated!.Nombre.Should().Be("Efectivo BS Modificado");
            updated.Valor.Should().Be("Efectivo BS Modificado");
            updated.GrupoMoneda.Should().Be(2);
            updated.EsUSD.Should().BeFalse();
            updated.EsVuelto.Should().BeTrue();
            updated.Orden.Should().Be(5);
            updated.Activo.Should().BeFalse();
        }

        [Fact]
        public async Task Should_ThrowInvalidOperationException_When_UpdatedValueAlreadyExistsForAnotherMethod()
        {
            // Arrange
            var methodA = new CatalogoMetodoPago("Method A", "A", 1);
            var methodB = new CatalogoMetodoPago("Method B", "B", 1);
            _context.CatalogoMetodosPago.AddRange(methodA, methodB);
            await _context.SaveChangesAsync();

            var handler = new UpdatePaymentMethodCommandHandler(_context);
            var command = new UpdatePaymentMethodCommand
            {
                Id = methodB.Id,
                Nombre = "Method B Modificado",
                Valor = "a", // Collides with methodA
                GrupoMoneda = 1,
                Activo = true
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Ya existe otro método de pago con el valor interno 'a'*");
        }

        [Fact]
        public async Task Should_SoftDelete_When_MethodHasAssociatedPayments()
        {
            // Arrange
            var method = new CatalogoMetodoPago("Zelle USD", "Zelle", 1);
            _context.CatalogoMetodosPago.Add(method);

            var (pacienteId, cuentaId, reciboId) = await SeedBaseReceiptAsync();
            var payment = new DetallePago(reciboId, "Zelle", "REF-123", 100m, 100m, 1m, "cajero1");
            _context.DetallesPago.Add(payment);
            await _context.SaveChangesAsync();

            var handler = new DeletePaymentMethodCommandHandler(_context);
            var command = new DeletePaymentMethodCommand { Id = method.Id };

            // Act
            var success = await handler.Handle(command, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            var result = await _context.CatalogoMetodosPago.FirstOrDefaultAsync(x => x.Id == method.Id);
            result.Should().NotBeNull();
            result!.Activo.Should().BeFalse(); // Soft-deleted
        }

        [Fact]
        public async Task Should_HardDelete_When_MethodHasNoAssociatedPayments()
        {
            // Arrange
            var method = new CatalogoMetodoPago("Euro Cash", "Euro", 1);
            _context.CatalogoMetodosPago.Add(method);
            await _context.SaveChangesAsync();

            var handler = new DeletePaymentMethodCommandHandler(_context);
            var command = new DeletePaymentMethodCommand { Id = method.Id };

            // Act
            var success = await handler.Handle(command, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            var result = await _context.CatalogoMetodosPago.FirstOrDefaultAsync(x => x.Id == method.Id);
            result.Should().BeNull(); // Hard-deleted
        }

        // ==========================================
        // 2. CONVERSION ENGINE TESTS (DetallePago via RegistrarReciboFactura)
        // ==========================================

        private async Task<(Guid PacienteId, Guid CuentaId, CajaDiaria Caja)> SeedBillingEnvironmentAsync()
        {
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-999999", "Paciente Juan", "0412-0000000");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            _context.PacientesAdmision.Add(paciente);

            var cuenta = new CuentaServicios(pacienteId, "cajero1", "Particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, cuentaId);
            
            // Add a sample service to the account to have a total balance
            cuenta.AgregarServicio(Guid.NewGuid(), "Consulta Medica", 100m, 0m, 1, "Medico", "cajero1");
            _context.CuentasServicios.Add(cuenta);

            var caja = new CajaDiaria(0m, 50m, "user-cajero", "cajero1");
            _context.CajasDiarias.Add(caja);

            await _context.SaveChangesAsync();

            return (pacienteId, cuentaId, caja);
        }

        [Fact]
        public async Task Should_CalculateEquivalentDirectly_When_GrupoMonedaIs1()
        {
            // Arrange
            var (pacienteId, cuentaId, caja) = await SeedBillingEnvironmentAsync();
            
            var zelleMethod = new CatalogoMetodoPago("Zelle USD", "Zelle", 1); // Grupo 1 (USD)
            _context.CatalogoMetodosPago.Add(zelleMethod);
            await _context.SaveChangesAsync();

            _cajaRepositoryMock.Setup(r => r.ObtenerCajaAbiertaNoTrackingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(caja);

            var account = await _context.CuentasServicios.FirstAsync(c => c.Id == cuentaId);
            _billingRepositoryMock.Setup(r => r.ObtenerCuentaPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _billingRepositoryMock.Setup(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
                .Returns(async () => { await _context.SaveChangesAsync(); });

            var command = new RegistrarReciboFacturaCommand
            {
                CuentaServicioId = cuentaId,
                CajeroUserId = "cajero1",
                TasaCambioDia = 50m,
                PagosMultidivisa = new List<DetallesPagoDto>
                {
                    new() { MetodoPago = "Zelle", ReferenciaBancaria = "Z-1", MontoAbonadoMoneda = 100m, EquivalenteAbonadoBase = 0m }
                }
            };

            var handler = new RegistrarReciboFacturaCommandHandler(
                _cajaRepositoryMock.Object,
                _billingRepositoryMock.Object,
                _context
            );

            // Act
            var reciboId = await handler.Handle(command, CancellationToken.None);

            // Assert
            reciboId.Should().NotBeEmpty();
            var details = await _context.DetallesPago.Where(x => x.ReciboFacturaId == reciboId).ToListAsync();
            details.Should().ContainSingle();
            details[0].MetodoPago.Should().Be("Zelle");
            details[0].MontoAbonadoMoneda.Should().Be(100m);
            details[0].EquivalenteAbonadoBase.Should().Be(100m); // Exact 1:1 match
            details[0].TasaCambioAplicada.Should().Be(1m); // USD has rate 1.0
            details[0].UsuarioCarga.Should().Be("cajero1");
        }

        [Fact]
        public async Task Should_CalculateEquivalentDividedByRate_When_GrupoMonedaIs2()
        {
            // Arrange
            var (pacienteId, cuentaId, caja) = await SeedBillingEnvironmentAsync();
            
            var bsMethod = new CatalogoMetodoPago("Efectivo BS", "Efectivo BS", 2); // Grupo 2 (VES)
            _context.CatalogoMetodosPago.Add(bsMethod);
            await _context.SaveChangesAsync();

            _cajaRepositoryMock.Setup(r => r.ObtenerCajaAbiertaNoTrackingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(caja);

            var account = await _context.CuentasServicios.FirstAsync(c => c.Id == cuentaId);
            _billingRepositoryMock.Setup(r => r.ObtenerCuentaPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _billingRepositoryMock.Setup(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
                .Returns(async () => { await _context.SaveChangesAsync(); });

            var command = new RegistrarReciboFacturaCommand
            {
                CuentaServicioId = cuentaId,
                CajeroUserId = "cajero1",
                TasaCambioDia = 50m, // Rate = 50 VES per USD
                PagosMultidivisa = new List<DetallesPagoDto>
                {
                    new() { MetodoPago = "Efectivo BS", ReferenciaBancaria = "B-1", MontoAbonadoMoneda = 5000m, EquivalenteAbonadoBase = 0m }
                }
            };

            var handler = new RegistrarReciboFacturaCommandHandler(
                _cajaRepositoryMock.Object,
                _billingRepositoryMock.Object,
                _context
            );

            // Act
            var reciboId = await handler.Handle(command, CancellationToken.None);

            // Assert
            reciboId.Should().NotBeEmpty();
            var details = await _context.DetallesPago.Where(x => x.ReciboFacturaId == reciboId).ToListAsync();
            details.Should().ContainSingle();
            details[0].MetodoPago.Should().Be("Efectivo BS");
            details[0].MontoAbonadoMoneda.Should().Be(5000m);
            details[0].EquivalenteAbonadoBase.Should().Be(100m); // 5000 / 50 = 100 USD
            details[0].TasaCambioAplicada.Should().Be(50m);
            details[0].UsuarioCarga.Should().Be("cajero1");
        }

        [Fact]
        public async Task Should_ThrowException_When_ExchangeRateIsZeroOrNegativeAndGrupoMonedaIs2()
        {
            // Arrange
            var (pacienteId, cuentaId, caja) = await SeedBillingEnvironmentAsync();
            
            var bsMethod = new CatalogoMetodoPago("Efectivo BS", "Efectivo BS", 2); // Grupo 2 (VES)
            _context.CatalogoMetodosPago.Add(bsMethod);
            await _context.SaveChangesAsync();

            _cajaRepositoryMock.Setup(r => r.ObtenerCajaAbiertaNoTrackingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(caja);

            var account = await _context.CuentasServicios.FirstAsync(c => c.Id == cuentaId);
            _billingRepositoryMock.Setup(r => r.ObtenerCuentaPorIdAsync(cuentaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            var command = new RegistrarReciboFacturaCommand
            {
                CuentaServicioId = cuentaId,
                CajeroUserId = "cajero1",
                TasaCambioDia = 0m, // Invalid rate (0)
                PagosMultidivisa = new List<DetallesPagoDto>
                {
                    new() { MetodoPago = "Efectivo BS", ReferenciaBancaria = "B-1", MontoAbonadoMoneda = 5000m }
                }
            };

            var handler = new RegistrarReciboFacturaCommandHandler(
                _cajaRepositoryMock.Object,
                _billingRepositoryMock.Object,
                _context
            );

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("La tasa de cambio del día debe ser mayor a cero para pagos en Bolívares.");
        }
    }
}
