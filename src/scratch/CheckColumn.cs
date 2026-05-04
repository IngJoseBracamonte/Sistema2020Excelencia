using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Tests
{
    public static class CheckColumn
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);
            
            Console.WriteLine("--- VERIFICANDO COLUMNA CUENTASERVICIOID1 ---");
            try {
                // Intentamos una consulta que falle si la columna no existe
                await context.Database.ExecuteSqlRawAsync("SELECT CuentaServicioId1 FROM DetallesServicioCuenta LIMIT 1");
                Console.WriteLine("LA COLUMNA EXISTE.");
            } catch (Exception) {
                Console.WriteLine("LA COLUMNA NO EXISTE.");
            }
        }
    }
}
