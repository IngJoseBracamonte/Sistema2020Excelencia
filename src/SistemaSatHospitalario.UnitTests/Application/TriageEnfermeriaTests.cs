using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class TriageEnfermeriaTests
    {
        [Fact]
        public async Task Handle_DeberiaRegistrarTriageYValoracionCorrectamente()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var cuentaId = Guid.NewGuid();
            var cuenta = new CuentaServicios(Guid.NewGuid(), "Emergencia");
            typeof(CuentaServicios).GetProperty("Id")!.SetValue(cuenta, cuentaId);

            var cuentasList = new List<CuentaServicios> { cuenta }.BuildMockDbSet();
            var triagesList = new List<TriageEnfermeria>().BuildMockDbSet();
            var valoracionesList = new List<ValoracionFisica>().BuildMockDbSet();

            mockContext.Setup(c => c.CuentasServicios).Returns(cuentasList.Object);
            mockContext.Setup(c => c.TriagesEnfermeria).Returns(triagesList.Object);
            mockContext.Setup(c => c.ValoracionesFisicas).Returns(valoracionesList.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new RegistrarTriageYValoracionCommandHandler(mockContext.Object);

            var command = new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuentaId,
                UsuarioRegistro = Guid.NewGuid().ToString(),
                RegistrarConstantesVitales = true,
                MotivoConsulta = "Dolor abdominal agudo",
                TensionArterial = "120/80",
                FrecuenciaCardiaca = 75,
                FrecuenciaRespiratoria = 18,
                Temperatura = 36.5m,
                SaturacionO2 = 98,
                RegistrarValoracionFisica = true,
                EstadoConciencia = "Alerta",
                ViaAerea = "Permeable",
                Ventilacion = "Normal",
                Pulso = "Ritmico",
                PielMucosas = "Normocoloreada",
                LlenadoCapilar = "< 2 segundos",
                Pupilas = "Isocoricas"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cuentaId, result.CuentaServicioId);
            Assert.Equal("Dolor abdominal agudo", result.MotivoConsulta);
            Assert.Equal("120/80", result.TensionArterial);
            mockContext.Verify(c => c.TriagesEnfermeria.AddAsync(It.IsAny<TriageEnfermeria>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(c => c.ValoracionesFisicas.AddAsync(It.IsAny<ValoracionFisica>(), It.IsAny<CancellationToken>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
