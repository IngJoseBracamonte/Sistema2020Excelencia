using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Tests
{
    public static class AddMetadataColumns
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);
            
            Console.WriteLine("--- ADICIÓN DE COLUMNAS DE METADATA EN CUENTASPORCOBRAR ---");
            try 
            {
                // Agregando columnas si no existen
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE CuentasPorCobrar ADD COLUMN IF NOT EXISTS QuienAutorizo VARCHAR(500) NULL");
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE CuentasPorCobrar ADD COLUMN IF NOT EXISTS DoctorProcedimiento VARCHAR(500) NULL");
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE CuentasPorCobrar ADD COLUMN IF NOT EXISTS InformacionAdicional VARCHAR(2000) NULL");
                Console.WriteLine("COLUMNAS DE METADATA AGREGADAS/VERIFICADAS CON ÉXITO.");
            } 
            catch (Exception ex) 
            {
                Console.WriteLine("ERROR AL AGREGAR COLUMNAS: " + ex.Message);
            }
        }
    }
}
