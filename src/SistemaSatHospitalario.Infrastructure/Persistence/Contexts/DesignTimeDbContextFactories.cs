using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

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
                var conStr = DesignTimeConnectionSettings.GetRequiredConnectionString("ConnectionStrings__mysql-system");
                optionsBuilder.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)));
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SistemaSatHospitalario;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            var context = new SatHospitalarioDbContext(optionsBuilder.Options);

            // =========================================================================
            // 🎯 DETECTOR DIRECTO DE PROPIEDAD CORRUPTA
            // =========================================================================
            var mappingSource = context.GetService<IRelationalTypeMappingSource>();
            foreach (var entity in context.Model.GetEntityTypes())
            {
                foreach (var prop in entity.GetProperties())
                {
                    try
                    {
                        var mapping = mappingSource.FindMapping(prop);
                        if (mapping == null)
                        {
                            throw new Exception($"\n\n>>> LA PROPIEDAD NO TIENE MAPEO DE MYSQL: [{entity.ClrType.Name}].[{prop.Name}] (Tipo: {prop.ClrType.FullName}) <<<\n\n");
                        }
                    }
                    catch (Exception ex) when (!ex.Message.Contains("LA PROPIEDAD"))
                    {
                        throw new Exception($"\n\n>>> LA PROPIEDAD QUE HACE CRASH ES: [{entity.ClrType.Name}].[{prop.Name}] (Tipo: {prop.ClrType.FullName}) <<<\n\n", ex);
                    }
                }
            }

            return context;
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
                var conStr = DesignTimeConnectionSettings.GetRequiredConnectionString("ConnectionStrings__mysql-identity");
                optionsBuilder.UseMySql(conStr, new MySqlServerVersion(new Version(8, 0, 21)));
            }
            else
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SistemaSatHospitalarioIdentity;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            return new SatHospitalarioIdentityDbContext(optionsBuilder.Options);
        }
    }

    internal static class DesignTimeConnectionSettings
    {
        internal static string GetRequiredConnectionString(string variableName) =>
            Environment.GetEnvironmentVariable(variableName)
            ?? throw new InvalidOperationException($"{variableName} debe configurarse para crear el contexto de diseño.");
    }
}