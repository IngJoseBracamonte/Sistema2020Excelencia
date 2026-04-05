using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyManualTest
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

            Console.WriteLine("--- ANALIZANDO METADATOS PARA SINCRONIZACIÓN ---");
            
            // 1. Paciente: BRUNEQUILDE GIL
            var p1 = await context.Pacientes.Where(p => p.NombreCompleto.Contains("BRUNEQUILDE")).FirstOrDefaultAsync();
            if (p1 != null) {
                Console.WriteLine($"Paciente: {p1.NombreCompleto} | IdPacienteLegacy: {p1.IdPacienteLegacy ?? 0}");
            } else {
                Console.WriteLine("BRUNEQUILDE GIL no encontrada.");
            }

            // 2. Servicio: PERFIL 20
            var s1 = await context.OtrosServicios.Where(s => s.Nombre.Contains("PERFIL 20")).FirstOrDefaultAsync();
            if (s1 != null) {
                Console.WriteLine($"Servicio: {s1.Nombre} | LegacyMappingId: '{s1.LegacyMappingId}' | Tipo: {s1.TipoServicio}");
            } else {
                Console.WriteLine("PERFIL 20 no encontrado.");
            }
        }
    }
}
