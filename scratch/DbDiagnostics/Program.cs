using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure;

namespace DbDiagnostics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Identity DB Diagnostics ===");

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
            var identityDbContext = scope.ServiceProvider.GetRequiredService<SatHospitalarioIdentityDbContext>();

            Console.WriteLine("Fetching all users from SatHospitalarioIdentity...");
            var users = await identityDbContext.Users.ToListAsync();
            Console.WriteLine($"Total users found: {users.Count}");

            foreach (var user in users)
            {
                Console.WriteLine($"- ID: {user.Id}, UserName: {user.UserName}, EsActivo: {user.EsActivo}, RequirePasswordReset: {user.RequirePasswordReset}");
            }

            var rxUser = users.FirstOrDefault(u => u.UserName?.ToLower() == "user_rx");
            if (rxUser != null)
            {
                Console.WriteLine("\n[user_rx found]");
                if (args.Contains("--set-reset"))
                {
                    rxUser.RequirePasswordReset = true;
                    await identityDbContext.SaveChangesAsync();
                    Console.WriteLine("Successfully set RequirePasswordReset = true for user_rx!");
                }
            }
            else
            {
                Console.WriteLine("\n[WARNING] user_rx was NOT found in the database!");
            }
        }
    }
}
