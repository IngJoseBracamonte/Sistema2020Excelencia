using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Admision
{
    public class BillingTimezoneTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly SatHospitalarioDbContext _context;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public BillingTimezoneTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _context.Database.EnsureCreated();

            _identityServiceMock = new Mock<IIdentityService>();
            _identityServiceMock.Setup(i => i.GetUsersAsync()).ReturnsAsync(new List<UserDto>());
        }

        [Fact]
        public async Task Query_ShouldReturnRecordsMatchingLocalTime_EvenIfUtcDateIsDifferent()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var paciente = new PacienteAdmision("V-123456", "Paciente Test TZ", "0412-1111111");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente, pacienteId);
            _context.PacientesAdmision.Add(paciente);

            var cuentaId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "Particular", "particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, cuentaId);
            _context.CuentasServicios.Add(cuenta);

            // Record 1: May 27th 19:30 local time -> UTC 2026-05-27 23:30:00 (UTC date matches local)
            var service1 = new DetalleServicioCuenta(cuentaId, Guid.NewGuid(), "Consulta 1", 50m, 15m, 1, "CONSULTA", "test-admin");
            typeof(DetalleServicioCuenta).GetProperty(nameof(DetalleServicioCuenta.FechaCarga))?.SetValue(service1, new DateTime(2026, 5, 27, 23, 30, 0, DateTimeKind.Utc));
            _context.DetallesServicioCuenta.Add(service1);

            // Record 2: May 27th 21:00 local time -> UTC 2026-05-28 01:00:00 (UTC date is May 28th, but local is May 27th!)
            var service2 = new DetalleServicioCuenta(cuentaId, Guid.NewGuid(), "Consulta 2", 50m, 15m, 1, "CONSULTA", "test-admin");
            typeof(DetalleServicioCuenta).GetProperty(nameof(DetalleServicioCuenta.FechaCarga))?.SetValue(service2, new DateTime(2026, 5, 28, 1, 0, 0, DateTimeKind.Utc));
            _context.DetallesServicioCuenta.Add(service2);

            // Record 3: May 28th 10:00 local time -> UTC 2026-05-28 14:00:00 (Both local and UTC are May 28th)
            var service3 = new DetalleServicioCuenta(cuentaId, Guid.NewGuid(), "Consulta 3", 50m, 15m, 1, "CONSULTA", "test-admin");
            typeof(DetalleServicioCuenta).GetProperty(nameof(DetalleServicioCuenta.FechaCarga))?.SetValue(service3, new DateTime(2026, 5, 28, 14, 0, 0, DateTimeKind.Utc));
            _context.DetallesServicioCuenta.Add(service3);

            await _context.SaveChangesAsync();

            var handler = new GetExpedienteFacturacionQueryHandler(_context, _identityServiceMock.Object);

            // Act: Query for date May 27th
            var query = new GetExpedienteFacturacionQuery
            {
                StartDate = new DateTime(2026, 5, 27),
                EndDate = new DateTime(2026, 5, 27),
                FilterType = "todo"
            };
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: Should contain only Consulta 1 (since timezone shift is removed)
            result.Should().HaveCount(1);
            result.Select(x => x.Estudio).Should().Contain("Consulta 1");
            result.Select(x => x.Estudio).Should().NotContain("Consulta 2");
            result.Select(x => x.Estudio).Should().NotContain("Consulta 3");
        }

        [Fact]
        public async Task Query_WithSoloCompromisoFilter_ShouldReturnOnlyRecordsWithCompromisoGenerado()
        {
            // Arrange
            var pacienteId1 = Guid.NewGuid();
            var paciente1 = new PacienteAdmision("V-111111", "Paciente Con Compromiso", "0412-1111111");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente1, pacienteId1);
            _context.PacientesAdmision.Add(paciente1);

            var pacienteId2 = Guid.NewGuid();
            var paciente2 = new PacienteAdmision("V-222222", "Paciente Sin Compromiso", "0412-2222222");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(paciente2, pacienteId2);
            _context.PacientesAdmision.Add(paciente2);

            var cuentaId1 = Guid.NewGuid();
            var cuenta1 = new CuentaServicios(pacienteId1, "Particular", "particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta1, cuentaId1);
            _context.CuentasServicios.Add(cuenta1);

            var cuentaId2 = Guid.NewGuid();
            var cuenta2 = new CuentaServicios(pacienteId2, "Particular", "particular");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta2, cuentaId2);
            _context.CuentasServicios.Add(cuenta2);

            var service1 = new DetalleServicioCuenta(cuentaId1, Guid.NewGuid(), "Consulta Con Compromiso", 50m, 15m, 1, "CONSULTA", "test-admin");
            _context.DetallesServicioCuenta.Add(service1);

            var service2 = new DetalleServicioCuenta(cuentaId2, Guid.NewGuid(), "Consulta Sin Compromiso", 50m, 15m, 1, "CONSULTA", "test-admin");
            _context.DetallesServicioCuenta.Add(service2);

            // Account Receivable (CuentaPorCobrar) with CompromisoGenerado = true
            var cxc1 = new CuentaPorCobrar(cuentaId1, pacienteId1, 50m, 0m);
            cxc1.MarcarCompromisoGenerado();
            _context.CuentasPorCobrar.Add(cxc1);

            // Account Receivable (CuentaPorCobrar) with CompromisoGenerado = false
            var cxc2 = new CuentaPorCobrar(cuentaId2, pacienteId2, 50m, 0m);
            _context.CuentasPorCobrar.Add(cxc2);

            await _context.SaveChangesAsync();

            var handler = new GetExpedienteFacturacionQueryHandler(_context, _identityServiceMock.Object);

            // Act: Query with SoloCompromiso = true
            var query = new GetExpedienteFacturacionQuery
            {
                SoloCompromiso = true,
                FilterType = "todo"
            };
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().Estudio.Should().Be("Consulta Con Compromiso");
            result.First().CompromisoGenerado.Should().BeTrue();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }
    }
}
