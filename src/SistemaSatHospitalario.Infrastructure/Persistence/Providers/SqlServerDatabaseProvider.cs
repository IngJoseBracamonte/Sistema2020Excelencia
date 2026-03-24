using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using System;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Providers
{
    public class SqlServerDatabaseProvider : IDatabaseProvider
    {
        public string Name => "SqlServer";

        public void ConfigureIdentityContext(IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("IdentityConnection_SqlServer") 
                         ?? throw new InvalidOperationException("IdentityConnection_SqlServer not found.");
            
            services.AddDbContext<SatHospitalarioIdentityDbContext>(options =>
                options.UseSqlServer(conStr, 
                    b => b.MigrationsAssembly(typeof(SatHospitalarioIdentityDbContext).Assembly.FullName)));
        }

        public void ConfigureApplicationContext(IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("SystemConnection_SqlServer") 
                         ?? throw new InvalidOperationException("SystemConnection_SqlServer not found.");
            
            services.AddDbContext<SatHospitalarioDbContext>(options =>
                options.UseSqlServer(conStr, 
                    b => b.MigrationsAssembly(typeof(SatHospitalarioDbContext).Assembly.FullName)));
        }

        public void ConfigureLegacyContext(IServiceCollection services, IConfiguration configuration)
        {
            // Nota: El sistema legacy parece estar siempre en MySql según el código original
            // pero si existiera una versión SqlServer se configuraría aquí.
            var conStr = configuration.GetConnectionString("LegacyConnection");
            if (!string.IsNullOrEmpty(conStr))
            {
               // Asumimos MySql para legacy por ahora como en el código original
                services.AddDbContext<Sistema2020LegacyDbContext>(options =>
                    options.UseMySql(conStr, ServerVersion.AutoDetect(conStr)));
            }
        }
    }
}
