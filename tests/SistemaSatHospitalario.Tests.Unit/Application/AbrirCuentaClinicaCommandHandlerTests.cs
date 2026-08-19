using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class AbrirCuentaClinicaCommandHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly AbrirCuentaClinicaCommandHandler _handler;

        public AbrirCuentaClinicaCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _handler = new AbrirCuentaClinicaCommandHandler(_context, NullLogger<AbrirCuentaClinicaCommandHandler>.Instance);
        }

        [Fact]
        public async Task Should_CreateAccountWithZeroDollarAdmissionItem_When_AdmittingToEmergencia()
        {
            // Arrange
            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0414-1234567");
            await _context.PacientesAdmision.AddAsync(paciente);
            await _context.SaveChangesAsync();

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Emergencia",
                UsuarioCarga = "EnfermeraUser"
            };

            // Act
            var cuentaId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            cuentaId.Should().NotBeEmpty();
            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == cuentaId);

            cuenta.Should().NotBeNull();
            cuenta!.Estado.Should().Be(EstadoConstants.Abierta);
            cuenta.TipoIngreso.Should().Be("Emergencia");
            cuenta.Detalles.Should().HaveCount(1);

            var detalle = cuenta.Detalles.First();
            detalle.Descripcion.Should().Contain("Ingreso - Emergencia");
            detalle.Precio.Should().Be(0.00m);
            detalle.Cantidad.Should().Be(1m);
            detalle.TipoServicio.Should().Be("Servicio");
        }

        [Fact]
        public async Task Should_CreateAccountWithBedTariffAdmissionItem_When_AdmittingToHospitalizacionWithBed()
        {
            // Arrange
            var paciente = new PacienteAdmision("V-87654321", "Maria Rodriguez", "0412-7654321");
            await _context.PacientesAdmision.AddAsync(paciente);

            var servicioEstancia = new ServicioClinico("HOSP-HAB-01", "Día Hospitalización Habitación Estándar", 75.00m, "Hospitalario");
            await _context.ServiciosClinicos.AddAsync(servicioEstancia);

            var sedeId = Guid.NewGuid();
            var cama = new AreaClinica(sedeId, "HAB-101", "Habitación 101", true, servicioEstancia.Id);
            await _context.AreasClinicas.AddAsync(cama);
            await _context.SaveChangesAsync();

            var command = new AbrirCuentaClinicaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Hospitalizacion",
                AreaClinicaId = cama.Id,
                UsuarioCarga = "AdmisionUser"
            };

            // Act
            var cuentaId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            cuentaId.Should().NotBeEmpty();
            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == cuentaId);

            cuenta.Should().NotBeNull();
            cuenta!.Estado.Should().Be(EstadoConstants.Abierta);
            cuenta.Detalles.Should().HaveCount(1);

            var detalle = cuenta.Detalles.First();
            detalle.Descripcion.Should().Contain("Ingreso - Hospitalizacion");
            detalle.Descripcion.Should().Contain("Habitación 101");
            detalle.Precio.Should().Be(75.00m);
            detalle.Cantidad.Should().Be(1m);
            detalle.AreaClinicaId.Should().Be(cama.Id);
        }
    }
}
