using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using MockQueryable.Moq;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class OrdenCirugiaTests
    {
        #region Domain Entity Tests

        [Fact]
        public void Constructor_ShouldCreateOrdenWithPendienteEjecucionState()
        {
            // Arrange
            var cuentaId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            var fecha = DateTime.UtcNow.AddDays(1);

            // Act
            var orden = new OrdenCirugia(cuentaId, pacienteId, "Colecistectomía Laparoscópica", 1500m, medicoId, fecha, "admin");

            // Assert
            Assert.NotEqual(Guid.Empty, orden.Id);
            Assert.Equal(cuentaId, orden.CuentaServicioId);
            Assert.Equal(pacienteId, orden.PacienteId);
            Assert.Equal("Colecistectomía Laparoscópica", orden.DescripcionCirugia);
            Assert.Equal(1500m, orden.PrecioBaseUsd);
            Assert.Equal(medicoId, orden.MedicoId);
            Assert.Equal(fecha, orden.FechaHoraProgramada);
            Assert.Equal(EstadoCirugiaConstants.PendienteEjecucion, orden.Estado);
            Assert.Equal("admin", orden.UsuarioCreacion);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenDescripcionIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new OrdenCirugia(Guid.NewGuid(), Guid.NewGuid(), "", 100m, Guid.NewGuid(), DateTime.UtcNow, "admin"));
            Assert.Contains("descripción", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPrecioIsNegative()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new OrdenCirugia(Guid.NewGuid(), Guid.NewGuid(), "Cirugia X", -50m, Guid.NewGuid(), DateTime.UtcNow, "admin"));
            Assert.Contains("precio", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMedicoIdIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new OrdenCirugia(Guid.NewGuid(), Guid.NewGuid(), "Cirugia X", 100m, Guid.Empty, DateTime.UtcNow, "admin"));
            Assert.Contains("médico", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IniciarCirugia_ShouldTransitionToEnProceso()
        {
            // Arrange
            var orden = CrearOrdenValida();

            // Act
            orden.IniciarCirugia("enfermera01");

            // Assert
            Assert.Equal(EstadoCirugiaConstants.EnProceso, orden.Estado);
            Assert.Single(orden.Logs);
            Assert.Equal(CirugiaEventoConstants.TransicionEstado, orden.Logs.First().Evento);
        }

        [Fact]
        public void IniciarCirugia_ShouldThrow_WhenNotPendiente()
        {
            var orden = CrearOrdenValida();
            orden.IniciarCirugia("user1");

            var ex = Assert.Throws<InvalidOperationException>(() => orden.IniciarCirugia("user2"));
            Assert.Contains("Pendiente de Ejecución", ex.Message);
        }

        [Fact]
        public void CompletarCirugia_ShouldTransitionToCompletada()
        {
            var orden = CrearOrdenValida();
            orden.IniciarCirugia("user1");

            orden.CompletarCirugia("user2");

            Assert.Equal(EstadoCirugiaConstants.Completada, orden.Estado);
            Assert.Equal(2, orden.Logs.Count);
        }

        [Fact]
        public void CompletarCirugia_ShouldThrow_WhenNotEnProceso()
        {
            var orden = CrearOrdenValida();

            var ex = Assert.Throws<InvalidOperationException>(() => orden.CompletarCirugia("user1"));
            Assert.Contains("En Proceso", ex.Message);
        }

        [Fact]
        public void CancelarCirugia_ShouldTransitionToCancelada_WithMotivo()
        {
            var orden = CrearOrdenValida();

            orden.CancelarCirugia("admin", "Paciente rechazó procedimiento");

            Assert.Equal(EstadoCirugiaConstants.Cancelada, orden.Estado);
            Assert.Equal("Paciente rechazó procedimiento", orden.MotivoCancelacion);
            Assert.Single(orden.Logs);
        }

        [Fact]
        public void CancelarCirugia_ShouldThrow_WhenCompletada()
        {
            var orden = CrearOrdenValida();
            orden.IniciarCirugia("user1");
            orden.CompletarCirugia("user2");

            var ex = Assert.Throws<InvalidOperationException>(() =>
                orden.CancelarCirugia("admin", "Motivo"));
            Assert.Contains("completada", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CancelarCirugia_ShouldThrow_WhenMotivoIsEmpty()
        {
            var orden = CrearOrdenValida();

            var ex = Assert.Throws<ArgumentException>(() =>
                orden.CancelarCirugia("admin", ""));
            Assert.Contains("motivo", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void AgregarLog_ShouldCreateImmutableAuditEntry()
        {
            var orden = CrearOrdenValida();

            var log = orden.AgregarLog("user1", CirugiaEventoConstants.DespachoInsumos, "5 unidades de gasa despachadas");

            Assert.Single(orden.Logs);
            Assert.Equal(orden.Id, log.OrdenCirugiaId);
            Assert.Equal("user1", log.UsuarioId);
            Assert.Equal(CirugiaEventoConstants.DespachoInsumos, log.Evento);
            Assert.Equal("5 unidades de gasa despachadas", log.Detalle);
        }

        #endregion

        #region CirugiaLog Domain Tests

        [Fact]
        public void CirugiaLog_ShouldThrow_WhenOrdenCirugiaIdIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new CirugiaLog(Guid.Empty, "user1", "Creacion", "detalle"));
            Assert.Contains("orden de cirugía", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CirugiaLog_ShouldThrow_WhenUsuarioIdIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new CirugiaLog(Guid.NewGuid(), "", "Creacion", "detalle"));
            Assert.Contains("usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CirugiaLog_ShouldThrow_WhenEventoIsEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new CirugiaLog(Guid.NewGuid(), "user1", "", "detalle"));
            Assert.Contains("evento", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Command Handler Tests

        [Fact]
        public async Task HandleAnexarCargoExtra_ShouldDeductStockAndRecordMovementAndAsignacion()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AnexarCargoExtraCirugiaCommandHandler>>();

            var orden = CrearOrdenValida();
            var cuenta = new CuentaServicios(orden.PacienteId, "admin", "Hospitalizacion");
            typeof(CuentaServicios).GetProperty("Id")?.SetValue(cuenta, orden.CuentaServicioId);

            var insumo = new Insumo("INS-EXTRA", "Sutura 3-0 Extra", 0m, SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida.UNIDAD, 25m, true);
            var stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Principal, 10m);

            var insumosList = new List<Insumo> { insumo };
            var ordenesList = new List<OrdenCirugia> { orden };
            var cuentasList = new List<CuentaServicios> { cuenta };
            var stocksList = new List<StockSede> { stockSede };
            var movimientosList = new List<MovimientoInsumo>();
            var asignacionesList = new List<InsumoCirugiaPaciente>();

            mockContext.Setup(c => c.OrdenesCirugia).Returns(ordenesList.BuildMockDbSet().Object);
            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.BuildMockDbSet().Object);
            mockContext.Setup(c => c.Insumos).Returns(insumosList.BuildMockDbSet().Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksList.BuildMockDbSet().Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosList.BuildMockDbSet().Object);
            mockContext.Setup(c => c.InsumosCirugiasPacientes).Returns(asignacionesList.BuildMockDbSet().Object);

            var command = new AnexarCargoExtraCirugiaCommand
            {
                OrdenCirugiaId = orden.Id,
                CuentaServicioId = cuenta.Id,
                ServicioId = insumo.Id,
                Descripcion = "Sutura 3-0 Extra",
                Precio = 25m,
                Honorario = 0m,
                Cantidad = 2m,
                TipoServicio = "Insumo",
                UsuarioId = "cirujano1"
            };

            var handler = new AnexarCargoExtraCirugiaCommandHandler(mockContext.Object, mockLogger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            // Stock deducted from 10 to 8 (10 - 2)
            Assert.Equal(8m, stockSede.StockActual);
            // Service added to billing account
            Assert.Single(cuenta.Detalles);
            Assert.Equal("Sutura 3-0 Extra", cuenta.Detalles.First().Descripcion);
            Assert.Equal(2m, cuenta.Detalles.First().Cantidad);
            // Audit log added to surgery order
            Assert.Single(orden.Logs);
            Assert.Equal(CirugiaEventoConstants.CargoExtra, orden.Logs.First().Evento);

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Helpers

        private static OrdenCirugia CrearOrdenValida()
        {
            return new OrdenCirugia(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Colecistectomía Laparoscópica",
                1500m,
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1),
                "admin");
        }

        #endregion
    }
}
