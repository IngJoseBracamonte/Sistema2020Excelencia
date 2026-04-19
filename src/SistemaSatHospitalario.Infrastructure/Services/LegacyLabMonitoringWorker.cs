using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    /// <summary>
    /// [PHASE-7] Proactive Automation: Background worker that ensures legacy system health.
    /// This alerts infrastructure failures BEFORE a patient attempts billing.
    /// </summary>
    public class LegacyLabMonitoringWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LegacyLabMonitoringWorker> _logger;
        private readonly ILegacyErrorReportingService _fileLogger;
        private readonly string _connectionString;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public LegacyLabMonitoringWorker(
            IServiceProvider serviceProvider,
            ILogger<LegacyLabMonitoringWorker> logger,
            IConfiguration configuration,
            ILegacyErrorReportingService fileLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _fileLogger = fileLogger;

            var rawConnStr = configuration.GetConnectionString("LegacyConnection") ?? "";
            // Senior Logic: Use case-safe normalization for external legacy DBs
            _connectionString = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConnStr, forceLowercase: false);
            _connectionString = ConnectionStringHelper.EnhanceForCloud(_connectionString);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _fileLogger.LogTrace("[MONITOR] Iniciando Servicio de Monitoreo Proactivo de Laboratorio.");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckConnectivityAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado en el ciclo de monitoreo legacy.");
                    _fileLogger.LogError("[MONITOR] Error crítico en el ciclo de monitoreo.", ex);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckConnectivityAsync(CancellationToken ct)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _fileLogger.LogError("[MONITOR] ABORTADO: No hay cadena de conexión configurada.");
                return;
            }

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(ct);
                
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync(ct);
                
                _logger.LogInformation("✅ Monitoreo Legacy: Conexión Exitosa a base de datos '{Database}'.", connection.Database);
                _fileLogger.LogTrace($"[MONITOR] [HEARTBEAT] OK - Conexión establecida exitosamente con el Sistema Legacy ({connection.Database}).");
            }
            catch (MySqlException sqlEx) when (sqlEx.Number == 1049 || sqlEx.Message.Contains("Unknown database"))
            {
                 _fileLogger.LogError($"[MONITOR] [INFRA-FALLO] La base de datos no existe. Verifique el nombre en la configuración.", sqlEx);
            }
            catch (Exception ex)
            {
                _fileLogger.LogError($"[MONITOR] [FALLO] No se pudo establecer conexión con el Sistema Legacy.", ex);
            }
        }
    }
}
