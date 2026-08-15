using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class DoctorHonorariumSummaryQueryTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;

        public DoctorHonorariumSummaryQueryTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
        }

        [Fact]
        public async Task Handle_Should_CorrectlySeparateConsultationsAndTechnicalServices_WithoutDoubleCounting()
        {
            // Arrange
            var especialidadId = Guid.NewGuid();
            var medico = new Medico("Dr. Gregory House", especialidadId, 50.0m, "04141234567");

            var pacienteId = Guid.NewGuid();
            var cuenta = new CuentaServicios(pacienteId, "admin", EstadoConstants.Abierta, null);

            var date = DateTime.UtcNow;

            // 1. Cita médica atendida (Consulta)
            var cita = new CitaMedica(medico.Id, pacienteId, cuenta.Id, date, "Consulta General");
            cita.MarcarComoAtendida();

            var detalleConsulta = new DetalleServicioCuenta(
                cuenta.Id,
                Guid.NewGuid(),
                "Consulta Médica General",
                50.0m,
                50.0m,
                1,
                "MEDICO",
                "admin",
                null,
                null,
                null,
                TipoServicioConstants.Medico
            );
            detalleConsulta.AsignarMedicoResponsable(medico.Id, HonorarioConstants.CategoriaConsulta, 50.0m);

            // 2. Servicio técnico realizado (RX de Tórax)
            var detalleRx = new DetalleServicioCuenta(
                cuenta.Id,
                Guid.NewGuid(),
                "RX Tórax PA",
                30.0m,
                10.0m, // 10 USD honorario informe
                1,
                "RX",
                "admin",
                null,
                null,
                null,
                TipoServicioConstants.RX
            );
            detalleRx.AsignarMedicoResponsable(medico.Id, "RX", 10.0m);
            detalleRx.MarcarRealizado("tecnico1");

            // 3. Mock datasets
            var medicosMock = new List<Medico> { medico }.BuildMockDbSet();
            var cuentasMock = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var citasMock = new List<CitaMedica> { cita }.BuildMockDbSet();
            var detallesMock = new List<DetalleServicioCuenta> { detalleConsulta, detalleRx }.BuildMockDbSet();

            _mockContext.Setup(c => c.Medicos).Returns(medicosMock.Object);
            _mockContext.Setup(c => c.CuentasServicios).Returns(cuentasMock.Object);
            _mockContext.Setup(c => c.CitasMedicas).Returns(citasMock.Object);
            _mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesMock.Object);

            var handler = new GetDoctorHonorariumSummaryQueryHandler(_mockContext.Object);
            var query = new GetDoctorHonorariumSummaryQuery
            {
                StartDate = date.Date,
                EndDate = date.Date
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var doctorSummary = result[0];
            Assert.Equal(medico.Id, doctorSummary.MedicoId);
            Assert.Equal("Dr. Gregory House", doctorSummary.MedicoNombre);
            Assert.Equal(2, doctorSummary.CantidadServicios);
            Assert.Equal(60.0m, doctorSummary.TotalHonorarios); // 50 (Consulta) + 10 (RX)

            // Desglose debe tener exactamente Consulta (50) y RX (10)
            var consultaDesglose = doctorSummary.Desglose.FirstOrDefault(d => d.Categoria == HonorarioConstants.CategoriaConsulta);
            Assert.NotNull(consultaDesglose);
            Assert.Equal(1, consultaDesglose.Cantidad);
            Assert.Equal(50.0m, consultaDesglose.Total);

            var rxDesglose = doctorSummary.Desglose.FirstOrDefault(d => d.Categoria == "RX");
            Assert.NotNull(rxDesglose);
            Assert.Equal(1, rxDesglose.Cantidad);
            Assert.Equal(10.0m, rxDesglose.Total);
        }
    }
}
