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
            optionsBuilder.UseMySql("Server=localhost;Database=dummyDb;Uid=root;Pwd=;", serverVersion);

            return new Sistema2020LegacyDbContext(optionsBuilder.Options);
        }
    }
}
