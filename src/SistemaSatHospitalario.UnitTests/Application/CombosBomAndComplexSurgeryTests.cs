using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class CombosBomAndComplexSurgeryTests
    {
        [Fact]
        public async Task LoadComplexSurgery_ShouldRegisterIndependentHonorariumsAndDeductRecipeStock()
        {
            // Arrange
            var mockContext = new Mock<IApplicationDbContext>();
            var mockBillingRepo = new Mock<IBillingRepository>();
            var mockExternaService = new Mock<IOrdenExternaService>();
            var mockMapper = new Mock<IHonorariumMapperService>();
            var mockLogger = new Mock<ILogger<CargarServicioACuentaCommandHandler>>();
            var mockInvLogger = new Mock<ILogger<InventoryService>>();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, null, "Direccion");
            var doctorPrincipal = new Medico("Dr. Perez", Guid.NewGuid(), 100);
            var doctorAnestesiólogo = new Medico("Dra. Gomez", Guid.NewGuid(), 50);

            var servicioCirugia = new ServicioClinico("CIR-01", "Cirugía de Vesícula", 1500, "Procedimiento");
            servicioCirugia.RequiereInventario = true;

            var sedePrincipal = new Sede("S01", "Sede Principal", true);
            var insumo1 = new Insumo("INS-01", "Insumo 1", 100, UnidadMedida.UNIDAD, 1.50m);
            var stockSede = new StockSede(insumo1.Id, sedePrincipal.Id, 50); // Sede Principal con stock inicial de 50
            insumo1.StocksPorSede.Add(stockSede);

            var recetaInsumo = new ServicioInsumoReceta(servicioCirugia.Id, servicioCirugia.Codigo, insumo1.Id, 10, UnidadMedida.UNIDAD);
            recetaInsumo.Insumo = insumo1; // EF Navigation load

            var cuenta = new CuentaServicios(paciente.Id, "Cajero", "Particular");

            var pacientesList = new List<PacienteAdmision> { paciente };
            var pacientesMock = pacientesList.BuildMockDbSet<PacienteAdmision>();

            var serviciosList = new List<ServicioClinico> { servicioCirugia };
            var serviciosMock = serviciosList.BuildMockDbSet<ServicioClinico>();

            var medicosList = new List<Medico> { doctorPrincipal, doctorAnestesiólogo };
            var medicosMock = medicosList.BuildMockDbSet<Medico>();

            var recetasList = new List<ServicioInsumoReceta> { recetaInsumo };
            var recetasMock = recetasList.BuildMockDbSet<ServicioInsumoReceta>();

            var stocksList = new List<StockSede> { stockSede };
            var stocksMock = stocksList.BuildMockDbSet<StockSede>();

            var sedesList = new List<Sede> { sedePrincipal };
            var sedesMock = sedesList.BuildMockDbSet<Sede>();

            var logAsignaciones = new List<LogAsignacionHonorario>();
            var logsMock = logAsignaciones.BuildMockDbSet<LogAsignacionHonorario>();

            var detallesCuentasList = new List<DetalleServicioCuenta>();
            var detallesCuentasMock = detallesCuentasList.BuildMockDbSet<DetalleServicioCuenta>();

            var consumosList = new List<ConsumoServicioRealizado>();
            var consumosMock = consumosList.BuildMockDbSet<ConsumoServicioRealizado>();

            var movimientosList = new List<MovimientoInsumo>();
            var movimientosMock = movimientosList.BuildMockDbSet<MovimientoInsumo>();

            mockContext.Setup(c => c.PacientesAdmision).Returns(pacientesMock.Object);
            mockContext.Setup(c => c.ServiciosClinicos).Returns(serviciosMock.Object);
            mockContext.Setup(c => c.Medicos).Returns(medicosMock.Object);
            mockContext.Setup(c => c.ServiciosInsumoRecetas).Returns(recetasMock.Object);
            mockContext.Setup(c => c.StocksSedes).Returns(stocksMock.Object);
            mockContext.Setup(c => c.Sedes).Returns(sedesMock.Object);
            mockContext.Setup(c => c.LogsAsignacionHonorario).Returns(logsMock.Object);
            mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesCuentasMock.Object);
            mockContext.Setup(c => c.ConsumosServiciosRealizados).Returns(consumosMock.Object);
            mockContext.Setup(c => c.MovimientosInsumo).Returns(movimientosMock.Object);

            mockBillingRepo.Setup(r => r.ObtenerCuentaAbiertaPorPacienteAsync(paciente.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cuenta);

            var inventoryService = new InventoryService(mockContext.Object, mockInvLogger.Object);
            var handler = new CargarServicioACuentaCommandHandler(
                mockBillingRepo.Object, mockExternaService.Object, mockContext.Object, mockMapper.Object, inventoryService, mockLogger.Object);

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                ServicioId = servicioCirugia.Id.ToString(),
                Descripcion = servicioCirugia.Descripcion,
                Precio = 1500,
                Cantidad = 1,
                TipoServicio = "Procedimiento",
                UsuarioCarga = "Cajero",
                MedicosRoles = new List<MedicoRolInputDto>
                {
                    new MedicoRolInputDto { MedicoId = doctorPrincipal.Id, Rol = "Cirujano Principal", MontoHonorario = 500 },
                    new MedicoRolInputDto { MedicoId = doctorAnestesiólogo.Id, Rol = "Anestesiólogo", MontoHonorario = 200 }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var detalleCargado = cuenta.Detalles.FirstOrDefault();
            Assert.NotNull(detalleCargado);

            // Verificar honorarios acumulados en el detalle de la cuenta
            Assert.Equal(700, detalleCargado.Honorario); // 500 + 200

            // Verificar múltiples médicos cargados en el detalle del servicio
            Assert.Equal(2, detalleCargado.MedicosResponsables.Count);
            Assert.Contains(detalleCargado.MedicosResponsables, m => m.MedicoId == doctorPrincipal.Id && m.Rol == "Cirujano Principal" && m.MontoHonorario == 500);
            Assert.Contains(detalleCargado.MedicosResponsables, m => m.MedicoId == doctorAnestesiólogo.Id && m.Rol == "Anestesiólogo" && m.MontoHonorario == 200);

            // Verificar descuento total de insumos en el stock de la sede
            Assert.Equal(40, stockSede.StockActual); // 50 inicial - 10 consumidos por receta
        }
    }
}
