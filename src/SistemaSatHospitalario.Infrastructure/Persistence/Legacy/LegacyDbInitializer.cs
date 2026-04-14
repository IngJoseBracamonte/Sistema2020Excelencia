using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;

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
            var connStr = _configuration.GetConnectionString("LegacyConnection");

            if (string.IsNullOrWhiteSpace(connStr))
            {
                _logger.LogCritical("ERROR CRÍTICO: La cadena de conexión 'LegacyConnection' está vacía o no existe. El sistema Legacy no puede ser accesado.");
                throw new InvalidOperationException("LegacyConnection string is missing or empty. Please configure it in your environment variables as ConnectionStrings__LegacyConnection.");
            }

            try
            {
                _logger.LogInformation("Verificando conectividad fuerte con Legacy Database...");

                await RunConnectionTestAsync(connStr);
                
                _logger.LogInformation("Legacy Database Inicializada (Check de Conexión Fuerte OK).");
            }
            catch (Exception ex) when (ex.Message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Fallo por 'Base de Datos desconocida'. Intentando auto-recuperación mediante normalización a minúsculas...");
                
                try 
                {
                    var builder = new MySqlConnector.MySqlConnectionStringBuilder(connStr);
                    builder.Database = builder.Database.ToLowerInvariant();
                    var normalizedConnStr = builder.ConnectionString;

                    await RunConnectionTestAsync(normalizedConnStr);
                    _logger.LogInformation("✅ Conexión con Legacy Database establecida tras normalización (Uso de '{DbName}').", builder.Database);
                }
                catch (Exception retryEx)
                {
                    _logger.LogCritical(retryEx, "ERROR CRÍTICO: No se pudo conectar a la base de datos Legacy tras intento de normalización.");
                    throw new Exception($"Fallo crítico al inicializar la conexión Legacy: {retryEx.Message}", retryEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ERROR CRÍTICO: No se pudo conectar a la base de datos Legacy.");
                throw new Exception($"Fallo crítico al inicializar la conexión Legacy: {ex.Message}", ex);
            }
        }

        private async Task RunConnectionTestAsync(string connStr)
        {
            using var connection = new MySqlConnector.MySqlConnection(connStr);
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT 1";
            await cmd.ExecuteScalarAsync();
        }
    }
}
