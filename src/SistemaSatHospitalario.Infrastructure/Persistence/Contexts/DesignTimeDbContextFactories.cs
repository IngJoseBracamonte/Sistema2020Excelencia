using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System.IO;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Contexts
{
    public class SatHospitalarioDbContextFactory : IDesignTimeDbContextFactory<SatHospitalarioDbContext>
    {
        public SatHospitalarioDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SatHospitalarioDbContext>();
            var provider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "MySql";

            if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
            {
                var conStr = "Server=localhost;Port=3306;Database=SatHospitalario;Uid=root;Pwd=;Connection Timeout=20;";
                optionsBuilder.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)));
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SistemaSatHospitalario;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            return new SatHospitalarioDbContext(optionsBuilder.Options);
        }
    }

    public class SatHospitalarioIdentityDbContextFactory : IDesignTimeDbContextFactory<SatHospitalarioIdentityDbContext>
    {
        public SatHospitalarioIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SatHospitalarioIdentityDbContext>();
            var provider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "MySql";

            if (provider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
            {
                var conStr = "Server=localhost;Port=3306;Database=SatHospitalarioIdentity;Uid=root;Pwd=;Connection Timeout=20;";
                optionsBuilder.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)));
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SistemaSatHospitalarioIdentity;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            return new SatHospitalarioIdentityDbContext(optionsBuilder.Options);
        }
    }
}
