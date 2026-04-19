using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using System;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Providers
{
    public class MySqlDatabaseProvider : IDatabaseProvider
    {
        public string Name => "MySql";

        public void ConfigureIdentityContext(IServiceCollection services, IConfiguration configuration)
        {
            // Priorizamos el nombre orquestado por Aspire "mysql-identity"
            var conStr = configuration.GetConnectionString("mysql-identity") 
                         ?? configuration.GetConnectionString("DefaultConnection")
                         ?? configuration.GetConnectionString("IdentityConnection_MySql")
                         ?? configuration["ConnectionStrings:mysql-identity"] // Direct mapping fallback
                         ?? throw new InvalidOperationException("mysql-identity (or DefaultConnection) connection string not found.");
            
            services.AddDbContext<SatHospitalarioIdentityDbContext>(options =>
                options.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)), 
                    b => {
                        b.MigrationsAssembly(typeof(SatHospitalarioIdentityDbContext).Assembly.FullName);
                        b.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
                    }));
        }

        public void ConfigureApplicationContext(IServiceCollection services, IConfiguration configuration)
        {
            // Priorizamos el nombre orquestado por Aspire "mysql-system"
            var conStr = configuration.GetConnectionString("mysql-system") 
                         ?? configuration.GetConnectionString("DefaultConnection")
                         ?? configuration.GetConnectionString("SystemConnection_MySql")
                         ?? configuration["ConnectionStrings:mysql-system"]
                         ?? throw new InvalidOperationException("mysql-system (or DefaultConnection) connection string not found.");
            
            services.AddDbContext<SatHospitalarioDbContext>(options =>
                options.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)), 
                    b => {
                        b.MigrationsAssembly(typeof(SatHospitalarioDbContext).Assembly.FullName);
                        b.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
                    }));
        }

        public void ConfigureLegacyContext(IServiceCollection services, IConfiguration configuration)
        {
            var rawConStr = configuration.GetConnectionString("LegacyConnection");
            
            services.AddDbContext<Sistema2020LegacyDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(rawConStr))
                {
                    // Senior Architecture: Normalize and Enhance for Cloud (SSL, etc)
                    var conStr = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConStr, forceLowercase: false);
                    conStr = ConnectionStringHelper.EnhanceForCloud(conStr);

                    ServerVersion version;
                    try 
                    {
                        // Intentamos auto-detectar pero con un timeout corto para no colgar el arranque
                        version = ServerVersion.AutoDetect(conStr);
                    }
                    catch
                    {
                        // Fallback para nubes gestionadas (Aiven suele ser MySQL 8.0+)
                        version = new MySqlServerVersion(new Version(8, 0, 21));
                    }

                    options.UseMySql(conStr, version);
                }
                else
                {
                    // El DbContext se registra pero sin Provider configurado si la cadena falta.
                }
            });
        }
    }
}
