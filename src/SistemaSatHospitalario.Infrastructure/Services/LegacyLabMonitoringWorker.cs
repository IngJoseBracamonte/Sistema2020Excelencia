using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
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
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public LegacyLabMonitoringWorker(
            IServiceProvider serviceProvider,
            ILogger<LegacyLabMonitoringWorker> logger,
            ILegacyErrorReportingService fileLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _fileLogger = fileLogger;
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
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Sistema2020LegacyDbContext>();
                var connection = context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(ct);
                
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                await command.ExecuteScalarAsync(ct);
                
                _logger.LogInformation("✅ Monitoreo Legacy: Conexión Exitosa a base de datos '{Database}'.", connection.Database);
                _fileLogger.LogTrace($"[MONITOR] [HEARTBEAT] OK - Conexión establecida exitosamente con el Sistema Legacy ({connection.Database}).");
            }
            catch (Exception ex)
            {
                _fileLogger.LogError($"[MONITOR] [FALLO] No se pudo establecer conexión con el Sistema Legacy.", ex);
            }
        }
    }
}
