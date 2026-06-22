using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class EnfermeriaCommandHandlerAndQueryTests
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly RegistrarTriageYValoracionCommandHandler _registerHandler;
        private readonly ModificarTriageYValoracionCommandHandler _modifyHandler;
        private readonly GetTriageYValoracionHistoryQueryHandler _historyHandler;
        private readonly GetNurseAuditReportQueryHandler _auditHandler;

        public EnfermeriaCommandHandlerAndQueryTests()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SatHospitalarioDbContext(options);
            _registerHandler = new RegistrarTriageYValoracionCommandHandler(_context);
            _modifyHandler = new ModificarTriageYValoracionCommandHandler(_context);
            _historyHandler = new GetTriageYValoracionHistoryQueryHandler(_context);
            _auditHandler = new GetNurseAuditReportQueryHandler(_context);
        }

        private async Task<CuentaServicios> SeedBaseAccountAsync()
        {
            var paciente = new PacienteAdmision("V-99999999", "Test Patient Name", "555-4567");
            await _context.PacientesAdmision.AddAsync(paciente);

            var cuenta = new CuentaServicios(paciente.Id, "NurseUser", EstadoConstants.Emergencia);
            await _context.CuentasServicios.AddAsync(cuenta);
            await _context.SaveChangesAsync();

            return cuenta;
        }

        [Fact]
        public async Task Should_RegisterTriageAndValoracion_Successfully()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            var command = new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Dolor de cabeza severo",
                TensionArterial = "120/80",
                FrecuenciaCardiaca = 80,
                FrecuenciaRespiratoria = 18,
                Temperatura = 36.5m,
                SaturacionO2 = 98,
                GlicemiaCapilar = 100,
                DescripcionRapida = "Estable",
                DescripcionDetallada = "Paciente alerta y consciente",
                EstadoConciencia = "Alerta",
                GlasgowOcular = 4,
                GlasgowVerbal = 5,
                GlasgowMotor = 6,
                GlasgowTotal = 15,
                ViaAerea = "Permeable",
                Ventilacion = "Normal",
                Pulso = "Rítmico",
                PielMucosas = "Normocoloreada",
                LlenadoCapilar = "< 2 segundos",
                Pupilas = "Isocóricas",
                Alergias = "Ninguna",
                AccesosVenosos = "No",
                Pertenencias = "Familiar",
                AntecedentesMedicos = "Hipertensión",
                UsuarioRegistro = "NurseUser"
            };

            // Act
            var result = await _registerHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TriageId.Should().NotBeEmpty();
            result.ValoracionId.Should().NotBeEmpty();
            result.MotivoConsulta.Should().Be("Dolor de cabeza severo");
            result.DescripcionRapida.Should().Be("Estable");
            result.DescripcionDetallada.Should().Be("Paciente alerta y consciente");
            result.GlasgowTotal.Should().Be(15);
            result.Alergias.Should().Be("Ninguna");

            // Verify in DB
            var dbTriage = await _context.TriagesEnfermeria.FindAsync(result.TriageId);
            dbTriage.Should().NotBeNull();
            dbTriage.MotivoConsulta.Should().Be("Dolor de cabeza severo");
            dbTriage.DescripcionRapida.Should().Be("Estable");

            var dbValoracion = await _context.ValoracionesFisicas.FindAsync(result.ValoracionId);
            dbValoracion.Should().NotBeNull();
            dbValoracion.EstadoConciencia.Should().Be("Alerta");
        }

        [Fact]
        public async Task Should_ThrowException_When_CuentaDoesNotExist()
        {
            // Arrange
            var command = new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = Guid.NewGuid(),
                UsuarioRegistro = "NurseUser"
            };

            // Act & Assert
            Func<Task> act = async () => await _registerHandler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Should_FusePreviousData_When_RegistrarSectionsAreFalse()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            
            // First register completely
            var command1 = new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Primer Motivo",
                TensionArterial = "120/80",
                FrecuenciaCardiaca = 80,
                FrecuenciaRespiratoria = 18,
                Temperatura = 36.5m,
                SaturacionO2 = 98,
                DescripcionRapida = "Primer Estado",
                DescripcionDetallada = "Primer Detalle",
                EstadoConciencia = "Alerta",
                GlasgowTotal = 15,
                ViaAerea = "Permeable",
                Ventilacion = "Normal",
                Pulso = "Rítmico",
                PielMucosas = "Normocoloreada",
                LlenadoCapilar = "< 2 segundos",
                Pupilas = "Isocóricas",
                Alergias = "Penicilina",
                AntecedentesMedicos = "Asma",
                UsuarioRegistro = "Nurse1"
            };
            await _registerHandler.Handle(command1, CancellationToken.None);

            // Register again but disable all sections (updates should copy forward from last entry)
            var command2 = new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                RegistrarConstantesVitales = false,
                RegistrarValoracionFisica = false,
                RegistrarAntecedentes = false,
                RegistrarEstadoActual = false,
                UsuarioRegistro = "Nurse2"
            };

            // Act
            var result = await _registerHandler.Handle(command2, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            // Constantes Vitales should be copied from first entry
            result.MotivoConsulta.Should().Be("Primer Motivo");
            result.TensionArterial.Should().Be("120/80");
            result.FrecuenciaCardiaca.Should().Be(80);
            result.FrecuenciaRespiratoria.Should().Be(18);
            result.Temperatura.Should().Be(36.5m);
            result.SaturacionO2.Should().Be(98);

            // Estado Actual should be copied from first entry
            result.DescripcionRapida.Should().Be("Primer Estado");
            result.DescripcionDetallada.Should().Be("Primer Detalle");

            // Valoración Física should be copied
            result.EstadoConciencia.Should().Be("Alerta");
            result.GlasgowTotal.Should().Be(15);
            result.ViaAerea.Should().Be("Permeable");

            // Antecedentes should be copied
            result.Alergias.Should().Be("Penicilina");
            result.AntecedentesMedicos.Should().Be("Asma");
        }

        [Fact]
        public async Task Should_ModifyTriageAndValoracion_Successfully()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            var regResult = await _registerHandler.Handle(new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Consulta Original",
                TensionArterial = "120/80",
                EstadoConciencia = "Alerta",
                UsuarioRegistro = "Nurse"
            }, CancellationToken.None);

            var modifyCommand = new ModificarTriageYValoracionCommand
            {
                TriageId = regResult.TriageId,
                ValoracionId = regResult.ValoracionId,
                MotivoConsulta = "Consulta Modificada",
                TensionArterial = "130/90",
                FrecuenciaCardiaca = 85,
                FrecuenciaRespiratoria = 20,
                Temperatura = 37.0m,
                SaturacionO2 = 97,
                EstadoConciencia = "Somnoliento",
                ViaAerea = "Permeable",
                Ventilacion = "Normal",
                Pulso = "Rítmico",
                PielMucosas = "Normocoloreada",
                LlenadoCapilar = "< 2 segundos",
                Pupilas = "Isocóricas",
                Alergias = "Ninguna",
                UsuarioRegistro = "NurseEditor"
            };

            // Act
            var result = await _modifyHandler.Handle(modifyCommand, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.MotivoConsulta.Should().Be("Consulta Modificada");
            result.TensionArterial.Should().Be("130/90");
            result.EstadoConciencia.Should().Be("Somnoliento");
            result.UsuarioRegistro.Should().Be("NurseEditor");

            // Verify in DB
            var dbTriage = await _context.TriagesEnfermeria.FindAsync(regResult.TriageId);
            dbTriage.MotivoConsulta.Should().Be("Consulta Modificada");
            dbTriage.UsuarioRegistro.Should().Be("NurseEditor");
        }

        [Fact]
        public async Task Should_ThrowException_When_ModifyTriageDoesNotExist()
        {
            // Arrange
            var modifyCommand = new ModificarTriageYValoracionCommand
            {
                TriageId = Guid.NewGuid(),
                ValoracionId = Guid.NewGuid(),
                MotivoConsulta = "Consulta Modificada",
                TensionArterial = "130/90",
                UsuarioRegistro = "NurseEditor"
            };

            // Act & Assert
            Func<Task> act = async () => await _modifyHandler.Handle(modifyCommand, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Should_ThrowException_When_ModifyValoracionDoesNotExist()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            var regResult = await _registerHandler.Handle(new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Consulta Original",
                TensionArterial = "120/80",
                EstadoConciencia = "Alerta",
                UsuarioRegistro = "Nurse"
            }, CancellationToken.None);

            var modifyCommand = new ModificarTriageYValoracionCommand
            {
                TriageId = regResult.TriageId,
                ValoracionId = Guid.NewGuid(), // Invalid
                MotivoConsulta = "Consulta Modificada",
                TensionArterial = "130/90",
                EstadoConciencia = "Somnoliento",
                ViaAerea = "Permeable",
                Ventilacion = "Normal",
                Pulso = "Rítmico",
                PielMucosas = "Normocoloreada",
                LlenadoCapilar = "< 2 segundos",
                Pupilas = "Isocóricas",
                UsuarioRegistro = "NurseEditor"
            };

            // Act & Assert
            Func<Task> act = async () => await _modifyHandler.Handle(modifyCommand, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Should_GetHistory_Successfully()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            
            await _registerHandler.Handle(new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Triage 1",
                TensionArterial = "120/80",
                EstadoConciencia = "Alerta",
                UsuarioRegistro = "Nurse"
            }, CancellationToken.None);

            await Task.Delay(20); // ensure minor time gap if needed

            await _registerHandler.Handle(new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Triage 2",
                TensionArterial = "110/70",
                EstadoConciencia = "Alerta",
                UsuarioRegistro = "Nurse"
            }, CancellationToken.None);

            var query = new GetTriageYValoracionHistoryQuery { CuentaServicioId = cuenta.Id };

            // Act
            var history = await _historyHandler.Handle(query, CancellationToken.None);

            // Assert
            history.Should().NotBeNull();
            history.Should().HaveCount(2);
            history[0].MotivoConsulta.Should().Be("Triage 2"); // Descending order
            history[1].MotivoConsulta.Should().Be("Triage 1");
        }

        [Fact]
        public async Task Should_GetNurseAuditReport_Successfully()
        {
            // Arrange
            var cuenta = await SeedBaseAccountAsync();
            
            // Seed a triage activity
            await _registerHandler.Handle(new RegistrarTriageYValoracionCommand
            {
                CuentaServicioId = cuenta.Id,
                MotivoConsulta = "Consulta Auditoria",
                TensionArterial = "120/80",
                EstadoConciencia = "Alerta",
                DescripcionRapida = "Audit-Estable",
                UsuarioRegistro = "NurseAudited"
            }, CancellationToken.None);

            // Seed a service carga activity
            var serviceDetail = new DetalleServicioCuenta(
                cuenta.Id,
                Guid.NewGuid(),
                "Insumo Quirurgico A",
                15.5m,
                0m,
                2,
                "Insumo",
                "NurseAudited"
            );
            await _context.DetallesServicioCuenta.AddAsync(serviceDetail);
            await _context.SaveChangesAsync();

            var query = new GetNurseAuditReportQuery
            {
                NurseUsername = "NurseAudited",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var auditList = await _auditHandler.Handle(query, CancellationToken.None);

            // Assert
            auditList.Should().NotBeNull();
            auditList.Should().HaveCount(2);
            
            // Should contain service load and triage registration
            auditList.Any(a => a.TipoActividad == "Triage / Constantes Vitales" && a.Usuario == "NurseAudited").Should().BeTrue();
            auditList.Any(a => a.TipoActividad == "Carga de Insumo" && a.Usuario == "NurseAudited" && a.Detalle.Contains("Insumo Quirurgico A")).Should().BeTrue();
        }
    }
}
