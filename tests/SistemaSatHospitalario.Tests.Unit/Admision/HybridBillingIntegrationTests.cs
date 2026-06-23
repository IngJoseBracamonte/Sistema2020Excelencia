using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Repositories;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Admision
{
    public class HybridBillingIntegrationTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SatHospitalarioDbContext> _options;
        private readonly Mock<ILegacyLabRepository> _legacyLabRepoMock;
        private readonly Mock<IHonorariumMapperService> _mapperServiceMock;
        private readonly Mock<ILogger<SyncCarritoCommandHandler>> _loggerMock;

        public HybridBillingIntegrationTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseSqlite(_connection)
                .LogTo(Console.WriteLine)
                .Options;

            using (var context = new SatHospitalarioDbContext(_options))
            {
                context.Database.EnsureCreated();
            }

            _legacyLabRepoMock = new Mock<ILegacyLabRepository>();
            _mapperServiceMock = new Mock<IHonorariumMapperService>();
            _loggerMock = new Mock<ILogger<SyncCarritoCommandHandler>>();
        }

        private (SatHospitalarioDbContext context, SyncCarritoCommandHandler handler) CreateFreshContextAndHandler()
        {
            var context = new SatHospitalarioDbContext(_options);
            var repository = new BillingRepository(context);
            var mockInventory = new Mock<IInventoryService>();
            var handler = new SyncCarritoCommandHandler(
                repository,
                context,
                _legacyLabRepoMock.Object,
                _mapperServiceMock.Object,
                mockInventory.Object,
                _loggerMock.Object
            );
            return (context, handler);
        }

        [Fact]
        public async Task SyncCarrito_PlanA_NewHospitalization_Particular_ShouldCreateOpenAccountWithoutConvenio()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var p = new PacienteAdmision("999111", "Plan A Paciente", "123456");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(p, pacienteId);
            
            var service = new ServicioClinico("H001", "Habitación por Día", 120.00m, "Otros");
            var serviceId = Guid.NewGuid();
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, serviceId);

            using (var arrangeContext = new SatHospitalarioDbContext(_options))
            {
                arrangeContext.PacientesAdmision.Add(p);
                arrangeContext.ServiciosClinicos.Add(service);
                await arrangeContext.SaveChangesAsync();
            }

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "CajeroAdmin",
                TipoIngreso = "Hospitalizacion",
                ConvenioId = null, // Particular
                IsPrivilegedUser = true, // Evitar validaciones de supervisor para el test
                Items = new List<ServicioCarritoDto>
                {
                    new() { ServicioId = serviceId.ToString(), Descripcion = "Habitación por Día", Precio = 120.00m, Cantidad = 2, TipoServicio = "Otros" }
                }
            };

            // Act
            var (actContext, actHandler) = CreateFreshContextAndHandler();
            var result = await actHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            using (var assertContext = new SatHospitalarioDbContext(_options))
            {
                var cuenta = await assertContext.CuentasServicios.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == result.CuentaId);
                cuenta.Should().NotBeNull();
                cuenta!.Estado.Should().Be(EstadoConstants.Abierta);
                cuenta.TipoIngreso.Should().Be("Hospitalizacion");
                cuenta.ConvenioId.Should().BeNull();
                cuenta.Detalles.Should().HaveCount(1);
                cuenta.Detalles.First().Cantidad.Should().Be(2);
                cuenta.Detalles.First().Precio.Should().Be(120.00m);
            }
        }

        [Fact]
        public async Task SyncCarrito_PlanA_NewHospitalization_Convenio_ShouldCreateOpenAccountWithConvenio()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var p = new PacienteAdmision("999222", "Plan A Asegurado", "123456");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(p, pacienteId);
            
            // Añadir SeguroConvenio para satisfacer la clave foránea
            var convenio = new SeguroConvenio("PDVSA", "RTN-5", "Direccion", "123", "pdvsa@test.com");
            typeof(SeguroConvenio).GetProperty("Id")?.SetValue(convenio, 5);

            var service = new ServicioClinico("H001", "Habitación por Día", 80.00m, "Otros");
            var serviceId = Guid.NewGuid();
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, serviceId);

            using (var arrangeContext = new SatHospitalarioDbContext(_options))
            {
                arrangeContext.PacientesAdmision.Add(p);
                arrangeContext.SegurosConvenios.Add(convenio);
                arrangeContext.ServiciosClinicos.Add(service);
                await arrangeContext.SaveChangesAsync();
            }

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "CajeroAdmin",
                TipoIngreso = "Hospitalizacion",
                ConvenioId = 5, // Convenio PDVSA / Seguro
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new() { ServicioId = serviceId.ToString(), Descripcion = "Habitación por Día", Precio = 80.00m, Cantidad = 1, TipoServicio = "Otros" }
                }
            };

            // Act
            var (actContext, actHandler) = CreateFreshContextAndHandler();
            var result = await actHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            using (var assertContext = new SatHospitalarioDbContext(_options))
            {
                var cuenta = await assertContext.CuentasServicios.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == result.CuentaId);
                cuenta.Should().NotBeNull();
                cuenta!.ConvenioId.Should().Be(5);
                cuenta.TipoIngreso.Should().Be("Hospitalizacion");
            }
        }

        [Fact]
        public async Task SyncCarrito_PlanB_IncrementalCharges_ShouldAppendToExistingOpenAccount()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var p = new PacienteAdmision("999333", "Plan B Paciente", "123456");
            typeof(PacienteAdmision).GetProperty("Id")?.SetValue(p, pacienteId);

            // Crear una cuenta abierta previa de hospitalización para el paciente
            var cuentaPrevia = new CuentaServicios(pacienteId, "CajeroAdmin", "Hospitalizacion", null);

            var service = new ServicioClinico("L001", "Hemograma", 25.00m, "Laboratorio");
            var serviceId = Guid.NewGuid();
            typeof(ServicioClinico).GetProperty("Id")?.SetValue(service, serviceId);

            using (var arrangeContext = new SatHospitalarioDbContext(_options))
            {
                arrangeContext.PacientesAdmision.Add(p);
                arrangeContext.CuentasServicios.Add(cuentaPrevia);
                arrangeContext.ServiciosClinicos.Add(service);
                await arrangeContext.SaveChangesAsync();
            }

            var command = new SyncCarritoCommand
            {
                PacienteId = pacienteId,
                UsuarioCarga = "AsistentePiso",
                TipoIngreso = "Hospitalizacion",
                IsPrivilegedUser = true,
                Items = new List<ServicioCarritoDto>
                {
                    new() { ServicioId = serviceId.ToString(), Descripcion = "Hemograma", Precio = 25.00m, Cantidad = 1, TipoServicio = "Laboratorio" }
                }
            };

            // Act
            var (actContext, actHandler) = CreateFreshContextAndHandler();
            var result = await actHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaId.Should().Be(cuentaPrevia.Id); // Debe reusar la misma cuenta

            using (var assertContext = new SatHospitalarioDbContext(_options))
            {
                var cuenta = await assertContext.CuentasServicios.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == cuentaPrevia.Id);
                cuenta.Should().NotBeNull();
                cuenta!.Detalles.Should().HaveCount(1);
                cuenta.Detalles.First().Descripcion.Should().Be("Hemograma");
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
