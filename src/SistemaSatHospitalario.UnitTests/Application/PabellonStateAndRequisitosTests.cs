using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.State;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class PabellonStateAndRequisitosTests
    {
        private DbContextOptions<SatHospitalarioDbContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void StatePattern_TransicionProgramadaAEnEsperaYEnCirugia_CambiaEstadoCorrectamente()
        {
            // Arrange
            var orden = new OrdenCirugia(
                Guid.NewGuid(), Guid.NewGuid(), "Apendicectomía", 800m, Guid.NewGuid(), DateTime.UtcNow, "admin");

            Assert.Equal("Programada", orden.Estado);
            Assert.IsType<ProgramadaState>(orden.CurrentState);

            // Act 1: Pasar a EnEspera
            orden.IniciarEspera("enfermero1");
            Assert.Equal("EnEspera", orden.Estado);
            Assert.IsType<EnEsperaState>(orden.CurrentState);

            // Act 2: Pasar a EnCirugia
            orden.IniciarCirugia("cirujano1");
            Assert.Equal("EnCirugia", orden.Estado);
            Assert.IsType<EnCirugiaState>(orden.CurrentState);

            // Act 3: Finalizar
            orden.FinalizarCirugia("cirujano1");
            Assert.Equal("Finalizado", orden.Estado);
            Assert.IsType<FinalizadoState>(orden.CurrentState);
        }

        [Fact]
        public void StatePattern_ReprogramarEnCirugia_LanzaExcepcion()
        {
            // Arrange
            var orden = new OrdenCirugia(
                Guid.NewGuid(), Guid.NewGuid(), "Colecistectomía", 1200m, Guid.NewGuid(), DateTime.UtcNow, "admin");

            orden.IniciarCirugia("cirujano1");

            // Act & Assert: Intentar reprogramar en plena cirugía debe fallar
            Assert.Throws<InvalidOperationException>(() =>
                orden.Reprogramar(DateTime.UtcNow.AddDays(1), "Emergencia médica", "admin"));
        }

        [Fact]
        public void Reprogramar_SinMotivo_LanzaArgumentException()
        {
            // Arrange
            var orden = new OrdenCirugia(
                Guid.NewGuid(), Guid.NewGuid(), "Hernioplastia", 900m, Guid.NewGuid(), DateTime.UtcNow, "admin");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                orden.Reprogramar(DateTime.UtcNow.AddDays(2), "", "admin"));
        }

        [Fact]
        public async Task CrearOrden_PoblaRequisitosActivosDelCatalogo()
        {
            // Arrange
            var options = GetInMemoryOptions();
            using var db = new SatHospitalarioDbContext(options);

            var req1 = new RequisitoCirugia("Exámenes de Laboratorio", "Perfil 20 y Tiempos de Coagulación", true);
            var req2 = new RequisitoCirugia("Ayuno 8 Horas", "Ayuno pre-quirúrgico estricto", true);
            var req3 = new RequisitoCirugia("Evaluación Cardiovascular", "Inactivo de prueba", false);

            await db.RequisitosCirugia.AddRangeAsync(req1, req2, req3);
            await db.SaveChangesAsync();

            var loggerMock = new Mock<ILogger<CrearOrdenCirugiaCommandHandler>>();
            var handler = new CrearOrdenCirugiaCommandHandler(db, loggerMock.Object);

            var command = new CrearOrdenCirugiaCommand
            {
                CuentaServicioId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                DescripcionCirugia = "Mastectomía",
                PrecioBaseUsd = 1500m,
                MedicoId = Guid.NewGuid(),
                FechaHoraProgramada = DateTime.UtcNow.AddDays(1),
                UsuarioCreacion = "admin_pabellon"
            };

            // Act
            var ordenId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var ordenBD = await db.OrdenesCirugia
                .Include(o => o.Requisitos)
                .Include(o => o.HistorialObservaciones)
                .FirstOrDefaultAsync(o => o.Id == ordenId);

            Assert.NotNull(ordenBD);
            Assert.Equal(2, ordenBD.Requisitos.Count); // Solo los 2 activos
            Assert.Single(ordenBD.HistorialObservaciones);
        }

        [Fact]
        public async Task ToggleRequisito_ActualizaEstadoCumplidoCorrectamente()
        {
            // Arrange
            var options = GetInMemoryOptions();
            Guid ordenId;
            Guid reqId;

            using (var seedDb = new SatHospitalarioDbContext(options))
            {
                var req = new RequisitoCirugia("Evaluación Anestésica", "Valoración pre-anestesia", true);
                await seedDb.RequisitosCirugia.AddAsync(req);

                var orden = new OrdenCirugia(
                    Guid.NewGuid(), Guid.NewGuid(), "Traumatología", 1100m, Guid.NewGuid(), DateTime.UtcNow, "admin");
                orden.AgregarRequisito(req.Id, false);

                await seedDb.OrdenesCirugia.AddAsync(orden);
                await seedDb.SaveChangesAsync();

                ordenId = orden.Id;
                reqId = req.Id;
            }

            using (var testDb = new SatHospitalarioDbContext(options))
            {
                var loggerMock = new Mock<ILogger<ToggleRequisitoCirugiaCommandHandler>>();
                
                var handler = new ToggleRequisitoCirugiaCommandHandler(
                    testDb,
                    loggerMock.Object
                );

                var command = new ToggleRequisitoCirugiaCommand
                {
                    OrdenCirugiaId = ordenId,
                    RequisitoCirugiaId = reqId,
                    Cumplido = true,
                    UsuarioId = "anestesiologo1"
                };

                // Act
                var success = await handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.True(success);

                var itemReqBD = await testDb.OrdenesCirugiaRequisitos
                    .FirstOrDefaultAsync(r => r.OrdenCirugiaId == ordenId && r.RequisitoCirugiaId == reqId);
                Assert.NotNull(itemReqBD);
                Assert.True(itemReqBD.Cumplido);
                Assert.Equal("anestesiologo1", itemReqBD.VerificadoPor);
            }
        }
    }
}