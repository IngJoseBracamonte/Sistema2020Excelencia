using System;
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
        public async Task Should_TransferPaciente_ClosingActiveAccount_And_CreatingChildAccount()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var cuentaActual = new CuentaServicios(pacienteId, "Nurse", EstadoConstants.Emergencia);
            await _context.CuentasServicios.AddAsync(cuentaActual);
            await _context.SaveChangesAsync();

            var command = new TrasladarPacienteCommand
            {
                PacienteId = pacienteId,
                NuevoTipoIngreso = "UCI",
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
            cuentaAnterior.Estado.Should().Be(EstadoConstants.Facturada);

            var nuevaCuenta = await _context.CuentasServicios.FindAsync(result.NuevaCuentaId);
            nuevaCuenta.Estado.Should().Be(EstadoConstants.Abierta);
            nuevaCuenta.TipoIngreso.Should().Be("UCI");
            nuevaCuenta.CuentaPrincipalId.Should().Be(cuentaActual.Id);
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
            cuentaAnterior.Estado.Should().Be(EstadoConstants.Facturada);
        }
    }
}
