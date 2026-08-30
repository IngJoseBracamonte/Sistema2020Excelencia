using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class Sistema2020LegacyDbContextFactory : IDesignTimeDbContextFactory<Sistema2020LegacyDbContext>
    {
        public Sistema2020LegacyDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Sistema2020LegacyDbContext>();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21)); // Dummy version for DesignTime
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LegacyConnection")
                    ?? throw new InvalidOperationException("ConnectionStrings__LegacyConnection debe configurarse para crear el contexto de diseño.");
                optionsBuilder.UseMySql(connectionString, serverVersion);

            return new Sistema2020LegacyDbContext(optionsBuilder.Options);
        }
    }
}
