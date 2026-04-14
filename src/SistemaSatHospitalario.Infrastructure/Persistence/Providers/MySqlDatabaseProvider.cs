using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
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
            var conStr = configuration.GetConnectionString("LegacyConnection");
            
            // Registramos siempre el DbContext para evitar fallos de DI en el repositorio
            services.AddDbContext<Sistema2020LegacyDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(conStr))
                {
                    options.UseMySql(conStr, ServerVersion.AutoDetect(conStr));
                }
                else
                {
                    // Senior Note: Se deja sin configurar el provider para que explote
                    // con un mensaje claro de EF Core si se intenta usar sin conexión.
                    // Esto es preferible a un NullReferenceException posterior.
                }
            });
        }
    }
}
