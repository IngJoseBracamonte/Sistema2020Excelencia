using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class TrasladarPacienteCommandHandlerTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly TrasladarPacienteCommandHandler _handler;

        public TrasladarPacienteCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _handler = new TrasladarPacienteCommandHandler(_context);
        }

        [Fact]
        public async Task Should_TransferPaciente_ClosingActiveAccount_And_CreatingChildAccountWithTransferItem()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuentaActual = new CuentaServicios(pacienteId, "Nurse", EstadoConstants.Emergencia);
            await _context.CuentasServicios.AddAsync(cuentaActual);

            var servicioEstanciaUci = new ServicioClinico("HOSP-UCI-01", "Día Cama Cuidados Intensivos", 150.00m, "Hospitalario");
            await _context.ServiciosClinicos.AddAsync(servicioEstanciaUci);

            var sedeId = Guid.NewGuid();
            var camaUci = new AreaClinica(sedeId, "UCI-01", "Cama UCI 1", true, servicioEstanciaUci.Id);
            await _context.AreasClinicas.AddAsync(camaUci);
            await _context.SaveChangesAsync();

            var command = new TrasladarPacienteCommand
            {
                PacienteId = pacienteId,
                NuevoTipoIngreso = "UCI",
                NuevaAreaClinicaId = camaUci.Id,
                NuevoConvenioId = 1,
                UsuarioTraslado = "NurseUser",
                EsEgreso = false
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaCerradaId.Should().Be(cuentaActual.Id);
            result.NuevaCuentaId.Should().NotBeNull();

            // Verificar estados en DB
            var cuentaAnterior = await _context.CuentasServicios.FindAsync(cuentaActual.Id);
            cuentaAnterior!.Estado.Should().Be(EstadoConstants.Facturada);

            var nuevaCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.NuevaCuentaId);

            nuevaCuenta.Should().NotBeNull();
            nuevaCuenta!.Estado.Should().Be(EstadoConstants.Abierta);
            nuevaCuenta.TipoIngreso.Should().Be("UCI");
            nuevaCuenta.CuentaPrincipalId.Should().Be(cuentaActual.Id);
            nuevaCuenta.Detalles.Should().HaveCount(1);

            var detalle = nuevaCuenta.Detalles.First();
            detalle.Descripcion.Should().Contain("Traslado a UCI");
            detalle.Descripcion.Should().Contain("Cama UCI 1");
            detalle.Precio.Should().Be(150.00m);
            detalle.Cantidad.Should().Be(1m);
        }

        [Fact]
        public async Task Should_TransferPacienteToEmergencia_WithZeroDollarTransferItem()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuentaActual = new CuentaServicios(pacienteId, "Nurse", EstadoConstants.Hospitalizacion);
            await _context.CuentasServicios.AddAsync(cuentaActual);
            await _context.SaveChangesAsync();

            var command = new TrasladarPacienteCommand
            {
                PacienteId = pacienteId,
                NuevoTipoIngreso = "Emergencia",
                UsuarioTraslado = "NurseUser",
                EsEgreso = false
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var nuevaCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.NuevaCuentaId);

            nuevaCuenta.Should().NotBeNull();
            nuevaCuenta!.Detalles.Should().HaveCount(1);

            var detalle = nuevaCuenta.Detalles.First();
            detalle.Descripcion.Should().Contain("Traslado a Emergencia");
            detalle.Precio.Should().Be(0.00m);
            detalle.Cantidad.Should().Be(1m);
        }

        [Fact]
        public async Task Should_DischargePaciente_ClosingActiveAccount_Without_NewAccount()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuentaActual = new CuentaServicios(pacienteId, "Nurse", EstadoConstants.Hospitalizacion);
            await _context.CuentasServicios.AddAsync(cuentaActual);
            await _context.SaveChangesAsync();

            var command = new TrasladarPacienteCommand
            {
                PacienteId = pacienteId,
                NuevoTipoIngreso = "",
                NuevoConvenioId = null,
                UsuarioTraslado = "NurseUser",
                EsEgreso = true
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaCerradaId.Should().Be(cuentaActual.Id);
            result.NuevaCuentaId.Should().BeNull();

            var cuentaAnterior = await _context.CuentasServicios.FindAsync(cuentaActual.Id);
            cuentaAnterior!.Estado.Should().Be(EstadoConstants.Facturada);
        }
    }
}
