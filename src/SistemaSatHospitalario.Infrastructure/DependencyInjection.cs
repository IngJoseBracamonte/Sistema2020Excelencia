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
using SistemaSatHospitalario.Infrastructure.Persistence.Providers;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace SistemaSatHospitalario.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbProviderName = configuration.GetValue<string>("DatabaseProvider") ?? "MySql";
            
            IDatabaseProvider dbProvider = dbProviderName.Equals("MySql", StringComparison.OrdinalIgnoreCase)
                ? new MySqlDatabaseProvider()
                : new SqlServerDatabaseProvider();

            // Configure Contexts using the selected strategy (SOLID: Strategy Pattern)
            dbProvider.ConfigureIdentityContext(services, configuration);
            dbProvider.ConfigureApplicationContext(services, configuration);
            dbProvider.ConfigureLegacyContext(services, configuration);

            // Registro de Interfaz para desacoplamiento (Micro-Ciclo 30 fix)
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SatHospitalarioDbContext>());

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
            services.AddHttpContextAccessor(); // Requerido para CurrentUserService
            services.AddScoped<IAuthService, JwtAuthService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>(); // [Fase 2] Identity Decoupling

            // Configuración de Inicializadores de DB (Multi-Provider Seeders)
            services.AddScoped<IDatabaseInitializer, IdentityDbInitializer>();
            services.AddScoped<IDatabaseInitializer, SystemDbInitializer>();
            services.AddScoped<IDatabaseInitializer, LegacyDbInitializer>();
            
            // Legacy Query Infrastructure (Senior Refactor)
            services.AddScoped<ILegacyQueryService, LegacyQueryService>();
            
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

            // Legacy Diagnostics (V12.3)
            services.AddSingleton<ILegacyErrorReportingService, SistemaSatHospitalario.Infrastructure.Services.LegacyErrorReportingService>();

            // [PHASE-6] Standardized Time Infrastructure
            services.AddSingleton<IDateTimeProvider, SistemaSatHospitalario.Infrastructure.Services.MachineDateTimeProvider>();

            return services;
        }
    }
}
