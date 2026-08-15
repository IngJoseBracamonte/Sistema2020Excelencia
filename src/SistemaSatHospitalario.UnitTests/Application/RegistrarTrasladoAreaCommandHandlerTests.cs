using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class RegistrarTrasladoAreaCommandHandlerTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task Handle_TrasladoExitosoConMedicoTratanteOpcionalNulo_ProcesaCorrectamente()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var sedeId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-99887766", "Paciente Traslado", "0412-1112233", null, DateTime.Today.AddYears(-40));
            var camaOrigen = new AreaClinica(sedeId, "P-101", "Pabellón Piso 1", false);
            var camaDestino = new AreaClinica(sedeId, "UCI-01", "Unidad Cuidados Intensivos 1", false);

            camaOrigen.MarcarComoOcupada();

            await context.PacientesAdmision.AddAsync(paciente);
            await context.AreasClinicas.AddRangeAsync(camaOrigen, camaDestino);
            await context.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Hospitalizacion", null, camaOrigen.Id);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarTrasladoAreaCommandHandler(context);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "UCI",
                CamaDestinoId = camaDestino.Id,
                CantidadHoras = 24,
                CambiaMedicoTratante = false,
                NuevoMedicoId = null, // Nullable Guid explícito
                Observacion = "Traslado a UCI por monitoreo continuo",
                MontoACobrarUsd = 600m,
                UsuarioTraslado = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(camaDestino.Id, result.CamaDestinoId);
            Assert.Equal(600m, result.MontoCargadoUsd);

            var cuentaActualizada = await context.CuentasServicios.Include(c => c.Detalles).FirstAsync(c => c.Id == cuenta.Id);
            Assert.Equal(camaDestino.Id, cuentaActualizada.AreaClinicaId);

            var camaOrigenActualizada = await context.AreasClinicas.FirstAsync(a => a.Id == camaOrigen.Id);
            Assert.Equal(EstadoUbicacion.Disponible, camaOrigenActualizada.Estado);

            var camaDestinoActualizada = await context.AreasClinicas.FirstAsync(a => a.Id == camaDestino.Id);
            Assert.Equal(EstadoUbicacion.Ocupada, camaDestinoActualizada.Estado);
        }

        [Fact]
        public async Task Handle_TrasladoAEmergencia_AplicaCodigoHospEmgYDetalleCorrecto()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var sedeId = Guid.NewGuid();

            var paciente = new PacienteAdmision("V-11223344", "Paciente Emergencia", "0424-5556677", null, DateTime.Today.AddYears(-30));
            var camaOrigen = new AreaClinica(sedeId, "HOSP-201", "Habitación 201", false);
            var camaDestino = new AreaClinica(sedeId, "EMG-BOX-1", "Box de Emergencia 1", false);
            var servicioEmg = new ServicioClinico("HOSP-EMG-01", "Cargo por Traslado / Estancia Emergencia", 300.00m, "Hospitalario")
            {
                HonorariumCategory = "HOSPITALARIO"
            };

            camaOrigen.MarcarComoOcupada();

            await context.PacientesAdmision.AddAsync(paciente);
            await context.AreasClinicas.AddRangeAsync(camaOrigen, camaDestino);
            await context.ServiciosClinicos.AddAsync(servicioEmg);
            await context.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Hospitalizacion", null, camaOrigen.Id);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarTrasladoAreaCommandHandler(context);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "Observación de Emergencia",
                CamaDestinoId = camaDestino.Id,
                CantidadHoras = 12,
                CambiaMedicoTratante = false,
                Observacion = "Traslado urgente a observación de emergencia",
                MontoACobrarUsd = 300m,
                UsuarioTraslado = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(camaDestino.Id, result.CamaDestinoId);
            Assert.Equal(300m, result.MontoCargadoUsd);

            var cuentaActualizada = await context.CuentasServicios.Include(c => c.Detalles).FirstAsync(c => c.Id == cuenta.Id);
            Assert.Equal(camaDestino.Id, cuentaActualizada.AreaClinicaId);
            Assert.Equal("Observación de Emergencia", cuentaActualizada.SubAreaClinica);
            Assert.Single(cuentaActualizada.Detalles);

            var detalle = cuentaActualizada.Detalles.First();
            Assert.Equal(servicioEmg.Id, detalle.ServicioId);
            Assert.Contains("Emergencia", detalle.Descripcion);
            Assert.Equal(300m, detalle.Precio);
        }

        [Fact]
        public async Task Handle_CuentaNoExistente_LanzaExcepcion()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var handler = new RegistrarTrasladoAreaCommandHandler(context);

            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = Guid.NewGuid(),
                AreaDestino = "UCI",
                CamaDestinoId = Guid.NewGuid(),
                MontoACobrarUsd = 500m
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
