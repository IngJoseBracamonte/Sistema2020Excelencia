using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.System
{
    /// <summary>
    /// Endpoint de diagnóstico para verificar el estado de conectividad
    /// con los sistemas de base de datos (Legacy, System, Identity).
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(IConfiguration configuration, ILogger<DiagnosticsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Verifica la conectividad con el Sistema Legacy (Sistema2020) y reporta
        /// estadísticas básicas de las tablas clave.
        /// </summary>
        [HttpGet("legacy-status")]
        public async Task<IActionResult> GetLegacyStatus()
        {
            var connStr = _configuration.GetConnectionString("LegacyConnection");

            if (string.IsNullOrEmpty(connStr))
            {
                _logger.LogWarning("[DIAGNOSTICS] LegacyConnection string is empty or not configured.");
                return Ok(new
                {
                    Status = "NOT_CONFIGURED",
                    Message = "La cadena de conexión 'LegacyConnection' está vacía o no existe en la configuración.",
                    ConnectionStringPresent = false,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                // Enmascarar credenciales para el reporte
                var builder = new MySqlConnectionStringBuilder(connStr);
                var maskedHost = builder.Server;
                var maskedDb = builder.Database;
                var maskedUser = builder.UserID;

                using var connection = new MySqlConnection(connStr);
                await connection.OpenAsync();

                // Test básico de conectividad
                using var cmdPing = connection.CreateCommand();
                cmdPing.CommandText = "SELECT 1";
                await cmdPing.ExecuteScalarAsync();

                // Estadísticas de tablas clave
                var stats = new
                {
                    Pacientes = await GetTableCountAsync(connection, "datospersonales"),
                    Ordenes = await GetTableCountAsync(connection, "ordenes"),
                    Perfiles = await GetTableCountAsync(connection, "perfil"),
                    PerfilesFacturados = await GetTableCountAsync(connection, "perfilesfacturados"),
                    ResultadosPaciente = await GetTableCountAsync(connection, "resultadospaciente")
                };

                await connection.CloseAsync();

                _logger.LogInformation("[DIAGNOSTICS] Legacy connection OK to {Host}/{Database}", maskedHost, maskedDb);

                return Ok(new
                {
                    Status = "CONNECTED",
                    Message = "Conexión exitosa al Sistema Legacy.",
                    ConnectionStringPresent = true,
                    Server = maskedHost,
                    Database = maskedDb,
                    User = maskedUser,
                    TableCounts = stats,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "[DIAGNOSTICS] MySQL error connecting to Legacy system.");
                return Ok(new
                {
                    Status = "ERROR",
                    Message = $"Error MySQL al conectar con Legacy: {ex.Message}",
                    ErrorCode = ex.Number,
                    ConnectionStringPresent = true,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DIAGNOSTICS] Unexpected error connecting to Legacy system.");
                return Ok(new
                {
                    Status = "ERROR",
                    Message = $"Error inesperado: {ex.Message}",
                    InnerError = ex.InnerException?.Message,
                    ConnectionStringPresent = true,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Resumen rápido de todas las conexiones del sistema.
        /// </summary>
        [HttpGet("connection-summary")]
        public async Task<IActionResult> GetConnectionSummary()
        {
            var results = new
            {
                Legacy = await TestConnectionAsync(_configuration.GetConnectionString("LegacyConnection"), "LegacyConnection"),
                System = await TestConnectionAsync(
                    _configuration.GetConnectionString("mysql-system") 
                    ?? _configuration.GetConnectionString("SystemConnection_MySql"), "mysql-system"),
                Identity = await TestConnectionAsync(
                    _configuration.GetConnectionString("mysql-identity") 
                    ?? _configuration.GetConnectionString("IdentityConnection_MySql"), "mysql-identity"),
                Timestamp = DateTime.UtcNow
            };

            return Ok(results);
        }

        private static async Task<int> GetTableCountAsync(MySqlConnection connection, string tableName)
        {
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM `{tableName}`";
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch
            {
                return -1; // Tabla no existe o error
            }
        }

        private static async Task<object> TestConnectionAsync(string? connStr, string name)
        {
            if (string.IsNullOrEmpty(connStr))
            {
                return new { Name = name, Status = "NOT_CONFIGURED", Server = (string?)null, Database = (string?)null };
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connStr);
                using var connection = new MySqlConnection(connStr);
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();

                await connection.CloseAsync();
                return new { Name = name, Status = "CONNECTED", Server = builder.Server, Database = builder.Database };
            }
            catch (Exception ex)
            {
                return new { Name = name, Status = "ERROR", Server = (string?)null, Database = (string?)null, Error = ex.Message };
            }
        }
    }
}
