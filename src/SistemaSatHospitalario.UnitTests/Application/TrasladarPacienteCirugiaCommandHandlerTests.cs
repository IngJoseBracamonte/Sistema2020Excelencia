using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class TrasladarPacienteCirugiaCommandHandlerTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task Handle_TrasladoPreQuirurgicoAQuirofano_RetieneCamaOrigenYOcupaQuirofano()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var sedeHospId = Guid.NewGuid();
            var sedeCirugiaId = Guid.NewGuid();

            var sedeHosp = new Sede("HOSP", "Área de Hospitalización", false, sedeHospId);
            var sedeCir = new Sede("CIR", "Área Quirúrgica", false, sedeCirugiaId);

            var camaHosp = new AreaClinica(sedeHospId, "HAB-101", "Habitación 101");
            var quirofano1 = new AreaClinica(sedeCirugiaId, "QX-1", "Quirófano 1");

            camaHosp.MarcarComoOcupada();

            await context.Sedes.AddRangeAsync(sedeHosp, sedeCir);
            await context.AreasClinicas.AddRangeAsync(camaHosp, quirofano1);

            var paciente = new PacienteAdmision("V-12345678", "Juan Pérez", "0414-1234567", null, DateTime.Today.AddYears(-30));
            var especialidad = new Especialidad("Cirugía General");
            var medico = new Medico("Dr. Cirujano General", especialidad.Id, 150m);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.Especialidades.AddAsync(especialidad);
            await context.Medicos.AddAsync(medico);
            await context.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "admin", "Hospitalizacion", null, camaHosp.Id, "Hospitalización - Habitación 101");
            cuenta.AsignarCamaRetenida(camaHosp.Id);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var orden = new OrdenCirugia(
                cuenta.Id,
                paciente.Id,
                "Apendicectomía Laparoscópica",
                1200m,
                medico.Id,
                DateTime.UtcNow.AddHours(2),
                "admin",
                camaHosp.Id,
                "Quirófano 1",
                "General",
                false,
                0,
                sedeCirugiaId);

            await context.OrdenesCirugia.AddAsync(orden);
            await context.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<TrasladarPacienteCirugiaCommandHandler>>();
            var handler = new TrasladarPacienteCirugiaCommandHandler(context, loggerMock.Object);

            var command = new TrasladarPacienteCirugiaCommand
            {
                OrdenCirugiaId = orden.Id,
                SedeDestinoId = sedeCirugiaId,
                AreaClinicaCamaId = quirofano1.Id,
                Observacion = "Ingreso a quirófano para inicio de acto quirúrgico",
                UsuarioId = "cirujano1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            var ordenActualizada = await context.OrdenesCirugia.FindAsync(orden.Id);
            var qxActualizado = await context.AreasClinicas.FindAsync(quirofano1.Id);
            var camaHospActualizada = await context.AreasClinicas.FindAsync(camaHosp.Id);
            var cuentaActualizada = await context.CuentasServicios.FindAsync(cuenta.Id);

            Assert.NotNull(ordenActualizada);
            Assert.Equal(EstadoUbicacion.Ocupada, qxActualizado!.Estado);
            // La cama de hospitalización debe permanecer ocupada/retenida para no perder el cupo
            Assert.Equal(EstadoUbicacion.Ocupada, camaHospActualizada!.Estado);
            Assert.Equal(camaHosp.Id, ordenActualizada.AreaClinicaOrigenId);
            Assert.Equal("EnEspera", ordenActualizada.Estado);
            Assert.Single(context.CirugiaLogs.Where(l => l.OrdenCirugiaId == orden.Id && l.Evento == "Traslado"));
        }

        [Fact]
        public async Task Handle_TrasladoPostQuirurgicoAUCI_LiberaQuirofanoYLiberaCamaPreviaRetenida()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var sedeCirugiaId = Guid.NewGuid();
            var sedeUciId = Guid.NewGuid();

            var sedeCir = new Sede("CIR", "Área Quirúrgica", false, sedeCirugiaId);
            var sedeUci = new Sede("UCI", "Unidad de Cuidados Intensivos", false, sedeUciId);

            var camaHospPrevia = new AreaClinica(Guid.NewGuid(), "HAB-102", "Habitación 102");
            var quirofano1 = new AreaClinica(sedeCirugiaId, "QX-1", "Quirófano 1");
            var camaUci1 = new AreaClinica(sedeUciId, "UCI-01", "Cama UCI 1");

            camaHospPrevia.MarcarComoOcupada();
            quirofano1.MarcarComoOcupada();

            await context.Sedes.AddRangeAsync(sedeCir, sedeUci);
            await context.AreasClinicas.AddRangeAsync(camaHospPrevia, quirofano1, camaUci1);

            var paciente = new PacienteAdmision("V-87654321", "María Gómez", "0412-9876543", null, DateTime.Today.AddYears(-45));
            var especialidad = new Especialidad("Cardiología");
            var medico = new Medico("Dr. Cirujano Cardiovascular", especialidad.Id, 300m);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.Especialidades.AddAsync(especialidad);
            await context.Medicos.AddAsync(medico);
            await context.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "admin", "Hospitalizacion", null, quirofano1.Id, "Área Quirúrgica - Quirófano 1");
            cuenta.AsignarCamaRetenida(camaHospPrevia.Id);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var orden = new OrdenCirugia(
                cuenta.Id,
                paciente.Id,
                "Cirugía Cardiovascular",
                3500m,
                medico.Id,
                DateTime.UtcNow,
                "admin",
                quirofano1.Id,
                "Quirófano 1",
                "General",
                false,
                0,
                sedeCirugiaId);

            orden.GuardarUbicacionOrigen(camaHospPrevia.Id, camaHospPrevia.SedeId);
            orden.IniciarCirugia("cirujano1");
            orden.FinalizarCirugia("cirujano1");

            await context.OrdenesCirugia.AddAsync(orden);
            await context.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<TrasladarPacienteCirugiaCommandHandler>>();
            var handler = new TrasladarPacienteCirugiaCommandHandler(context, loggerMock.Object);

            var command = new TrasladarPacienteCirugiaCommand
            {
                OrdenCirugiaId = orden.Id,
                SedeDestinoId = sedeUciId,
                AreaClinicaCamaId = camaUci1.Id,
                Observacion = "Traslado post-quirúrgico inmediato a UCI por monitoreo hemodinámico",
                UsuarioId = "anestesiologo1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            var qxActualizado = await context.AreasClinicas.FindAsync(quirofano1.Id);
            var camaHospActualizada = await context.AreasClinicas.FindAsync(camaHospPrevia.Id);
            var camaUciActualizada = await context.AreasClinicas.FindAsync(camaUci1.Id);
            var cuentaActualizada = await context.CuentasServicios.FindAsync(cuenta.Id);

            // Quirófano debe quedar liberado para la siguiente cirugía
            Assert.Equal(EstadoUbicacion.Disponible, qxActualizado!.Estado);
            // La cama previa de hospitalización debe liberarse porque el paciente pasó a UCI
            Assert.Equal(EstadoUbicacion.Disponible, camaHospActualizada!.Estado);
            // La cama de UCI debe quedar ocupada
            Assert.Equal(EstadoUbicacion.Ocupada, camaUciActualizada!.Estado);

            Assert.Equal(camaUci1.Id, cuentaActualizada!.AreaClinicaId);
            Assert.Equal(camaUci1.Id, cuentaActualizada.CamaRetenidaId);
        }

        [Fact]
        public async Task Handle_UsuarioVacio_AplicaFallbackDefensivoSinLanzarExcepcion()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var sedeId = Guid.NewGuid();
            var sede = new Sede("HOSP", "Hospitalización", false, sedeId);
            var cama = new AreaClinica(sedeId, "HAB-201", "Habitación 201");

            await context.Sedes.AddAsync(sede);
            await context.AreasClinicas.AddAsync(cama);

            var paciente = new PacienteAdmision("V-11223344", "Carlos Ruiz", "0424-5556677", null, DateTime.Today.AddYears(-25));
            var especialidad = new Especialidad("Traumatología");
            var medico = new Medico("Dr. Traumatólogo", especialidad.Id, 100m);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.Especialidades.AddAsync(especialidad);
            await context.Medicos.AddAsync(medico);
            await context.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "admin", "Hospitalizacion", null, cama.Id);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var orden = new OrdenCirugia(
                cuenta.Id,
                paciente.Id,
                "Traumatología",
                1000m,
                medico.Id,
                DateTime.UtcNow,
                "admin");

            await context.OrdenesCirugia.AddAsync(orden);
            await context.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<TrasladarPacienteCirugiaCommandHandler>>();
            var handler = new TrasladarPacienteCirugiaCommandHandler(context, loggerMock.Object);

            var command = new TrasladarPacienteCirugiaCommand
            {
                OrdenCirugiaId = orden.Id,
                SedeDestinoId = sedeId,
                AreaClinicaCamaId = cama.Id,
                UsuarioId = "" // Usuario vacío para probar fallback defensivo
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            var log = await context.CirugiaLogs.FirstOrDefaultAsync(l => l.OrdenCirugiaId == orden.Id && l.Evento == "Traslado");
            Assert.NotNull(log);
            Assert.Equal("Sistema", log.UsuarioId);
        }
    }
}
