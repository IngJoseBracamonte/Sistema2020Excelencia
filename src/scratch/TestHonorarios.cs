using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Tests
{
    public static class TestHonorarios
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            
            var services = new ServiceCollection();
            services.AddDbContext<SatHospitalarioDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddMemoryCache();
            services.AddScoped<IHonorariumMapperService, HonorariumMapperService>();
            services.AddScoped<IApplicationDbContext>(p => p.GetRequiredService<SatHospitalarioDbContext>());
            services.AddScoped<ICurrentUserService, TestUserService>();

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SatHospitalarioDbContext>();
            var mapper = scope.ServiceProvider.GetRequiredService<IHonorariumMapperService>();

            Console.WriteLine("--- INICIANDO TEST DE HONORARIOS DINÁMICOS ---");

            // 1. Limpiar y Preparar Reglas de Mapeo
            var existingRules = await context.HonorariumMappingRules.ToListAsync();
            context.HonorariumMappingRules.RemoveRange(existingRules);
            
            context.HonorariumMappingRules.Add(new HonorariumMappingRule("RX_", HonorarioConstants.CategoriaRX, MappingRuleType.StartsWith, 1, "TEST"));
            context.HonorariumMappingRules.Add(new HonorariumMappingRule("CONSULTA", HonorarioConstants.CategoriaConsulta, MappingRuleType.Contains, 2, "TEST"));
            context.HonorariumMappingRules.Add(new HonorariumMappingRule("INFO", HonorarioConstants.CategoriaInforme, MappingRuleType.Contains, 3, "TEST"));
            
            await context.SaveChangesAsync();
            mapper.InvalidateCache();
            Console.WriteLine("Reglas de mapeo configuradas en DB.");

            // 2. Probar Mapeos
            var testCases = new Dictionary<string, string>
            {
                { "RX_TORAX", HonorarioConstants.CategoriaRX },
                { "CONSULTA PEDIATRIA", HonorarioConstants.CategoriaConsulta },
                { "INFORME MEDICO", HonorarioConstants.CategoriaInforme },
                { "SERVICIO DESCONOCIDO", HonorarioConstants.CategoriaOtros }
            };

            bool allPassed = true;
            foreach (var test in testCases)
            {
                var result = await mapper.MapToCategoryAsync(test.Key);
                if (result == test.Value)
                {
                    Console.WriteLine($"[PASS] {test.Key} -> {result}");
                }
                else
                {
                    Console.WriteLine($"[FAIL] {test.Key} -> Esperado: {test.Value}, Recibido: {result}");
                    allPassed = false;
                }
            }

            if (allPassed)
            {
                Console.WriteLine("\n--- TEST FINALIZADO CON ÉXITO ---");
            }
            else
            {
                Console.WriteLine("\n--- TEST FALLIDO ---");
            }
        }
    }

    public class TestUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Empty;
        public string? UserName => "TestUser";
        public string? Role => "Admin";
        public bool IsAuthenticated => true;
        public bool IsInRole(string role) => true;
        public bool IsAdmin() => true;
    }
}
