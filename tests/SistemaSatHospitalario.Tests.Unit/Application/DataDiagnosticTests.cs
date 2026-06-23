using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class DataDiagnosticTests
    {
        private readonly ITestOutputHelper _output;

        public DataDiagnosticTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Diagnostic_CheckMappeoData()
        {
            string connectionString = "Server=localhost;Port=3306;Database=sathospitalario;Uid=root;Pwd=Labordono1818;Allow User Variables=True";
            
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);

            _output.WriteLine("--- ANALIZANDO RECIENTES TICKET DE ERROR ---");
            
            _output.WriteLine("--- PROBANDO QUERY DE CONVENIOS ---");
            try
            {
                var convenios = await context.SegurosConvenios.AsNoTracking().ToListAsync();
                _output.WriteLine($"Convenios count: {convenios.Count}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"ERROR CONVENIOS: {ex.Message}\n{ex.StackTrace}");
            }

            _output.WriteLine("--- PROBANDO QUERY DE MEDICOS ---");
            try
            {
                var medicos = await context.Medicos.AsNoTracking().ToListAsync();
                _output.WriteLine($"Medicos count: {medicos.Count}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"ERROR MEDICOS: {ex.Message}\n{ex.StackTrace}");
            }

            _output.WriteLine("--- PROBANDO DETALLES DE GETBUSINESSINSIGHTSQUERY ---");
            try
            {
                var todayUtc = DateTime.UtcNow.Date;
                var tomorrowUtc = todayUtc.AddDays(1);

                _output.WriteLine("1. Probando Query de Ventas...");
                var totalVentasHoy = await context.DetallesPago
                    .AsNoTracking()
                    .Where(d => d.FechaPago >= todayUtc && d.FechaPago < tomorrowUtc && d.ReciboFactura.EstadoFiscal != "Anulada")
                    .SumAsync(d => d.EquivalenteAbonadoBase);
                _output.WriteLine($"Total Ventas: {totalVentasHoy}");

                _output.WriteLine("2. Probando Query de CuentasServicios...");
                var pacientesAtendidosHoy = await context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != "Anulada")
                    .CountAsync();
                _output.WriteLine($"Pacientes Atendidos: {pacientesAtendidosHoy}");

                _output.WriteLine("3. Probando Query de SaldoPendienteAR...");
                var saldoPendienteAR = await context.CuentasPorCobrar
                    .AsNoTracking()
                    .Where(ar => ar.Estado == "Pendiente" || ar.Estado == "Parcial")
                    .SumAsync(ar => ar.MontoTotalBase - ar.MontoPagadoBase);
                _output.WriteLine($"Saldo Pendiente AR: {saldoPendienteAR}");

                _output.WriteLine("4. Probando Query de Ventas por Especialidad...");
                var specialtyDetails = await context.CuentasServicios
                    .AsNoTracking()
                    .Where(c => c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != "Anulada")
                    .SelectMany(c => c.Detalles)
                    .Select(d => new { d.TipoServicio, d.Precio, d.Cantidad })
                    .ToListAsync();
                _output.WriteLine($"Detalles especialidad count: {specialtyDetails.Count}");

                _output.WriteLine("5. Probando Query de Ventas por Seguro (JOIN)...");
                var rawInsuranceData = await (from c in context.CuentasServicios.AsNoTracking()
                                               join s in context.SegurosConvenios.AsNoTracking() on c.ConvenioId equals (int?)s.Id into joinSeg
                                               from s in joinSeg.DefaultIfEmpty()
                                               where c.FechaCarga >= todayUtc && c.FechaCarga < tomorrowUtc && c.Estado != "Anulada"
                                               select new
                                               {
                                                   SeguroNombre = s != null ? s.Nombre : null,
                                                   Detalles = c.Detalles.Select(d => new { d.Precio, d.Cantidad })
                                               })
                                               .ToListAsync();
                _output.WriteLine($"Seguro data count: {rawInsuranceData.Count}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"ERROR FINANCE INSIGHTS: {ex.Message}\n{ex.StackTrace}");
            }

        }
    }
}
