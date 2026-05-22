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
            
            Console.WriteLine("--- IMPRIMIENDO SERVICIOS CLINICOS ---");
            try {
                var services = await context.ServiciosClinicos.AsNoTracking().ToListAsync();
                foreach (var s in services)
                {
                    Console.WriteLine($"ID: {s.Id} | Cod: {s.Codigo} | Desc: {s.Descripcion} | Price: {s.PrecioBase} | Tipo: {s.TipoServicio} | HonorarioBase: {s.HonorarioBase} | Cat: {s.Category} | HonCat: {s.HonorariumCategory}");
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
