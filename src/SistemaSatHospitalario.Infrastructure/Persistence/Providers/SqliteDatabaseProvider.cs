using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using System;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Providers
{
    public class SqliteDatabaseProvider : IDatabaseProvider
    {
        public string Name => "Sqlite";

        public void ConfigureIdentityContext(IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("IdentityConnection_Sqlite") ?? "Data Source=identity.db";
            services.AddDbContext<SatHospitalarioIdentityDbContext>(options =>
                options.UseSqlite(conStr, b => b.MigrationsAssembly(typeof(SatHospitalarioIdentityDbContext).Assembly.FullName)));
        }

        public void ConfigureApplicationContext(IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("SystemConnection_Sqlite") ?? "Data Source=application.db";
            services.AddDbContext<SatHospitalarioDbContext>(options =>
                options.UseSqlite(conStr, b => b.MigrationsAssembly(typeof(SatHospitalarioDbContext).Assembly.FullName)));
        }

        public void ConfigureLegacyContext(IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("LegacyConnection_Sqlite") ?? "Data Source=legacy.db";
            services.AddDbContext<Sistema2020LegacyDbContext>(options =>
                options.UseSqlite(conStr, b => b.MigrationsAssembly(typeof(Sistema2020LegacyDbContext).Assembly.FullName)));
        }
    }
}
