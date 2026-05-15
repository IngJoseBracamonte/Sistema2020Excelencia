using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;

namespace SistemaSatHospitalario.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private SqliteConnection? _identityConnection;
        private SqliteConnection? _appConnection;
        private SqliteConnection? _legacyConnection;
        private readonly string _id = Guid.NewGuid().ToString("n").Substring(0, 8);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((context, config) => {
                config.AddInMemoryCollection(new Dictionary<string, string?> {
                    {"DatabaseProvider", "Sqlite"}
                });
            });
            
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<SatHospitalarioDbContext>) ||
                         d.ServiceType == typeof(DbContextOptions<SatHospitalarioIdentityDbContext>) ||
                         d.ServiceType == typeof(DbContextOptions<Sistema2020LegacyDbContext>)).ToList();

                foreach (var descriptor in dbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Use separate file-based SQLite databases for isolation
                var dbIdentity = $"test_identity_{_id}.db";
                var dbApp = $"test_app_{_id}.db";
                var dbLegacy = $"test_legacy_{_id}.db";

                _identityConnection = new SqliteConnection($"Data Source={dbIdentity}");
                _appConnection = new SqliteConnection($"Data Source={dbApp}");
                _legacyConnection = new SqliteConnection($"Data Source={dbLegacy}");

                _identityConnection.Open();
                _appConnection.Open();
                _legacyConnection.Open();

                services.AddDbContext<SatHospitalarioIdentityDbContext>(options => options.UseSqlite(_identityConnection));
                services.AddDbContext<SatHospitalarioDbContext>(options => options.UseSqlite(_appConnection));
                services.AddDbContext<Sistema2020LegacyDbContext>(options => options.UseSqlite(_legacyConnection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            _identityConnection?.Close();
            _appConnection?.Close();
            _legacyConnection?.Close();
            
            _identityConnection?.Dispose();
            _appConnection?.Dispose();
            _legacyConnection?.Dispose();

            base.Dispose(disposing);
            
            // Clean up files
            try {
                if (File.Exists($"test_identity_{_id}.db")) File.Delete($"test_identity_{_id}.db");
                if (File.Exists($"test_app_{_id}.db")) File.Delete($"test_app_{_id}.db");
                if (File.Exists($"test_legacy_{_id}.db")) File.Delete($"test_legacy_{_id}.db");
            } catch { }
        }
    }
}
