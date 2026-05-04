using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Tests
{
    public static class FixMigrations
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);
            
            Console.WriteLine("--- REPARANDO HISTORIAL DE MIGRACIONES ---");
            
            try {
                await context.Database.ExecuteSqlRawAsync("INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260504190537_AddHonorarioConfigYAsignacion', '9.0.2')");
                Console.WriteLine("Registrada migración: 20260504190537_AddHonorarioConfigYAsignacion");
            } catch (Exception ex) {
                Console.WriteLine($"Error al registrar 20260504190537: {ex.Message}");
            }

            Console.WriteLine("--- FIN DE REPARACIÓN ---");
        }
    }
}
