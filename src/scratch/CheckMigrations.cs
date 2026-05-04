using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Tests
{
    public static class CheckMigrations
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);
            
            Console.WriteLine("--- MIGRACIONES APLICADAS (DB) ---");
            var history = await context.Database.SqlQueryRaw<MigrationRow>("SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory").ToListAsync();
            foreach (var row in history)
            {
                Console.WriteLine($"{row.MigrationId} [{row.ProductVersion}]");
            }
        }
    }

    public class MigrationRow
    {
        public string MigrationId { get; set; }
        public string ProductVersion { get; set; }
    }
}
