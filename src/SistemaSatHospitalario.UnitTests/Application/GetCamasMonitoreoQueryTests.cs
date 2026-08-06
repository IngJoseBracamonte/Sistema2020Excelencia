using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;
using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class GetCamasMonitoreoQueryTests
    {
        [Fact]
        public async Task Handle_ShouldReturnActiveCamas_AndMapSedeIdCorrectly()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var sedeEmg = new Sede("EMG", "Sede Emergencia", false);
            var sedeHos = new Sede("HOS", "Sede Hospitalizacion", false);

            var cama1 = new AreaClinica(sedeEmg.Id, "BOX1", "Box 1");
            var cama2 = new AreaClinica(sedeHos.Id, "HAB101", "Habitación 101");

            // Reflect navigation property for mock mapping
            typeof(AreaClinica).GetProperty("Sede")?.SetValue(cama1, sedeEmg);
            typeof(AreaClinica).GetProperty("Sede")?.SetValue(cama2, sedeHos);

            var listCamas = new List<AreaClinica> { cama1, cama2 };
            var mockSetCamas = listCamas.BuildMockDbSet();

            mockContext.Setup(c => c.AreasClinicas).Returns(mockSetCamas.Object);

            // Mock empty CuentaServicios
            var listCuentas = new List<CuentaServicios>();
            var mockSetCuentas = listCuentas.BuildMockDbSet();
            mockContext.Setup(c => c.CuentasServicios).Returns(mockSetCuentas.Object);

            var query = new GetCamasMonitoreoQuery();
            var handler = new GetCamasMonitoreoQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var dto1 = result.FirstOrDefault(c => c.Codigo == "BOX1");
            Assert.NotNull(dto1);
            Assert.Equal(cama1.Id, dto1.CamaId);
            Assert.Equal(sedeEmg.Id, dto1.SedeId);
            Assert.Equal("Sede Emergencia", dto1.SedeNombre);

            var dto2 = result.FirstOrDefault(c => c.Codigo == "HAB101");
            Assert.NotNull(dto2);
            Assert.Equal(cama2.Id, dto2.CamaId);
            Assert.Equal(sedeHos.Id, dto2.SedeId);
            Assert.Equal("Sede Hospitalizacion", dto2.SedeNombre);
        }

        [Fact]
        public async Task Handle_ShouldExcludeCamasInAlmacenPrincipal()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var sedePrincipal = new Sede("ALM", "Almacén Principal", true);
            var sedeEmg = new Sede("EMG", "Sede Emergencia", false);

            var camaAlmacen = new AreaClinica(sedePrincipal.Id, "FARM1", "Farmacia Almacén");
            var camaValida = new AreaClinica(sedeEmg.Id, "BOX1", "Box Emergencia");

            typeof(AreaClinica).GetProperty("Sede")?.SetValue(camaAlmacen, sedePrincipal);
            typeof(AreaClinica).GetProperty("Sede")?.SetValue(camaValida, sedeEmg);

            var listCamas = new List<AreaClinica> { camaAlmacen, camaValida };
            var mockSetCamas = listCamas.BuildMockDbSet();
            mockContext.Setup(c => c.AreasClinicas).Returns(mockSetCamas.Object);

            var listCuentas = new List<CuentaServicios>();
            var mockSetCuentas = listCuentas.BuildMockDbSet();
            mockContext.Setup(c => c.CuentasServicios).Returns(mockSetCuentas.Object);

            var query = new GetCamasMonitoreoQuery();
            var handler = new GetCamasMonitoreoQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("BOX1", result[0].Codigo);
        }

        [Fact]
        public async Task Handle_ShouldMapMedicoIdAndMedicoNombreWhenOccupied()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();

            var sedeHos = new Sede("HOS", "Sede Hospitalizacion", false);
            var cama = new AreaClinica(sedeHos.Id, "HAB102", "Habitación 102");
            typeof(AreaClinica).GetProperty("Sede")?.SetValue(cama, sedeHos);

            var medico = new Medico("Dr. Carlos Pérez", Guid.NewGuid(), 50m);

            var paciente = new PacienteAdmision("V12345678", "Juan Pérez", "04120000000");
            var cuenta = new CuentaServicios(paciente.Id, "Hospitalizacion", "Ingreso regular");
            cuenta.AsignarAreaClinica(cama.Id, "Piso 1");
            cuenta.AsignarMedico(medico.Id);
            typeof(CuentaServicios).GetProperty("Paciente")?.SetValue(cuenta, paciente);
            typeof(CuentaServicios).GetProperty("Medico")?.SetValue(cuenta, medico);

            var listCamas = new List<AreaClinica> { cama };
            var mockSetCamas = listCamas.BuildMockDbSet();
            mockContext.Setup(c => c.AreasClinicas).Returns(mockSetCamas.Object);

            var listCuentas = new List<CuentaServicios> { cuenta };
            var mockSetCuentas = listCuentas.BuildMockDbSet();
            mockContext.Setup(c => c.CuentasServicios).Returns(mockSetCuentas.Object);

            var query = new GetCamasMonitoreoQuery();
            var handler = new GetCamasMonitoreoQueryHandler(mockContext.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(medico.Id, result[0].MedicoId);
            Assert.Equal("Dr. Carlos Pérez", result[0].MedicoNombre);
        }
    }
}
