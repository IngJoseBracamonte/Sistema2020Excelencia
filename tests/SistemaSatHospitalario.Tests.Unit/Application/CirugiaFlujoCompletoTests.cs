using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class CirugiaFlujoCompletoTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SatHospitalarioDbContext(options);
            return context;
        }

        [Fact]
        public async Task Should_AssignNMedicos_WithIndividualHonorarios_And_DerechoSala()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0414-0000000");
            context.PacientesAdmision.Add(paciente);

            var esp = new Especialidad("Cirugía General");
            context.Especialidades.Add(esp);

            var medico1 = new Medico("Dr. Carlos Cirujano", esp.Id, honorarioBase: 100);
            var medico2 = new Medico("Dra. Ana Anestesiologo", esp.Id, honorarioBase: 80);
            context.Medicos.AddRange(medico1, medico2);

            var cuenta = new CuentaServicios(paciente.Id, "admin", "HOSPITALIZACION", null, null, "HAB-101");
            context.CuentasServicios.Add(cuenta);

            var orden = new OrdenCirugia(
                cuenta.Id,
                paciente.Id,
                "Apendicectomía Laparoscópica",
                precioBaseUsd: 0,
                medicoId: medico1.Id,
                fechaHoraProgramada: DateTime.UtcNow.AddDays(1),
                usuarioCreacion: "enfermera1",
                salaQuirofano: "Quirófano 2",
                modalidadAnestesia: "General",
                esAlquilado: true,
                precioDerechoSalaUsd: 350.00m);

            context.OrdenesCirugia.Add(orden);
            await context.SaveChangesAsync();

            var handler = new ActualizarHonorariosYPreciosCommandHandler(context, NullLogger<ActualizarHonorariosYPreciosCommandHandler>.Instance);

            var command = new ActualizarHonorariosYPreciosCommand
            {
                OrdenCirugiaId = orden.Id,
                PrecioDerechoSalaUsd = 400.00m,
                PrecioBaseUsd = 0m,
                EsAlquilado = true,
                UsuarioId = "admin1",
                Medicos = new List<CirugiaMedicoHonorarioInputDto>
                {
                    new() { MedicoId = medico1.Id, MontoHonorarioUsd = 600.00m, EsCirujanoPrincipal = true, EspecialidadId = esp.Id },
                    new() { MedicoId = medico2.Id, MontoHonorarioUsd = 250.00m, EsCirujanoPrincipal = false, EspecialidadId = esp.Id }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var ordenActualizada = await context.OrdenesCirugia
                .Include(o => o.MedicosHonorarios)
                .FirstAsync(o => o.Id == orden.Id);

            ordenActualizada.PrecioDerechoSalaUsd.Should().Be(400.00m);
            ordenActualizada.EsAlquilado.Should().BeTrue();
            ordenActualizada.MedicosHonorarios.Should().HaveCount(2);

            var principal = ordenActualizada.MedicosHonorarios.First(m => m.EsCirujanoPrincipal);
            principal.MedicoId.Should().Be(medico1.Id);
            principal.EspecialidadId.Should().Be(esp.Id);
            principal.MontoHonorarioUsd.Should().Be(600.00m);

            var anest = ordenActualizada.MedicosHonorarios.First(m => !m.EsCirujanoPrincipal);
            anest.MedicoId.Should().Be(medico2.Id);
            anest.EspecialidadId.Should().Be(esp.Id);
            anest.MontoHonorarioUsd.Should().Be(250.00m);
        }

        [Fact]
        public async Task Should_AssignKitToCirugia_And_CustomiseInsumos_And_DispatchAtomically()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var esp = new Especialidad("Cirugía General");
            context.Especialidades.Add(esp);

            var insumo1 = new Insumo("INS-001", "Sutura Vicryl 3-0", 100, UnidadMedida.UNIDAD, 5.00m);
            var insumo2 = new Insumo("INS-002", "Gasa Estéril", 200, UnidadMedida.UNIDAD, 1.50m);
            var insumo3Extra = new Insumo("INS-003", "Bisturí #11", 50, UnidadMedida.UNIDAD, 3.00m);

            context.Insumos.AddRange(insumo1, insumo2, insumo3Extra);

            var paciente = new PacienteAdmision("V-88888888", "Maria Lopez", "");
            context.PacientesAdmision.Add(paciente);
            var cuenta = new CuentaServicios(paciente.Id, "admin", "EMERGENCIA", null, null, "BOX-1");
            context.CuentasServicios.Add(cuenta);

            var medico = new Medico("Dr. Soto", esp.Id, 100);
            context.Medicos.Add(medico);

            var orden = new OrdenCirugia(cuenta.Id, paciente.Id, "Laparotomía", 0, medico.Id, DateTime.UtcNow, "enfermera");
            context.OrdenesCirugia.Add(orden);
            await context.SaveChangesAsync();

            var handler = new AsignarKitCirugiaCommandHandler(context, NullLogger<AsignarKitCirugiaCommandHandler>.Instance);

            var command = new AsignarKitCirugiaCommand
            {
                OrdenCirugiaId = orden.Id,
                CuentaServicioId = cuenta.Id,
                SedeDespachoId = SeedConstants.SedeId_Principal,
                UsuarioId = "supervisor_inv",
                InsumosPersonalizados = new List<KitInsumoPersonalizadoDto>
                {
                    new() { InsumoId = insumo1.Id, Cantidad = 2 },
                    new() { InsumoId = insumo2.Id, Cantidad = 10 },
                    new() { InsumoId = insumo3Extra.Id, Cantidad = 1 }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var asignados = await context.InsumosCirugiasPacientes.Where(i => i.CuentaServicioId == cuenta.Id).ToListAsync();
            asignados.Should().HaveCount(3);
            asignados.First(i => i.InsumoId == insumo1.Id).CantidadEntregada.Should().Be(2);
            asignados.First(i => i.InsumoId == insumo2.Id).CantidadEntregada.Should().Be(10);
            asignados.First(i => i.InsumoId == insumo3Extra.Id).CantidadEntregada.Should().Be(1);

            // Check stock deduction (100 - 2 = 98)
            var stock1Check = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo1.Id && s.SedeId == SeedConstants.SedeId_Principal);
            stock1Check.StockActual.Should().Be(98);
        }

        [Fact]
        public async Task Should_ProcessDevolucionInsumo_And_UpdateStockAndAccount()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var insumo = new Insumo("INS-001", "Guantes Quirúrgicos 7.5", 50, UnidadMedida.UNIDAD, 2.00m);
            context.Insumos.Add(insumo);

            var paciente = new PacienteAdmision("V-99999999", "Pedro Gomez", "");
            context.PacientesAdmision.Add(paciente);
            var cuenta = new CuentaServicios(paciente.Id, "admin", "HOSPITALIZACION", null, null, "HAB-102");
            context.CuentasServicios.Add(cuenta);

            var asignado = new InsumoCirugiaPaciente(cuenta.Id, insumo.Id, cantidadEntregada: 5);
            context.InsumosCirugiasPacientes.Add(asignado);
            await context.SaveChangesAsync();

            // Salida inicial de 5 para simular consumo inicial
            var stock = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Principal);
            stock.RegistrarSalida(5);
            await context.SaveChangesAsync();

            var handler = new ProcesarDevolucionInsumoCommandHandler(context, NullLogger<ProcesarDevolucionInsumoCommandHandler>.Instance);

            var command = new ProcesarDevolucionInsumoCommand
            {
                CuentaServicioId = cuenta.Id,
                InsumoId = insumo.Id,
                CantidadDevuelta = 2,
                SedeReingresoId = SeedConstants.SedeId_Principal,
                UsuarioId = "enfermera_piso"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            var item = await context.InsumosCirugiasPacientes.FirstAsync(i => i.Id == asignado.Id);
            item.CantidadDevuelta.Should().Be(2);
            item.CantidadConsumida.Should().Be(3);

            // Stock must increase back by 2 (from 45 to 47)
            var stockActualizado = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Principal);
            stockActualizado.StockActual.Should().Be(47);
        }

        [Fact]
        public async Task Should_CreateAdHocSolicitudInsumo_And_DispatchToPatient()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var esp = new Especialidad("Cirugía General");
            context.Especialidades.Add(esp);

            var insumo = new Insumo("INS-URG", "Atropina 1mg", 30, UnidadMedida.UNIDAD, 10.00m);
            context.Insumos.Add(insumo);

            var paciente = new PacienteAdmision("V-11112222", "Lucia Diaz", "");
            context.PacientesAdmision.Add(paciente);
            var cuenta = new CuentaServicios(paciente.Id, "admin", "CIRUGIA", null, null, "QUIROFANO-1");
            context.CuentasServicios.Add(cuenta);
            var medico = new Medico("Dr. Rivas", esp.Id, 100);
            context.Medicos.Add(medico);

            var orden = new OrdenCirugia(cuenta.Id, paciente.Id, "Cesárea de Emergencia", 0, medico.Id, DateTime.UtcNow, "enfermera");
            context.OrdenesCirugia.Add(orden);
            await context.SaveChangesAsync();

            var solicitarHandler = new SolicitarInsumosExtraCommandHandler(context, NullLogger<SolicitarInsumosExtraCommandHandler>.Instance);
            var solCommand = new SolicitarInsumosExtraCommand
            {
                OrdenCirugiaId = orden.Id,
                InsumoId = insumo.Id,
                Cantidad = 2,
                AlmacenOrigenId = SeedConstants.SedeId_Principal,
                UsuarioId = "circulante_quirofano",
                Observaciones = "Urgencia intraoperatoria"
            };

            // Act 1: Solicitar
            var solicitudId = await solicitarHandler.Handle(solCommand, CancellationToken.None);
            solicitudId.Should().NotBeEmpty();

            var sol = await context.SolicitudesInsumosCirugia.FirstAsync(s => s.Id == solicitudId);
            sol.EstadoSolicitud.Should().Be(EstadoSolicitudInsumoConstants.Pendiente);

            // Act 2: Despachar
            var despacharHandler = new DespacharSolicitudExtraCommandHandler(context, NullLogger<DespacharSolicitudExtraCommandHandler>.Instance);
            var despResult = await despacharHandler.Handle(new DespacharSolicitudExtraCommand
            {
                SolicitudId = solicitudId,
                UsuarioId = "farmaceutico1"
            }, CancellationToken.None);

            // Assert
            despResult.Should().BeTrue();
            var solDespachada = await context.SolicitudesInsumosCirugia.FirstAsync(s => s.Id == solicitudId);
            solDespachada.EstadoSolicitud.Should().Be(EstadoSolicitudInsumoConstants.Despachado);

            // Consumo cargado a la cuenta
            var consumo = await context.InsumosCirugiasPacientes.FirstOrDefaultAsync(i => i.CuentaServicioId == cuenta.Id && i.InsumoId == insumo.Id);
            consumo.Should().NotBeNull();
            consumo!.CantidadEntregada.Should().Be(2);

            // Stock descontado de almacén central (30 - 2 = 28)
            var stockCheck = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Principal);
            stockCheck.StockActual.Should().Be(28);
        }

        [Fact]
        public async Task Should_ProcessReposicionStock_BetweenSedes_WithNoStockDrift()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var insumo = new Insumo("INS-GUANTES", "Guantes Estériles Talla 7.5", 100, UnidadMedida.UNIDAD, 1.20m);
            context.Insumos.Add(insumo);

            // Add stock for Sede_Emergencia and Sede_Hospitalizacion
            var stockEmergencia = new StockSede(insumo.Id, SeedConstants.SedeId_Emergencia, 20);
            var stockHospitalizacion = new StockSede(insumo.Id, SeedConstants.SedeId_Hospitalizacion, 10);
            context.StocksSedes.AddRange(stockEmergencia, stockHospitalizacion);
            await context.SaveChangesAsync();

            var handler = new ProcesarReposicionStockCommandHandler(context, NullLogger<ProcesarReposicionStockCommandHandler>.Instance);

            var command = new ProcesarReposicionStockCommand
            {
                InsumoId = insumo.Id,
                SedeOrigenId = SeedConstants.SedeId_Emergencia,
                SedeDestinoId = SeedConstants.SedeId_Hospitalizacion,
                Cantidad = 5,
                Motivo = "CambioTalla",
                UsuarioId = "supervisor_inventario",
                Observaciones = "Devolución y reposición de 5 pares"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            var stockOrigen = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Emergencia);
            var stockDestino = await context.StocksSedes.FirstAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Hospitalizacion);

            stockOrigen.StockActual.Should().Be(15); // 20 - 5
            stockDestino.StockActual.Should().Be(15); // 10 + 5

            var transferencia = await context.TransferenciasReposicionStock.FirstOrDefaultAsync(t => t.InsumoId == insumo.Id);
            transferencia.Should().NotBeNull();
            transferencia!.Cantidad.Should().Be(5);
            transferencia.Motivo.Should().Be("CambioTalla");
        }
    }
}
