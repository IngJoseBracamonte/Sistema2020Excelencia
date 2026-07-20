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
    public class TrasladarPacienteCommandHandlerTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new SatHospitalarioDbContext(options);

            // Seed required lookup data
            context.TiposServicio.AddRange(
                new TipoServicio(TipoServicioConstants.Medico, "Medico", "MED"),
                new TipoServicio(TipoServicioConstants.Laboratorio, "Laboratorio", "LAB"),
                new TipoServicio(TipoServicioConstants.RX, "RX", "RX"),
                new TipoServicio(TipoServicioConstants.Tomo, "Tomo", "TOMO"),
                new TipoServicio(TipoServicioConstants.Insumo, "Insumo", "INS")
            );
            context.SaveChanges();
            
            return context;
        }

        [Fact]
        public async Task Handle_TrasladoNormal_CierraCuentaAntigua_CreaCuentaHija_CalculaEstanciaCorrectamente()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            // 1. Crear Sede y Servicio Clínico
            var sedeHos = new Sede("HOS", "Hospitalizacion", false, new Guid("10000000-0000-0000-0000-000000000003"));
            var servicioEstancia = new ServicioClinico("EST-01", "Estancia General", 100.00m, "ESTANCIA", "EST-MAPPING-01");
            servicioEstancia.UnidadMedida = "Día";
            servicioEstancia.PermiteFraccionamiento = false;
            
            // 2. Cama Origen
            var camaOrigen = new AreaClinica(sedeHos.Id, "HAB-101", "Habitación 101 Cama A", true);
            camaOrigen.AsignarServicioTarifa(servicioEstancia);
            camaOrigen.MarcarComoOcupada();

            // 3. Paciente
            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, DateTime.Today.AddYears(-30));

            // 4. Cuenta Activa Anterior
            var cuentaAnterior = new CuentaServicios(paciente.Id, "user-test", "Hospitalizacion", null, camaOrigen.Id, null);
            // Simular fecha de carga de hace 40 horas (duración de 2 días al redondear al entero superior)
            cuentaAnterior.ModificarFechaCargaParaPruebas(DateTime.UtcNow.AddHours(-40));

            await context.Sedes.AddAsync(sedeHos);
            await context.ServiciosClinicos.AddAsync(servicioEstancia);
            await context.AreasClinicas.AddAsync(camaOrigen);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuentaAnterior);
            await context.SaveChangesAsync();

            var handler = new TrasladarPacienteCommandHandler(context);
            var command = new TrasladarPacienteCommand
            {
                PacienteId = paciente.Id,
                NuevoTipoIngreso = "UCI",
                NuevoConvenioId = null,
                NuevaAreaClinicaId = Guid.NewGuid(), // Nueva cama UCI
                NuevaSubAreaClinica = "UCI",
                UsuarioTraslado = "enfermero-1",
                EsEgreso = false
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cuentaAnterior.Id, result.CuentaCerradaId);
            Assert.NotNull(result.NuevaCuentaId);

            // Verificar cuenta anterior cerrada
            var cuentaCerrada = await context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaCerradaId);
            Assert.NotNull(cuentaCerrada);
            Assert.Equal(EstadoConstants.Facturada, cuentaCerrada.Estado);

            // Verificar cargo de estancia calculado (48 horas = 2 días)
            var cargoEstancia = cuentaCerrada.Detalles.FirstOrDefault(d => d.ServicioId == servicioEstancia.Id);
            Assert.NotNull(cargoEstancia);
            // No permite fraccionamiento por defecto, por lo que redondea al entero superior: Math.Ceiling(48 / 24) = 2 días
            Assert.Equal(2.00m, cargoEstancia.Cantidad);
            Assert.Equal(100.00m, cargoEstancia.Precio);

            // Verificar cama origen liberada
            var camaLiberada = await context.AreasClinicas.FirstOrDefaultAsync(a => a.Id == camaOrigen.Id);
            Assert.NotNull(camaLiberada);
            Assert.Equal(EstadoUbicacion.Disponible, camaLiberada.Estado);

            // Verificar cuenta nueva abierta
            var cuentaNueva = await context.CuentasServicios.FirstOrDefaultAsync(c => c.Id == result.NuevaCuentaId.Value);
            Assert.NotNull(cuentaNueva);
            Assert.Equal(EstadoConstants.Abierta, cuentaNueva.Estado);
            Assert.Equal("UCI", cuentaNueva.TipoIngreso);
            Assert.Equal("UCI", cuentaNueva.SubAreaClinica);
            Assert.Equal(cuentaAnterior.Id, cuentaNueva.CuentaPrincipalId);
        }

        [Fact]
        public async Task Handle_TrasladoAQuirofano_RetieneCamaOrigen_Y_AsociaCamaRetenidaEnNuevaCuenta()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            var sedeHos = new Sede("HOS", "Hospitalizacion", false, new Guid("10000000-0000-0000-0000-000000000003"));
            var servicioEstancia = new ServicioClinico("EST-01", "Estancia General", 100.00m, "ESTANCIA", "EST-MAPPING-01");
            servicioEstancia.UnidadMedida = "Día";
            servicioEstancia.PermiteFraccionamiento = false;
            
            var camaOrigen = new AreaClinica(sedeHos.Id, "HAB-101", "Habitación 101 Cama A", true);
            camaOrigen.AsignarServicioTarifa(servicioEstancia);
            camaOrigen.MarcarComoOcupada();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, DateTime.Today.AddYears(-30));
            var cuentaAnterior = new CuentaServicios(paciente.Id, "user-test", "Hospitalizacion", null, camaOrigen.Id, null);
            cuentaAnterior.ModificarFechaCargaParaPruebas(DateTime.UtcNow.AddDays(-1));

            await context.Sedes.AddAsync(sedeHos);
            await context.ServiciosClinicos.AddAsync(servicioEstancia);
            await context.AreasClinicas.AddAsync(camaOrigen);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuentaAnterior);
            await context.SaveChangesAsync();

            var handler = new TrasladarPacienteCommandHandler(context);
            var command = new TrasladarPacienteCommand
            {
                PacienteId = paciente.Id,
                NuevoTipoIngreso = "Quirofano",
                NuevoConvenioId = null,
                NuevaAreaClinicaId = null, // En quirófano no requiere cama física al ingresar
                UsuarioTraslado = "enfermero-1",
                EsEgreso = false
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var camaRetenida = await context.AreasClinicas.FirstOrDefaultAsync(a => a.Id == camaOrigen.Id);
            Assert.NotNull(camaRetenida);
            Assert.Equal(EstadoUbicacion.RetencionQuirurgica, camaRetenida.Estado);

            var cuentaNueva = await context.CuentasServicios.FirstOrDefaultAsync(c => c.Id == result.NuevaCuentaId!.Value);
            Assert.NotNull(cuentaNueva);
            Assert.Equal(camaOrigen.Id, cuentaNueva.CamaRetenidaId);
        }

        [Fact]
        public async Task Handle_MitigacionEfectoSandwich_UsaFechaHoraEgresoEfectivaParaElCalculo()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            var sedeHos = new Sede("HOS", "Hospitalizacion", false, new Guid("10000000-0000-0000-0000-000000000003"));
            var servicioEstancia = new ServicioClinico("EST-01", "Estancia General", 100.00m, "ESTANCIA", "EST-MAPPING-01");
            servicioEstancia.UnidadMedida = "Día";
            servicioEstancia.PermiteFraccionamiento = false;
            
            var camaOrigen = new AreaClinica(sedeHos.Id, "HAB-101", "Habitación 101 Cama A", true);
            camaOrigen.AsignarServicioTarifa(servicioEstancia);
            camaOrigen.MarcarComoOcupada();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, DateTime.Today.AddYears(-30));
            var cuentaAnterior = new CuentaServicios(paciente.Id, "user-test", "Hospitalizacion", null, camaOrigen.Id, null);
            // El ingreso ocurrió hace 24 horas
            cuentaAnterior.ModificarFechaCargaParaPruebas(DateTime.UtcNow.AddHours(-24));

            await context.Sedes.AddAsync(sedeHos);
            await context.ServiciosClinicos.AddAsync(servicioEstancia);
            await context.AreasClinicas.AddAsync(camaOrigen);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuentaAnterior);
            await context.SaveChangesAsync();

            var handler = new TrasladarPacienteCommandHandler(context);
            
            // La enfermera confirma el traslado 3 horas después del egreso físico real.
            // FechaHoraEgresoEfectiva indica que el egreso ocurrió hace 3 horas (duración real de la estancia = 21 horas).
            var fechaEgresoReal = DateTime.UtcNow.AddHours(-3);

            var command = new TrasladarPacienteCommand
            {
                PacienteId = paciente.Id,
                NuevoTipoIngreso = "UCI",
                NuevoConvenioId = null,
                NuevaAreaClinicaId = Guid.NewGuid(),
                UsuarioTraslado = "enfermero-1",
                EsEgreso = false,
                FechaHoraEgresoEfectiva = fechaEgresoReal
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var cuentaCerrada = await context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaCerradaId);
            Assert.NotNull(cuentaCerrada);

            var cargo = cuentaCerrada.Detalles.First();
            // 21 horas < 24 horas, no permite fraccionamiento por lo que cobra 1 día completo (Math.Ceiling(21/24) = 1)
            Assert.Equal(1.00m, cargo.Cantidad);
        }

        [Fact]
        public async Task Handle_MontoSobrescrito_AplicaTarifaFijaManual_IgnoraPrecioBase()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
            var sedeHos = new Sede("HOS", "Hospitalizacion", false, new Guid("10000000-0000-0000-0000-000000000003"));
            var servicioEstancia = new ServicioClinico("EST-01", "Estancia General", 100.00m, "ESTANCIA", "EST-MAPPING-01");
            servicioEstancia.UnidadMedida = "Día";
            servicioEstancia.PermiteFraccionamiento = false;
            
            var camaOrigen = new AreaClinica(sedeHos.Id, "HAB-101", "Habitación 101 Cama A", true);
            camaOrigen.AsignarServicioTarifa(servicioEstancia);
            camaOrigen.MarcarComoOcupada();

            var paciente = new PacienteAdmision("V-12345678", "Juan Perez", "0412-1234567", null, DateTime.Today.AddYears(-30));
            var cuentaAnterior = new CuentaServicios(paciente.Id, "user-test", "Hospitalizacion", null, camaOrigen.Id, null);
            cuentaAnterior.ModificarFechaCargaParaPruebas(DateTime.UtcNow.AddDays(-1));

            await context.Sedes.AddAsync(sedeHos);
            await context.ServiciosClinicos.AddAsync(servicioEstancia);
            await context.AreasClinicas.AddAsync(camaOrigen);
            await context.PacientesAdmision.AddAsync(paciente);
            await context.CuentasServicios.AddAsync(cuentaAnterior);
            await context.SaveChangesAsync();

            var handler = new TrasladarPacienteCommandHandler(context);
            var command = new TrasladarPacienteCommand
            {
                PacienteId = paciente.Id,
                NuevoTipoIngreso = "UCI",
                NuevoConvenioId = null,
                NuevaAreaClinicaId = Guid.NewGuid(),
                UsuarioTraslado = "enfermero-1",
                EsEgreso = false,
                MontoSobrescrito = 150.00m // Monto sobrescrito plano de USD 150
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var cuentaCerrada = await context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == result.CuentaCerradaId);
            Assert.NotNull(cuentaCerrada);

            var cargo = cuentaCerrada.Detalles.First();
            Assert.Equal(1.00m, cargo.Cantidad); // Cantidad plana = 1 para monto sobrescrito
            Assert.Equal(150.00m, cargo.Precio); // Precio base de 100 ignorado, aplica 150
        }
    }
}
