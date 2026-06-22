using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Repositories;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CargarServiciosEspecialesTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly BillingRepository _repository;
        private readonly Mock<IOrdenExternaService> _externaServiceMock;
        private readonly Mock<IHonorariumMapperService> _mapperServiceMock;
        private readonly CargarServicioACuentaCommandHandler _handler;

        public CargarServiciosEspecialesTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _repository = new BillingRepository(_context);
            _externaServiceMock = new Mock<IOrdenExternaService>();
            _mapperServiceMock = new Mock<IHonorariumMapperService>();

            // Setup mapper service fallback
            _mapperServiceMock.Setup(m => m.MapToCategoryAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync((string type, Guid? id) => type);

            _handler = new CargarServicioACuentaCommandHandler(
                _repository,
                _externaServiceMock.Object,
                _context,
                _mapperServiceMock.Object,
                NullLogger<CargarServicioACuentaCommandHandler>.Instance
            );
        }

        private async Task<PacienteAdmision> SeedPacienteAsync()
        {
            var paciente = new PacienteAdmision("V-88888888", "Paciente Test Cargador", "555-1111");
            await _context.PacientesAdmision.AddAsync(paciente);
            await _context.SaveChangesAsync();
            return paciente;
        }

        [Fact]
        public async Task Should_ChargeConsulta_CreatingCita_And_CalculatingPriceWithDoctorHonorarium()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();
            
            // Especialidad
            var especialidad = new Especialidad("Pediatría");
            await _context.Especialidades.AddAsync(especialidad);

            // Medico con HonorarioBase = 25.50
            var medico = new Medico("Dr. Juan Pediatra", especialidad.Id, 25.50m);
            await _context.Medicos.AddAsync(medico);

            // Servicio clínico de tipo Medico con PrecioBase = 30.00 e HonorarioBase = 15.00
            var servicio = new ServicioClinico("CONS-PEDI", "Consulta Pediátrica", 30.00m, "Medico")
            {
                Category = ServiceCategory.Consultation,
                HonorariumCategory = "Medico",
                HonorarioBase = 15.00m
            };
            await _context.ServiciosClinicos.AddAsync(servicio);
            await _context.SaveChangesAsync();

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = servicio.Id.ToString(),
                Descripcion = servicio.Descripcion,
                Precio = 30.00m,       // base price
                Honorario = 0m,        // 0 indicates default doctor honorarium resolution
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "NurseAdmin",
                MedicoId = medico.Id,
                HoraCita = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CuentaId.Should().NotBeEmpty();
            result.DetalleId.Should().NotBeEmpty();

            // Verify Cuenta details
            var dbCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaId);
            
            dbCuenta.Should().NotBeNull();
            dbCuenta.Detalles.Should().ContainSingle();
            
            var detail = dbCuenta.Detalles.First();
            detail.TipoServicio.Should().Be("Medico");
            // expectedPrecio = baseService.PrecioBase + doctorHonorary (30.00 + 25.50 = 55.50)
            detail.Precio.Should().Be(55.50m); 
            detail.Honorario.Should().Be(25.50m); // assigned to pediatrician's honorarium

            // Verify CitaMedica was created
            var cita = await _context.CitasMedicas.FirstOrDefaultAsync(c => c.CuentaServicioId == result.CuentaId);
            cita.Should().NotBeNull();
            cita.MedicoId.Should().Be(medico.Id);
            cita.PacienteId.Should().Be(paciente.Id);
            cita.Estado.Should().Be(EstadoConstants.Pendiente);

            // Verify External order notified
            _externaServiceMock.Verify(e => e.EnviarOrdenLegacyAsync(30.00m, 0, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ThrowException_When_CitaCollisionOccurs()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();
            var especialidad = new Especialidad("Ginecología");
            await _context.Especialidades.AddAsync(especialidad);

            var medico = new Medico("Dra. María Ginecologa", especialidad.Id, 30.00m);
            await _context.Medicos.AddAsync(medico);

            var servicio = new ServicioClinico("CONS-GIN", "Consulta Ginecológica", 40.00m, "Medico")
            {
                Category = ServiceCategory.Consultation,
                HonorariumCategory = "Medico"
            };
            await _context.ServiciosClinicos.AddAsync(servicio);
            await _context.SaveChangesAsync();

            var horaPautada = new DateTime(2026, 6, 22, 10, 0, 0, DateTimeKind.Unspecified);

            // Pre-seed an existing active citation at the exact same hour
            var existingCita = new CitaMedica(medico.Id, paciente.Id, Guid.NewGuid(), horaPautada);
            await _context.CitasMedicas.AddAsync(existingCita);
            await _context.SaveChangesAsync();

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = servicio.Id.ToString(),
                Descripcion = servicio.Descripcion,
                Precio = 40.00m,
                Honorario = 30.00m,
                Cantidad = 1,
                TipoServicio = "Medico",
                UsuarioCarga = "NurseAdmin",
                MedicoId = medico.Id,
                HoraCita = horaPautada
            };

            // Act & Assert
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*médico ya tiene una cita pautada*");
        }

        [Fact]
        public async Task Should_ChargeRX_CorrectlySettingDetailsAndLegacyId()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();
            
            var servicio = new ServicioClinico("RX-01", "Radiografía de Tórax", 50.00m, "RX")
            {
                Category = ServiceCategory.Radiology,
                LegacyMappingId = "RX-LEG-100"
            };
            await _context.ServiciosClinicos.AddAsync(servicio);
            await _context.SaveChangesAsync();

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = servicio.Id.ToString(),
                Descripcion = servicio.Descripcion,
                Precio = 50.00m,
                Honorario = 0m,
                Cantidad = 1,
                TipoServicio = "RX",
                UsuarioCarga = "NurseAdmin"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            
            var dbCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaId);

            dbCuenta.Should().NotBeNull();
            dbCuenta.Detalles.Should().ContainSingle();
            
            var detail = dbCuenta.Detalles.First();
            detail.TipoServicio.Should().Be("RX");
            detail.LegacyMappingId.Should().Be("RX-LEG-100");
            detail.Precio.Should().Be(50.00m);

            // External service check
            _externaServiceMock.Verify(e => e.EnviarOrdenLegacyAsync(50.00m, 0, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ChargeTomografia_CorrectlyNotifyingExternalSystem()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();

            var servicio = new ServicioClinico("TOMO-01", "Tomografía Computarizada Cerebral", 120.00m, "RX")
            {
                Category = ServiceCategory.Tomography,
                LegacyMappingId = "TOMO-LEG-200"
            };
            await _context.ServiciosClinicos.AddAsync(servicio);
            await _context.SaveChangesAsync();

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = servicio.Id.ToString(),
                Descripcion = servicio.Descripcion,
                Precio = 120.00m,
                Honorario = 0m,
                Cantidad = 1,
                TipoServicio = "RX",
                UsuarioCarga = "NurseAdmin"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            var dbCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaId);

            dbCuenta.Should().NotBeNull();
            dbCuenta.Detalles.Should().ContainSingle();

            var detail = dbCuenta.Detalles.First();
            detail.LegacyMappingId.Should().Be("TOMO-LEG-200");
            detail.Precio.Should().Be(120.00m);

            _externaServiceMock.Verify(e => e.EnviarOrdenLegacyAsync(120.00m, 0, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ChargePerfilLaboratorio_ResolvingNumericLegacyIdDirectly()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();

            // Profile legacy ID is "1403". It's not a GUID!
            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = "1403", // Numeric ID representing profile
                Descripcion = "Perfil de Rutina Adulto",
                Precio = 85.00m,
                Honorario = 0m,
                Cantidad = 1,
                TipoServicio = "Laboratorio",
                UsuarioCarga = "NurseAdmin"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            var dbCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaId);

            dbCuenta.Should().NotBeNull();
            dbCuenta.Detalles.Should().ContainSingle();

            var detail = dbCuenta.Detalles.First();
            detail.TipoServicio.Should().Be("Laboratorio");
            detail.LegacyMappingId.Should().Be("1403"); // Handled numeric legacy ID directly
            detail.Precio.Should().Be(85.00m);

            _externaServiceMock.Verify(e => e.EnviarOrdenLegacyAsync(85.00m, 0, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_ChargeMedicamento_WithoutCreatingCitaOrLegacyMappingConstraint()
        {
            // Arrange
            var paciente = await SeedPacienteAsync();

            var servicio = new ServicioClinico("MED-IBU", "Ibuprofeno 400mg", 5.00m, "Insumo")
            {
                Category = ServiceCategory.Insumo
            };
            await _context.ServiciosClinicos.AddAsync(servicio);
            await _context.SaveChangesAsync();

            var command = new CargarServicioACuentaCommand
            {
                PacienteId = paciente.Id,
                TipoIngreso = "Particular",
                ServicioId = servicio.Id.ToString(),
                Descripcion = servicio.Descripcion,
                Precio = 5.00m,
                Honorario = 0m,
                Cantidad = 3,
                TipoServicio = "Insumo",
                UsuarioCarga = "NurseAdmin"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();

            var dbCuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaId);

            dbCuenta.Should().NotBeNull();
            dbCuenta.Detalles.Should().ContainSingle();

            var detail = dbCuenta.Detalles.First();
            detail.TipoServicio.Should().Be("Insumo");
            detail.Precio.Should().Be(5.00m);
            detail.Cantidad.Should().Be(3);
            detail.LegacyMappingId.Should().BeNull(); // No mapping set

            // Verify no citas created
            var citas = await _context.CitasMedicas.Where(c => c.CuentaServicioId == result.CuentaId).ToListAsync();
            citas.Should().BeEmpty();

            _externaServiceMock.Verify(e => e.EnviarOrdenLegacyAsync(15.00m, 0, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
