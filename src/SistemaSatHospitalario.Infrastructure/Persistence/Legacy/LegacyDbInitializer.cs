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

                // Verificación fuerte directa contra MySQL
                using var connection = new MySqlConnector.MySqlConnection(connStr);
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();

                _logger.LogInformation("Legacy Database Inicializada (Check de Conexión Fuerte OK).");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ERROR CRÍTICO: No se pudo conectar a la base de datos Legacy.");
                throw new Exception($"Fallo crítico al inicializar la conexión Legacy: {ex.Message}", ex);
            }
        }
    }
}
