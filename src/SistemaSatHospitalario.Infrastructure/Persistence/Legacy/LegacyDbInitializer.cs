using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using MySqlConnector;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyDbInitializer : IDatabaseInitializer
    {
        private readonly Sistema2020LegacyDbContext _context;
        private readonly ILogger<LegacyDbInitializer> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public LegacyDbInitializer(
            Sistema2020LegacyDbContext context, 
            ILogger<LegacyDbInitializer> logger,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            var rawConnStr = _configuration.GetConnectionString("LegacyConnection");

            if (string.IsNullOrWhiteSpace(rawConnStr))
            {
                _logger.LogCritical("ERROR CRÍTICO: La cadena de conexión 'LegacyConnection' está vacía o no existe.");
                throw new InvalidOperationException("LegacyConnection string is missing or empty.");
            }

            // Senior Architecture: Normalize and Enhance for Cloud
            var conStr = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConnStr);
            conStr = ConnectionStringHelper.EnhanceForCloud(conStr);

            try
            {
                var builder = new MySqlConnectionStringBuilder(conStr);
                _logger.LogInformation("Verificando existencia de base de datos Legacy en {Host}:{Port}...", builder.Server, builder.Port);
                
                await EnsureDatabaseExistsAsync(conStr);

                _logger.LogInformation("Aplicando migraciones a Legacy Database (Enforcing Primary Keys)...");
                
                // Senior Defensive Pattern: Asegurar que sql_require_primary_key no bloquee la creación de __EFMigrationsHistory
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();
                
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SET SESSION sql_require_primary_key = 0;";
                    await cmd.ExecuteNonQueryAsync();
                }

                await _context.Database.MigrateAsync();
                
                _logger.LogInformation("Legacy Database Inicializada y Migrada Correctamente.");
            }
            catch (Exception ex)
            {
                // Extraemos info de host para el log de error
                var builder = new MySqlConnectionStringBuilder(conStr);
                _logger.LogCritical(ex, "ERROR CRÍTICO: No se pudo inicializar la base de datos Legacy en {Host}:{Port}.", builder.Server, builder.Port);
                throw new Exception($"Fallo crítico al inicializar la conexión Legacy ({builder.Server}): {ex.Message}", ex);
            }
        }

        private async Task EnsureDatabaseExistsAsync(string connStr)
        {
            var builder = new MySqlConnectionStringBuilder(connStr);
            var databaseName = builder.Database;

            // Conectamos sin base de datos seleccionada para poder crearla
            builder.Database = null; 
            var serverConnStr = builder.ConnectionString;

            using var connection = new MySqlConnection(serverConnStr);
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4;";
            await cmd.ExecuteNonQueryAsync();
            
            _logger.LogInformation($"Base de datos `{databaseName}` verificada/creada.");
        }
    }
}
