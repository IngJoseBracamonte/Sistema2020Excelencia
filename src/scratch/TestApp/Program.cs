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
            var optionsBuilder = new DbContextOptionsBuilder<SatHospitalarioDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            try
            {
                using var context = new SatHospitalarioDbContext(optionsBuilder.Options);
                
                Console.WriteLine("--- RUNNING CUENTAS DE SERVICIO QUERY VIA EF CORE ---");
                var query = context.CuentasServicios
                    .Include(c => c.Detalles)
                    .Include(c => c.Paciente)
                    .Include(c => c.Convenio)
                    .AsNoTracking();
                
                var list = await query.ToListAsync();
                Console.WriteLine($"Successfully loaded {list.Count} accounts!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}



