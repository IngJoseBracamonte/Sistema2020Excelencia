using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure;

namespace DbDiagnostics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Database Table Columns Diagnostics ===");

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

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            
            // Check Legacy Table Columns
            try
            {
                var legacyDbContext = scope.ServiceProvider.GetRequiredService<Sistema2020LegacyDbContext>();
                var connection = legacyDbContext.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SHOW COLUMNS FROM datospersonales";
                using var reader = await cmd.ExecuteReaderAsync();
                
                Console.WriteLine("\nColumns in sistema2020.datospersonales:");
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"- {reader.GetString(0)} ({reader.GetString(1)})");
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying legacy table datospersonales: {ex.Message}");
            }

            // Check Native Table Columns
            try
            {
                var nativeDbContext = scope.ServiceProvider.GetRequiredService<SatHospitalarioDbContext>();
                var connection = nativeDbContext.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SHOW COLUMNS FROM PacientesAdmision";
                using var reader = await cmd.ExecuteReaderAsync();
                
                Console.WriteLine("\nColumns in SatHospitalario.PacientesAdmision:");
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"- {reader.GetString(0)} ({reader.GetString(1)})");
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying native table PacientesAdmision: {ex.Message}");
            }
        }
    }
}
