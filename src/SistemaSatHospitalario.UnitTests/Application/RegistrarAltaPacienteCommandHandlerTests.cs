using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class RegistrarAltaPacienteCommandHandlerTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task Handle_SaldoPendienteSinConfirmacion_LanzaExcepcionDominio()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var paciente = new PacienteAdmision("V-11223344", "Paciente Test Deuda", "0412-0000000", null, DateTime.Today.AddYears(-30));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Hospitalizacion");
            cuenta.AgregarServicio(Guid.NewGuid(), "Estancia Hospitalaria", 500m, 0m, 1, "Hospitalario", "enfermera1");

            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarAltaPacienteCommandHandler(context);
            var command = new RegistrarAltaPacienteCommand
            {
                PacienteId = paciente.Id,
                AdmisionId = cuenta.Id,
                TipoAlta = TipoAltaEnum.Normal,
                ConfirmadoPorEnfermeriaSinSolvencia = false,
                UsuarioAlta = "enfermera1"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("saldo pendiente", ex.Message.ToLower());
        }

        [Fact]
        public async Task Handle_SaldoPendienteConConfirmacion_ProcesaAltaYRegistraAuditLog()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var sede = new Sede("S01", "Sede Central", true);
            var cama = new AreaClinica(sede.Id, "HOS-01", "Cama Hosp 1", true);
            cama.MarcarComoOcupada();

            var paciente = new PacienteAdmision("V-55667788", "Paciente Test Confirmado", "0414-0000000", null, DateTime.Today.AddYears(-40));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Hospitalizacion", areaClinicaId: cama.Id);
            cuenta.AgregarServicio(Guid.NewGuid(), "Consulta UCI", 350m, 0m, 1, "Hospitalario", "enfermera1");

            await context.Sedes.AddAsync(sede);
            await context.AreasClinicas.AddAsync(cama);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarAltaPacienteCommandHandler(context);
            var command = new RegistrarAltaPacienteCommand
            {
                PacienteId = paciente.Id,
                AdmisionId = cuenta.Id,
                TipoAlta = TipoAltaEnum.Voluntaria,
                ConfirmadoPorEnfermeriaSinSolvencia = true,
                Observaciones = "Alta voluntaria firmada por familiar",
                UsuarioAlta = "enfermera1",
                IpAddress = "192.168.1.50"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(350m, result.SaldoPendienteUsd);
            Assert.Equal("Voluntaria", result.TipoAltaDesc);

            // Cama debe quedar disponible
            var camaDb = await context.AreasClinicas.FirstAsync(a => a.Id == cama.Id);
            Assert.Equal(EstadoUbicacion.Disponible, camaDb.Estado);

            // Se debe haber creado una entrada en AuditLogs
            var auditLog = await context.AuditLogs.FirstOrDefaultAsync(a => a.ActionType == "ALTA_MEDICA");
            Assert.NotNull(auditLog);
            Assert.Equal("enfermera1", auditLog.UserId);
            Assert.Equal("192.168.1.50", auditLog.IpAddress);
            Assert.Contains("Voluntaria", auditLog.NewValue);
        }

        [Fact]
        public async Task Handle_SaldoZero_ProcesaAltaCorrectamente()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var paciente = new PacienteAdmision("V-99887766", "Paciente Solvente", "0416-0000000", null, DateTime.Today.AddYears(-20));
            var cuenta = new CuentaServicios(paciente.Id, "enfermera1", "Emergencia");
            
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuenta);
            await context.SaveChangesAsync();

            var handler = new RegistrarAltaPacienteCommandHandler(context);
            var command = new RegistrarAltaPacienteCommand
            {
                PacienteId = paciente.Id,
                AdmisionId = cuenta.Id,
                TipoAlta = TipoAltaEnum.Normal,
                ConfirmadoPorEnfermeriaSinSolvencia = false,
                UsuarioAlta = "enfermera1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Exitoso);
            Assert.Equal(0m, result.SaldoPendienteUsd);
        }
    }
}
