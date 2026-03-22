using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Identity.Services;
using SistemaSatHospitalario.Infrastructure.Persistence.Repositories;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using SistemaSatHospitalario.Infrastructure.Integration;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace SistemaSatHospitalario.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbProvider = configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

            services.AddDbContext<SatHospitalarioIdentityDbContext>(options =>
            {
                if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
                {
                    var conStr = configuration.GetConnectionString("IdentityConnection_MySql") 
                                 ?? throw new InvalidOperationException("IdentityConnection_MySql not found.");
                    options.UseMySql(conStr, ServerVersion.AutoDetect(conStr), 
                        b => b.MigrationsAssembly(typeof(SatHospitalarioIdentityDbContext).Assembly.FullName));
                }
                else
                {
                    var conStr = configuration.GetConnectionString("IdentityConnection_SqlServer") 
                                 ?? throw new InvalidOperationException("IdentityConnection_SqlServer not found.");
                    options.UseSqlServer(conStr, 
                        b => b.MigrationsAssembly(typeof(SatHospitalarioIdentityDbContext).Assembly.FullName));
                }
            });

            services.AddDbContext<SatHospitalarioDbContext>(options =>
            {
                if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
                {
                    var conStr = configuration.GetConnectionString("SystemConnection_MySql") 
                                 ?? throw new InvalidOperationException("SystemConnection_MySql not found.");
                    options.UseMySql(conStr, ServerVersion.AutoDetect(conStr), 
                        b => {
                            b.MigrationsAssembly(typeof(SatHospitalarioDbContext).Assembly.FullName);
                            b.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
                        });
                }
                else
                {
                    var conStr = configuration.GetConnectionString("SystemConnection_SqlServer") 
                                 ?? throw new InvalidOperationException("SystemConnection_SqlServer not found.");
                    options.UseSqlServer(conStr, 
                        b => b.MigrationsAssembly(typeof(SatHospitalarioDbContext).Assembly.FullName));
                }
            });

            // Registro de Interfaz para desacoplamiento (Micro-Ciclo 30 fix)
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SatHospitalarioDbContext>());

            services.AddDbContext<Sistema2020LegacyDbContext>(options =>
            {
                var constr = configuration.GetConnectionString("LegacyConnection");
                options.UseMySql(constr, ServerVersion.AutoDetect(constr));
            });

            services.AddIdentity<UsuarioHospital, IdentityRole<Guid>>(options =>
            {
                // Configuraciones de políticas
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = false; // Desactivado por solicitud (Micro-Ciclo 30)
            })
            .AddEntityFrameworkStores<SatHospitalarioIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.AddMemoryCache();
            services.AddScoped<IAuthService, JwtAuthService>();

            // Configuración de Inicializadores de DB (Multi-Provider Seeders)
            services.AddScoped<IDatabaseInitializer, IdentityDbInitializer>();
            services.AddScoped<IDatabaseInitializer, SystemDbInitializer>();
            
            // Configuración de Repositorio Legacy con Caching (Decorator Pattern)
            services.AddScoped<LegacyLabRepository>();
            services.AddScoped<ILegacyLabRepository>(provider => 
                new CachedLegacyLabRepository(
                    provider.GetRequiredService<LegacyLabRepository>(), 
                    provider.GetRequiredService<IMemoryCache>())
            );

            services.AddScoped<ITurnoMedicoRepository, TurnoMedicoRepository>();
            services.AddScoped<IAuditoriaIncidenciaRepository, AuditoriaIncidenciaRepository>();
            services.AddScoped<ICajaAdministrativaRepository, CajaAdministrativaRepository>();
            services.AddScoped<IOrdenRepository, OrdenRepository>();
            services.AddScoped<IBillingRepository, BillingRepository>();
            services.AddScoped<IOrdenExternaService, OrdenExternaService>();

            // Email Integration
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
