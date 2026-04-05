using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CheckData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Main DB Connection (MySQL version of SatHospitalarioDbContext)
            string connectionString = "Server=localhost;Port=3306;Database=sistemasat;Uid=root;Pwd=Labordono1818;Allow User Variables=True";
            
            var services = new ServiceCollection();
            services.AddDbContext<SatHospitalarioDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SatHospitalarioDbContext>();

            Console.WriteLine("--- DATOS DE PACIENTE ---");
            var paciente = await context.Pacientes
                .Where(p => p.NombreCompleto.Contains("BRUNEQUILDE"))
                .FirstOrDefaultAsync();

            if (paciente != null)
            {
                Console.WriteLine($"Nombre: {paciente.NombreCompleto}");
                Console.WriteLine($"Legacy ID: {paciente.IdPacienteLegacy}");
            }
            else
            {
                Console.WriteLine("Paciente 'BRUNEQUILDE' no encontrado.");
                var anyPaciente = await context.Pacientes.Where(p => p.IdPacienteLegacy > 0).Take(1).FirstOrDefaultAsync();
                if (anyPaciente != null) Console.WriteLine($"Sugerencia: Usa a '{anyPaciente.NombreCompleto}' (Legacy ID: {anyPaciente.IdPacienteLegacy})");
            }

            Console.WriteLine("\n--- DATOS DE SERVICIO (PERFIL 20) ---");
            var servicio = await context.OtrosServicios
                .Where(s => s.Nombre.Contains("PERFIL 20"))
                .FirstOrDefaultAsync();

            if (servicio != null)
            {
                Console.WriteLine($"Servicio: {servicio.Nombre}");
                Console.WriteLine($"Legacy Mapping ID: {servicio.LegacyMappingId}");
                Console.WriteLine($"Tipo: {servicio.TipoServicio}");
            }
            else
            {
                Console.WriteLine("Servicio 'PERFIL 20' no encontrado.");
            }
        }
    }
}
