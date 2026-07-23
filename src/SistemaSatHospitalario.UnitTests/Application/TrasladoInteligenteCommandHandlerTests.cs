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
    public class TrasladoInteligenteCommandHandlerTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SatHospitalarioDbContext(options);

            // Seed tipos de servicio
            context.TiposServicio.AddRange(
                new TipoServicio(TipoServicioConstants.Medico, "Medico", "MED"),
                new TipoServicio(99, "Hospitalario", "HOSP")
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Handle_CambioCama_NoAlteraSaldoFinanciero()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var sede = new Sede("S01", "Sede Principal", true);
            var camaOrigen = new AreaClinica(sede.Id, "EMG-01", "Emergencia Cama 1", true);
            var camaDestino = new AreaClinica(sede.Id, "EMG-02", "Emergencia Cama 2", true);
            camaOrigen.MarcarComoOcupada();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, DateTime.Today.AddYears(-30));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Emergencia", null, camaOrigen.Id, "Emergencia");

            await context.Sedes.AddAsync(sede);
            await context.AreasClinicas.AddRangeAsync(camaOrigen, camaDestino);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarCambioCamaCommandHandler(context);
            var command = new RegistrarCambioCamaCommand
            {
                CuentaId = cuenta.Id,
                CamaDestinoId = camaDestino.Id,
                UsuarioCarga = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(camaDestino.Id, result.CamaDestinoId);

            var cuentaActualizada = await context.CuentasServicios.Include(c => c.Detalles).FirstAsync(c => c.Id == cuenta.Id);
            Assert.Equal(camaDestino.Id, cuentaActualizada.AreaClinicaId);
            Assert.Empty(cuentaActualizada.Detalles); // $0 USD, sin detalles
            Assert.Equal(0m, cuentaActualizada.Detalles.Sum(d => d.ObtenerSubtotal()));

            var camaOrigenDb = await context.AreasClinicas.FirstAsync(a => a.Id == camaOrigen.Id);
            Assert.Equal(EstadoUbicacion.Disponible, camaOrigenDb.Estado);

            var camaDestinoDb = await context.AreasClinicas.FirstAsync(a => a.Id == camaDestino.Id);
            Assert.Equal(EstadoUbicacion.Ocupada, camaDestinoDb.Estado);
        }

        [Fact]
        public async Task Handle_TrasladoArea_AplicaMontoSobreescrito()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var sede = new Sede("S01", "Sede Principal", true);
            var camaEmg = new AreaClinica(sede.Id, "EMG-01", "Emergencia Cama 1", true);
            var camaUci = new AreaClinica(sede.Id, "UCI-01", "UCI Cama 1", true);
            camaEmg.MarcarComoOcupada();

            var catalogUci = new ServicioClinico("HOSP-UCI-01", "Cargo por Traslado / Estancia Unidad de Cuidados Intensivos", 600.00m, "Hospitalario")
            {
                HonorariumCategory = "HOSPITALARIO"
            };

            var paciente = new PacienteAdmision("V-87654321", "Maria Lopez", "0414-9876543", null, DateTime.Today.AddYears(-25));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Emergencia", null, camaEmg.Id, "Emergencia");

            await context.Sedes.AddAsync(sede);
            await context.AreasClinicas.AddRangeAsync(camaEmg, camaUci);
            await context.ServiciosClinicos.AddAsync(catalogUci);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarTrasladoAreaCommandHandler(context);
            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "UCI",
                CamaDestinoId = camaUci.Id,
                CantidadHoras = 12,
                CambiaMedicoTratante = false,
                Observacion = "Traslado urgente por descompensación",
                MontoACobrarUsd = 500.00m, // Sobreescrito manualmente (Tarifa base $600 -> $500)
                UsuarioTraslado = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(500.00m, result.MontoCargadoUsd);

            var cuentaActualizada = await context.CuentasServicios.Include(c => c.Detalles).FirstAsync(c => c.Id == cuenta.Id);
            Assert.Equal(camaUci.Id, cuentaActualizada.AreaClinicaId);
            Assert.Single(cuentaActualizada.Detalles);

            var detalle = cuentaActualizada.Detalles.First();
            Assert.Equal(catalogUci.Id, detalle.ServicioId);
            Assert.Equal(500.00m, detalle.Precio);
            Assert.Equal(500.00m, detalle.ObtenerSubtotal());
        }

        [Fact]
        public async Task Handle_TrasladoArea_CambiaMedicoTratante_ActualizaMedicoEnCuenta()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var sede = new Sede("S01", "Sede Principal", true);
            var camaEmg = new AreaClinica(sede.Id, "EMG-01", "Emergencia Cama 1", true);
            var camaHos = new AreaClinica(sede.Id, "HOS-101", "Hospitalización 101", true);
            camaEmg.MarcarComoOcupada();

            var nuevoMedicoId = Guid.NewGuid();
            var paciente = new PacienteAdmision("V-11223344", "Carlos Ruiz", "0416-1122334", null, DateTime.Today.AddYears(-40));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Emergencia", null, camaEmg.Id, "Emergencia");

            await context.Sedes.AddAsync(sede);
            await context.AreasClinicas.AddRangeAsync(camaEmg, camaHos);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarTrasladoAreaCommandHandler(context);
            var command = new RegistrarTrasladoAreaCommand
            {
                CuentaId = cuenta.Id,
                AreaDestino = "HOSPITALIZACION",
                CamaDestinoId = camaHos.Id,
                CantidadHoras = 24,
                CambiaMedicoTratante = true,
                NuevoMedicoId = nuevoMedicoId,
                Observacion = "Pase a piso",
                MontoACobrarUsd = 450.00m,
                UsuarioTraslado = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);

            var cuentaActualizada = await context.CuentasServicios.FirstAsync(c => c.Id == cuenta.Id);
            Assert.Equal(nuevoMedicoId, cuentaActualizada.MedicoId);
        }
    }
}
