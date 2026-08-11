using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Strategies;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class ImagingReportLoadingStrategyTests
    {
        private readonly DbContextOptions<SatHospitalarioDbContext> _options;

        public ImagingReportLoadingStrategyTests()
        {
            _options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: "ImagingReportStrategy_" + Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ImagingStrategy_WithToggleOff_ShouldCreateSingleDetailWithoutDoctorFee()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var mockExterna = new Mock<IOrdenExternaService>();
            var mockMediator = new Mock<IMediator>();

            var strategy = new ImagingLoadingStrategy(mockExterna.Object, context, mockMediator.Object);

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "04121234567", null, DateTime.Now.AddYears(-30), "Dir");
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");

            var baseService = new ServicioClinico("RX01", "RX Torax PA", 20.00m, "RX")
            {
                Category = ServiceCategory.Radiology,
                HonorarioBase = 5.00m
            };

            await context.PacientesAdmision.AddAsync(paciente);
            await context.ServiciosClinicos.AddAsync(baseService);
            await context.SaveChangesAsync();

            var request = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = baseService.Id.ToString(),
                Descripcion = baseService.Descripcion,
                Precio = 20.00m,
                Cantidad = 1,
                TipoServicio = "RX",
                RequiereInforme = false
            };

            var detalle = cuenta.AgregarServicio(baseService.Id, baseService.Descripcion, 20.00m, 5.00m, 1, "RX", "Admin", null, null);

            // Act
            await strategy.ExecuteAsync(request, cuenta, paciente, detalle, baseService, CancellationToken.None);

            // Assert
            Assert.Null(detalle.MedicoResponsableId);
            Assert.Equal(0m, detalle.Honorario);
            Assert.Single(cuenta.Detalles);
        }

        [Fact]
        public async Task ImagingStrategy_WithToggleOn_ShouldCreateTwoDetailsAndAssignParentChildRelationship()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var mockExterna = new Mock<IOrdenExternaService>();
            var mockMediator = new Mock<IMediator>();

            var strategy = new ImagingLoadingStrategy(mockExterna.Object, context, mockMediator.Object);

            var paciente = new PacienteAdmision("V-87654321", "Maria Gomez", "04141234567", null, DateTime.Now.AddYears(-25), "Dir");
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");

            var reportService = new ServicioClinico("INF01", "Informe RX Torax", 10.00m, "INFORME")
            {
                TipoServicioId = TipoServicioConstants.Informe,
                EsServicioInforme = true,
                HonorarioBase = 7.00m
            };

            var baseService = new ServicioClinico("RX01", "RX Torax PA", 20.00m, "RX")
            {
                Category = ServiceCategory.Radiology,
                ServicioInformeId = reportService.Id,
                ServicioInforme = reportService
            };

            var medicoRadiologo = new Medico("Dr. Radiologo", Guid.NewGuid(), 10m);

            await context.PacientesAdmision.AddAsync(paciente);
            await context.ServiciosClinicos.AddRangeAsync(baseService, reportService);
            await context.Medicos.AddAsync(medicoRadiologo);
            await context.SaveChangesAsync();

            var request = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = baseService.Id.ToString(),
                Descripcion = baseService.Descripcion,
                Precio = 20.00m,
                Cantidad = 1,
                TipoServicio = "RX",
                RequiereInforme = true,
                MedicoInterpreteId = medicoRadiologo.Id
            };

            var detalleBase = cuenta.AgregarServicio(baseService.Id, baseService.Descripcion, 20.00m, 0m, 1, "RX", "Admin", null, null);

            // Act
            await strategy.ExecuteAsync(request, cuenta, paciente, detalleBase, baseService, CancellationToken.None);

            // Assert
            Assert.Null(detalleBase.MedicoResponsableId);
            Assert.Equal(2, cuenta.Detalles.Count);

            var detalleInforme = cuenta.Detalles.FirstOrDefault(d => d.DetallePadreId == detalleBase.Id);
            Assert.NotNull(detalleInforme);
            Assert.Equal(reportService.Id, detalleInforme.ServicioId);
            Assert.Equal(medicoRadiologo.Id, detalleInforme.MedicoResponsableId);
            Assert.Equal(10.00m, detalleInforme.Precio);
            Assert.Equal(7.00m, detalleInforme.Honorario);
            Assert.Equal(30.00m, cuenta.Detalles.Sum(d => d.ObtenerSubtotal()));
        }

        [Fact]
        public async Task RemoveServicioDeCuenta_ProcessedOrder_ShouldThrowInvalidOperationException()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var mockRepo = new Mock<IBillingRepository>();

            var paciente = new PacienteAdmision("V-5555555", "Carlos Rivas", "04165555555", null, DateTime.Now.AddYears(-40), "Dir");
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");

            var baseService = new ServicioClinico("RX01", "RX Torax PA", 20.00m, "RX");
            var detalle = cuenta.AgregarServicio(baseService.Id, baseService.Descripcion, 20.00m, 0m, 1, "RX", "Admin", null, null);

            var orden = new OrdenImagen(cuenta.Id, paciente.Id, "Carlos Rivas", "RX Torax PA", "RX");
            orden.MarcarComoProcesado("AsistenteRX");
            orden.LinkInforme = "https://pacs.hospital.com/report/123.pdf";

            mockRepo.Setup(r => r.ObtenerCuentaPorIdAsync(cuenta.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(cuenta);

            await context.OrdenesImagenes.AddAsync(orden);
            await context.SaveChangesAsync();

            var handler = new RemoveServicioDeCuentaCommandHandler(mockRepo.Object, context);
            var command = new RemoveServicioDeCuentaCommand { CuentaId = cuenta.Id, DetalleId = detalle.Id };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("procesado", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task TomographyService_DomainClassification_ShouldAssertExactTomographyConstant()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);
            var mockExterna = new Mock<IOrdenExternaService>();
            var mockMediator = new Mock<IMediator>();

            var strategy = new ImagingLoadingStrategy(mockExterna.Object, context, mockMediator.Object);

            var paciente = new PacienteAdmision("V-99999999", "Pedro Gomez", "04129999999", null, DateTime.Now.AddYears(-50), "Dir");
            var cuenta = new CuentaServicios(paciente.Id, "Admin", "Particular");

            var tomoService = new ServicioClinico("TAC01", "Tomografía Axial Computarizada Cráneo", 120.00m, TipoServicioConstants.TomografiaString)
            {
                TipoServicioId = TipoServicioConstants.Tomografia,
                Category = ServiceCategory.Radiology,
                HonorariumCategory = "TOMO"
            };

            await context.PacientesAdmision.AddAsync(paciente);
            await context.ServiciosClinicos.AddAsync(tomoService);
            await context.SaveChangesAsync();

            var request = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = tomoService.Id.ToString(),
                Descripcion = tomoService.Descripcion,
                Precio = 120.00m,
                Cantidad = 1,
                TipoServicio = TipoServicioConstants.TomografiaString,
                RequiereInforme = false
            };

            var detalle = cuenta.AgregarServicio(tomoService.Id, tomoService.Descripcion, 120.00m, 25.00m, 1, TipoServicioConstants.TomografiaString, "Admin", null, null);

            // Act
            await strategy.ExecuteAsync(request, cuenta, paciente, detalle, tomoService, CancellationToken.None);

            // Assert
            Assert.Equal(TipoServicioConstants.TomografiaString, tomoService.TipoServicio);
            Assert.Equal(TipoServicioConstants.Tomografia, tomoService.TipoServicioId);
            Assert.Equal(TipoServicioConstants.TomografiaString, detalle.TipoServicio);
            mockExterna.Verify(e => e.EnviarOrdenTomoAsync(cuenta.Id, paciente.Id, tomoService.Descripcion, paciente.NombreCompleto, It.IsAny<CancellationToken>(), false, null), Times.Once);
            mockExterna.Verify(e => e.EnviarOrdenRXAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<Guid?>()), Times.Never);
        }
    }
}
