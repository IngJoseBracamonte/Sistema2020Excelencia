using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Infrastructure;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using SistemaSatHospitalario.Infrastructure.Persistence.Repositories;
using SistemaSatHospitalario.Infrastructure.Services;

namespace DbDiagnostics
{
    public class MockOrdenExternaService : IOrdenExternaService
    {
        public Task EnviarOrdenRXAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task EnviarOrdenTomoAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== CloseAccount Execution Diagnostic ===");

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "DatabaseProvider", "MySql" },
                    { "ConnectionStrings:mysql-system", "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None" },
                    { "ConnectionStrings:mysql-identity", "server=localhost;database=SatHospitalarioIdentity;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None" },
                    { "ConnectionStrings:LegacyConnection", "server=localhost;database=sistema2020;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None;Allow User Variables=True" }
                })
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            services.AddInfrastructureServices(config);
            services.AddScoped<IOrdenExternaService, MockOrdenExternaService>();

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SatHospitalarioDbContext>();
            var legacyDbContext = scope.ServiceProvider.GetRequiredService<Sistema2020LegacyDbContext>();
            var handler = new CloseAccountCommandHandler(
                dbContext,
                scope.ServiceProvider.GetRequiredService<ILegacyLabRepository>(),
                scope.ServiceProvider.GetRequiredService<ICajaAdministrativaRepository>(),
                scope.ServiceProvider.GetRequiredService<IBillingRepository>(),
                scope.ServiceProvider.GetRequiredService<ILegacyErrorReportingService>(),
                scope.ServiceProvider.GetRequiredService<IOrdenExternaService>()
            );

            // The account of interest:
            var targetAccountId = Guid.Parse("a419e9b4-0801-4324-9ae3-bd57134a41c2");
            Console.WriteLine($"Looking up account: {targetAccountId}");

            // 1. Cleanup previous runs
            var existingRecibos = await dbContext.RecibosFactura.Where(r => r.CuentaServicioId == targetAccountId).ToListAsync();
            if (existingRecibos.Any())
            {
                Console.WriteLine($"Deleting {existingRecibos.Count} existing RecibosFactura...");
                dbContext.RecibosFactura.RemoveRange(existingRecibos);
            }
            var existingCuentasPorCobrar = await dbContext.CuentasPorCobrar.Where(c => c.CuentaServicioId == targetAccountId).ToListAsync();
            if (existingCuentasPorCobrar.Any())
            {
                Console.WriteLine($"Deleting {existingCuentasPorCobrar.Count} existing CuentasPorCobrar...");
                dbContext.CuentasPorCobrar.RemoveRange(existingCuentasPorCobrar);
            }
            await dbContext.SaveChangesAsync();

            // 2. Set state to Abierta via raw SQL to bypass private setter and tracker issues
            int affected = await dbContext.Database.ExecuteSqlRawAsync(
                "UPDATE CuentasServicios SET Estado = 'Abierta', FechaCierre = NULL WHERE Id = {0}",
                targetAccountId.ToString()
            );
            Console.WriteLine($"Account state reset to Abierta. Affected rows: {affected}");

            var account = await dbContext.CuentasServicios
                .Include(c => c.Detalles)
                .AsNoTracking() // Refresh from database
                .FirstOrDefaultAsync(c => c.Id == targetAccountId);

            if (account == null)
            {
                Console.WriteLine("Account NOT found in SatHospitalario database.");
                return;
            }

            Console.WriteLine($"Found Account: {account.Id}");
            Console.WriteLine($"Patient ID: {account.PacienteId}");
            Console.WriteLine($"State: {account.Estado}");
            Console.WriteLine($"Total calculated: {account.CalcularTotal()}");
            Console.WriteLine("Details:");
            foreach (var detail in account.Detalles)
            {
                Console.WriteLine($" - {detail.Descripcion} ({detail.TipoServicio}): {detail.Precio} x {detail.Cantidad}");
            }

            // Create command
            var cmd = new CloseAccountCommand
            {
                CuentaId = targetAccountId,
                TasaCambio = 490.04m,
                UsuarioId = "08deaae2-cd62-4e60-8683-90de7f905080", // Playwright/Test default user ID
                UsuarioCajero = "Playwright_Bot",
                Pagos = new List<DetallePagoDto>() // No payments
            };

            Console.WriteLine("\nInvoking CloseAccountCommandHandler.Handle...");
            try
            {
                var result = await handler.Handle(cmd, CancellationToken.None);
                Console.WriteLine("[SUCCESS] Account closed successfully!");
                Console.WriteLine($"ReciboId: {result.ReciboId}");
                Console.WriteLine($"CuentaPorCobrarId: {result.CuentaPorCobrarId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR EXCEPTION THROWN]:");
                Console.WriteLine(ex.ToString());
                if (ex.InnerException != null)
                {
                    Console.WriteLine("\n[INNER EXCEPTION]:");
                    Console.WriteLine(ex.InnerException.ToString());
                }
            }
        }
    }
}
