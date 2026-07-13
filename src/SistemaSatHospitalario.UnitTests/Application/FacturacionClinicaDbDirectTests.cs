using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Dapper;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using SistemaSatHospitalario.Infrastructure.Persistence.Repositories;
using SistemaSatHospitalario.Infrastructure.Services;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class FacturacionClinicaDbDirectTests
    {
        private const string TestCedula = "V-99999999";
        private const string TestNombre = "Paciente Prueba BD";

        [Fact]
        public async Task TestFlujoFacturacionYOrdenLegacyCompleto()
        {
            var logPath = @"C:\Users\J_Bra\.gemini\antigravity-ide\brain\c2d7b1eb-b9e3-4575-bccd-f81b16ef0b9e\scratch\ef_log.txt";
            if (System.IO.File.Exists(logPath))
            {
                try { System.IO.File.Delete(logPath); } catch {}
            }

            // 1. Configurar conexiones de base de datos reales
            var systemConn = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            var legacyConn = "server=localhost;database=sistema2020;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None;Allow User Variables=True";

            var optionsSystem = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(systemConn, new MySqlServerVersion(new Version(8, 0, 21)))
                .LogTo(msg => {
                    try { System.IO.File.AppendAllText(logPath, msg + Environment.NewLine); } catch {}
                })
                .Options;

            var optionsLegacy = new DbContextOptionsBuilder<Sistema2020LegacyDbContext>()
                .UseMySql(legacyConn, new MySqlServerVersion(new Version(8, 0, 21)))
                .LogTo(msg => {
                    try { System.IO.File.AppendAllText(logPath, msg + Environment.NewLine); } catch {}
                })
                .Options;

            using var systemContext = new SatHospitalarioDbContext(optionsSystem);
            using var legacyContext = new Sistema2020LegacyDbContext(optionsLegacy);

            // Verificar conexión
            try
            {
                await systemContext.Database.OpenConnectionAsync();
                await systemContext.Database.CloseConnectionAsync();
                await legacyContext.Database.OpenConnectionAsync();
                await legacyContext.Database.CloseConnectionAsync();
            }
            catch (Exception ex)
            {
                // Si MySQL local no está levantado, omitimos la prueba (no falla en entornos CI sin DB)
                Console.WriteLine("MySQL local no disponible. Omitiendo prueba de BD directa. Detalle: " + ex.Message);
                return;
            }

            // Dump legacy tables for column diagnostic FIRST
            var legacyDbConn = legacyContext.Database.GetDbConnection();
            if (legacyDbConn.State != System.Data.ConnectionState.Open) await legacyDbConn.OpenAsync();

            try
            {
                System.IO.File.AppendAllText(logPath, "=== DIAGNOSTIC: COLUMNS OF 'ordenes' ===" + Environment.NewLine);
                var colsOrdenes = await legacyDbConn.QueryAsync<dynamic>("SHOW COLUMNS FROM ordenes");
                foreach (var c in colsOrdenes)
                {
                    System.IO.File.AppendAllText(logPath, $"COLUMN: {c.Field} ({c.Type})" + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Error showing columns from ordenes: {ex.Message}" + Environment.NewLine);
            }

            try
            {
                System.IO.File.AppendAllText(logPath, "=== DIAGNOSTIC: COLUMNS OF 'resultadospaciente' ===" + Environment.NewLine);
                var colsResultados = await legacyDbConn.QueryAsync<dynamic>("SHOW COLUMNS FROM resultadospaciente");
                foreach (var c in colsResultados)
                {
                    System.IO.File.AppendAllText(logPath, $"COLUMN resultadospaciente: {c.Field} ({c.Type})" + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Error showing columns from resultadospaciente: {ex.Message}" + Environment.NewLine);
            }

            // Instanciar servicios y repositorios requeridos para el flujo
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("ConnectionStrings")["LegacyConnection"]).Returns(legacyConn);

            var queryService = new LegacyQueryService(legacyContext);
            var legacyLogger = new LegacyErrorReportingService();
            var legacyLabRepository = new LegacyLabRepository(legacyContext, queryService, configMock.Object, legacyLogger);
            var cajaRepository = new CajaAdministrativaRepository(systemContext);
            var billingRepository = new BillingRepository(systemContext);

            // 2. Limpieza de pruebas anteriores para garantizar un estado consistente
            await LimpiarDatosDePruebaAsync(systemContext, legacyContext);

            // 3. Preparación de Catálogo y Datos Base
            // 3.1 Obtener Especialidad para evitar fallos de clave foránea en Médicos
            var especialidad = await systemContext.Especialidades.FirstOrDefaultAsync();
            if (especialidad == null)
            {
                especialidad = new Especialidad("Medicina General");
                systemContext.Especialidades.Add(especialidad);
                await systemContext.SaveChangesAsync();
            }

            // 3.2 Médico de prueba
            var doctor = await systemContext.Medicos.FirstOrDefaultAsync(m => m.Nombre == "Dr. Jose Bracamonte");
            if (doctor == null)
            {
                doctor = new Medico("Dr. Jose Bracamonte", especialidad.Id, 35.00m);
                systemContext.Medicos.Add(doctor);
                await systemContext.SaveChangesAsync();
            }

            // 3.3 Servicio Consulta General
            var consulta = await systemContext.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S001");
            if (consulta == null)
            {
                consulta = new ServicioClinico("S001", "Consulta Medica General", 30.00m, "Consulta") 
                { 
                    Category = ServiceCategory.Consultation, 
                    HonorariumCategory = "CONSULTA", 
                    HonorarioBase = 10.00m 
                };
                systemContext.ServiciosClinicos.Add(consulta);
                await systemContext.SaveChangesAsync();
            }

            // 3.4 Servicio RX
            var rx = await systemContext.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S002");
            if (rx == null)
            {
                rx = new ServicioClinico("S002", "Radiografía Tórax", 45.00m, "RX") 
                { 
                    Category = ServiceCategory.Radiology, 
                    HonorariumCategory = "RX" 
                };
                systemContext.ServiciosClinicos.Add(rx);
                await systemContext.SaveChangesAsync();
            }

            // 3.5 Medicamento por Defecto (Seguridad - Requisito del Usuario)
            var medicamento = await systemContext.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "MED-01");
            if (medicamento == null)
            {
                medicamento = new ServicioClinico("MED-01", "Ibuprofeno 600mg (Medicamento)", 5.00m, "Medicamento")
                {
                    Category = ServiceCategory.Insumo,
                    HonorariumCategory = "MEDICAMENTO"
                };
                systemContext.ServiciosClinicos.Add(medicamento);
                await systemContext.SaveChangesAsync();
            }

            // 3.6 Perfil de Laboratorio en Legado (Utilizando Dapper para respetar columnas dinámicas)
            var perfilesDisponibles = await legacyLabRepository.GetAvailableProfilesAsync(CancellationToken.None);
            var legacyProfile = perfilesDisponibles.FirstOrDefault();
            if (legacyProfile == null)
            {
                var conn = legacyContext.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

                var columns = await conn.QueryAsync<string>(
                    "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'perfil'",
                    new { db = conn.Database });
                var colList = columns.Select(c => c.ToUpper()).ToList();
                string descCol = colList.Contains("NOMBREPERFIL") ? "NombrePerfil" : "Descripcion";
                string estadoCol = colList.Contains("ACTIVO") ? "Activo" : "Estado";

                await conn.ExecuteAsync(
                    $"INSERT INTO perfil ( {descCol}, Precio, PrecioDolar, {estadoCol} ) VALUES (@Descripcion, @Precio, @PrecioDolar, @Estado)",
                    new { Descripcion = "Perfil de Prueba DB Directa", Precio = 15.00m, PrecioDolar = 15.00m, Estado = 1 });

                perfilesDisponibles = await legacyLabRepository.GetAvailableProfilesAsync(CancellationToken.None);
                legacyProfile = perfilesDisponibles.First(p => p.Descripcion == "Perfil de Prueba DB Directa");
            }

            // Mapear perfilesanalisis legacy para este perfil si no tiene (Usando Dapper directo para evitar problemas de EF)
            var hasMapping = await legacyDbConn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM perfilesanalisis WHERE IdPerfil = @IdPerfil",
                new { IdPerfil = legacyProfile.IdPerfil }) > 0;

            if (!hasMapping)
            {
                await legacyDbConn.ExecuteAsync(
                    "INSERT INTO perfilesanalisis (IdPerfil, IdAnalisis, IdOrganizador) VALUES (@IdPerfil, @IdAnalisis, @IdOrganizador)",
                    new { IdPerfil = legacyProfile.IdPerfil, IdAnalisis = 101, IdOrganizador = 1 });
            }

            // 4. Crear Paciente y Cuenta Abierta en Emergencia
            var paciente = new PacienteAdmision(TestCedula, TestNombre, "0412-9999999", null, null, "Urb. Test, Calle 2");
            systemContext.PacientesAdmision.Add(paciente);
            await systemContext.SaveChangesAsync();

            var cuenta = new CuentaServicios(paciente.Id, "admin", EstadoConstants.Emergencia);
            systemContext.CuentasServicios.Add(cuenta);
            await systemContext.SaveChangesAsync();

            // 5. Cargar todos los tipos de items (Simulando carrito de Enfermería)
            // 5.1 Carga de Consulta (Monto Base 30 + Honorario 10 = 40)
            var detConsulta = cuenta.AgregarServicio(consulta.Id, "Consulta Medica General", 40.00m, 10.00m, 1.00m, "Medico", "admin");
            detConsulta.AsignarMedicoResponsable(doctor.Id, "CONSULTA");
            systemContext.DetallesServicioCuenta.Add(detConsulta);

            // 5.2 Carga de RX
            var detRx = cuenta.AgregarServicio(rx.Id, "Radiografía Tórax", 45.00m, 0.00m, 1.00m, "RX", "admin");
            systemContext.DetallesServicioCuenta.Add(detRx);

            // 5.3 Carga de Laboratorio (Legacy Profile)
            var detLab = cuenta.AgregarServicio(Guid.Empty, $"Laboratorio - {legacyProfile.Descripcion}", 15.00m, 0.00m, 1.00m, "Laboratorio", "admin", legacyMappingId: legacyProfile.IdPerfil.ToString());
            systemContext.DetallesServicioCuenta.Add(detLab);

            // 5.4 Carga de Medicamento
            var detMed = cuenta.AgregarServicio(medicamento.Id, "Ibuprofeno 600mg (Medicamento)", 5.00m, 0.00m, 1.00m, "Medicamento", "admin");
            systemContext.DetallesServicioCuenta.Add(detMed);

            await systemContext.SaveChangesAsync();

            // Verificar montos cargados antes del checkout
            decimal totalCargado = cuenta.CalcularTotal();
            Assert.Equal(105.00m, totalCargado); // 40 (Consulta) + 45 (RX) + 15 (Lab) + 5 (Med) = 105

            // 6. Ejecutar Cierre de Cuenta (CloseAccountCommand)
            var mockOrdenExterna = new Mock<IOrdenExternaService>();

            var handler = new CloseAccountCommandHandler(
                systemContext,
                legacyLabRepository,
                cajaRepository,
                billingRepository,
                legacyLogger,
                mockOrdenExterna.Object
            );

            // Preparar pagos para liquidar la cuenta (105 USD total en efectivo)
            var command = new CloseAccountCommand
            {
                CuentaId = cuenta.Id,
                UsuarioId = "admin-user",
                UsuarioCajero = "Cajero Test",
                TasaCambio = 50.00m,
                Pagos = new List<DetallePagoDto>
                {
                    new DetallePagoDto
                    {
                        MetodoPago = "Efectivo Divisas",
                        ReferenciaBancaria = "N/A",
                        MontoAbonadoMoneda = 105.00m,
                        EquivalenteAbonadoBase = 105.00m
                    }
                },
                DestinoPaciente = "Alta Médica",
                PersonalRelevo = "N/A",
                Consolidar = false
            };

            // Act - Ejecutar el cierre de cuenta
            var result = await handler.Handle(command, CancellationToken.None);

            // 7. ASERCIONES Y VERIFICACIONES NATIVAS (SatHospitalario)
            Assert.NotNull(result);
            Assert.True(result.SincronizacionLegacyExitosa);

            // Recargar cuenta desde la base de datos
            var cuentaFacturada = await systemContext.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == cuenta.Id);

            Assert.NotNull(cuentaFacturada);
            Assert.Equal(EstadoConstants.Facturada, cuentaFacturada.Estado);
            Assert.True(cuentaFacturada.LegacyOrderId.HasValue && cuentaFacturada.LegacyOrderId.Value > 0);

            // 8. ASERCIONES Y VERIFICACIONES LEGACY (sistema2020)
            int legacyOrderId = cuentaFacturada.LegacyOrderId.Value;
            
            // Usar Dapper directo para obtener PrecioF de la orden de legado sin problemas de nombres de propiedades dinámicas
            var precioFObj = await legacyDbConn.ExecuteScalarAsync<object>(
                "SELECT PrecioF FROM ordenes WHERE idOrden = @IdOrden",
                new { IdOrden = legacyOrderId });
            
            Assert.NotNull(precioFObj);
            decimal precioFReal = decimal.Parse(precioFObj.ToString());
            Assert.Equal(15.00m, precioFReal); // Precio del Laboratorio

            // Verificar perfiles facturados en legado (Usando Dapper directo con conversion explicita)
            var precioPerfilFacturadoObj = await legacyDbConn.ExecuteScalarAsync<object>(
                "SELECT PrecioPerfil FROM perfilesfacturados WHERE idOrden = @IdOrden AND idPerfil = @IdPerfil",
                new { IdOrden = legacyOrderId, IdPerfil = legacyProfile.IdPerfil });
            
            Assert.NotNull(precioPerfilFacturadoObj);
            decimal precioPerfilFacturado = decimal.Parse(precioPerfilFacturadoObj.ToString());
            Assert.Equal(15.00m, precioPerfilFacturado);

            // Verificar stubs de resultados insertados para la orden usando Dapper directo para evitar problemas de mapping en ResultadosPacienteLegacy
            var countResultados = await legacyDbConn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM resultadospaciente WHERE IdOrden = @IdOrden",
                new { IdOrden = legacyOrderId });

            Assert.True(countResultados > 0);

            var firstIdAnalisis = await legacyDbConn.ExecuteScalarAsync<int>(
                "SELECT IdAnalisis FROM resultadospaciente WHERE IdOrden = @IdOrden LIMIT 1",
                new { IdOrden = legacyOrderId });

            // Obtener el ID de análisis esperado dinámicamente desde el mapeo real de la base de datos
            var expectedAnalisisId = await legacyDbConn.ExecuteScalarAsync<int>(
                "SELECT IdAnalisis FROM perfilesanalisis WHERE IdPerfil = @IdPerfil LIMIT 1",
                new { IdPerfil = legacyProfile.IdPerfil });

            Assert.Equal(expectedAnalisisId, firstIdAnalisis);

            Console.WriteLine("================ PRUEBA DIRECTA DE BASE DE DATOS EXITOSA ================");
            Console.WriteLine($"Paciente: {TestNombre} ({TestCedula})");
            Console.WriteLine($"Cuenta ID: {cuenta.Id} | Estado: {cuentaFacturada.Estado}");
            Console.WriteLine($"Items Cargados: Consulta ($40.00), RX ($45.00), Lab ($15.00), Medicamento ($5.00)");
            Console.WriteLine($"Total Cuenta: ${totalCargado} USD");
            Console.WriteLine($"Orden Sincronizada en Legacy MySQL: ID Orden {legacyOrderId}");
            Console.WriteLine($"Perfiles Facturados en Legacy: {legacyProfile.IdPerfil} | Precio: ${15.00m} USD");
            Console.WriteLine($"Resultados Stubs Creados en Legacy: {countResultados} registros");
            Console.WriteLine("=========================================================================");

            // 9. Limpieza final de datos de prueba
            await LimpiarDatosDePruebaAsync(systemContext, legacyContext);
        }

        private async Task LimpiarDatosDePruebaAsync(SatHospitalarioDbContext systemContext, Sistema2020LegacyDbContext legacyContext)
        {
            // Obtener el paciente de prueba si existe
            var paciente = await systemContext.PacientesAdmision
                .FirstOrDefaultAsync(p => p.CedulaPasaporte == TestCedula);

            if (paciente != null)
            {
                var cuentas = await systemContext.CuentasServicios
                    .Where(c => c.PacienteId == paciente.Id)
                    .ToListAsync();

                foreach (var c in cuentas)
                {
                    // Eliminar detalles, cuentas por cobrar, recibos y cuentas
                    var detalles = await systemContext.DetallesServicioCuenta.Where(d => d.CuentaServicioId == c.Id).ToListAsync();
                    systemContext.DetallesServicioCuenta.RemoveRange(detalles);

                    var cxc = await systemContext.CuentasPorCobrar.Where(ar => ar.CuentaServicioId == c.Id).ToListAsync();
                    systemContext.CuentasPorCobrar.RemoveRange(cxc);

                    var recibos = await systemContext.RecibosFactura.Where(r => r.CuentaServicioId == c.Id).ToListAsync();
                    systemContext.RecibosFactura.RemoveRange(recibos);
                }

                systemContext.CuentasServicios.RemoveRange(cuentas);
                systemContext.PacientesAdmision.Remove(paciente);
                await systemContext.SaveChangesAsync();
            }

            // Limpieza en el legado utilizando Dapper
            var conn = legacyContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();

            var idPersona = await conn.ExecuteScalarAsync<int?>(
                "SELECT idPersona FROM datospersonales WHERE cedula = @Cedula",
                new { Cedula = TestCedula });

            if (idPersona.HasValue)
            {
                var ordenIds = (await conn.QueryAsync<int>(
                    "SELECT idOrden FROM ordenes WHERE idPersona = @IdPersona",
                    new { IdPersona = idPersona.Value })).ToList();

                foreach (var idOrden in ordenIds)
                {
                    await conn.ExecuteAsync("DELETE FROM resultadospaciente WHERE idOrden = @IdOrden", new { IdOrden = idOrden });
                    await conn.ExecuteAsync("DELETE FROM perfilesfacturados WHERE idOrden = @IdOrden", new { IdOrden = idOrden });
                }

                if (ordenIds.Any())
                {
                    await conn.ExecuteAsync("DELETE FROM ordenes WHERE idPersona = @IdPersona", new { IdPersona = idPersona.Value });
                }

                await conn.ExecuteAsync("DELETE FROM datospersonales WHERE idPersona = @IdPersona", new { IdPersona = idPersona.Value });
            }
        }
    }
}
