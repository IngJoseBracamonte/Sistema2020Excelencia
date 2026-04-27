using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.WebAPI.Infrastructure
{
    public class LegacyOrderStatusWorker : BackgroundService
    {
        private readonly ILogger<LegacyOrderStatusWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(30);

        public LegacyOrderStatusWorker(
            ILogger<LegacyOrderStatusWorker> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[V16.3] Legacy Order Status Worker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PollLegacyStatus(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el ciclo de monitoreo de órdenes legacy.");
                }

                await Task.Delay(_pollInterval, stoppingToken);
            }
        }

        private async Task PollLegacyStatus(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var legacyRepo = scope.ServiceProvider.GetRequiredService<ILegacyLabRepository>();
            var notification = scope.ServiceProvider.GetRequiredService<INotificationService>();

            // 1. Buscar cuentas pendientes de procesamiento del DÍA ACTUAL (V16.3 Optimization)
            var today = DateTime.Today;
            var pendingAccounts = await context.CuentasServicios
                .Where(c => c.LegacyOrderId.HasValue 
                       && c.ProcesamientoEstado == EstadoConstants.ProcesamientoPendiente
                       && c.FechaCarga >= today)
                .ToListAsync(ct);

            if (!pendingAccounts.Any()) return;

            _logger.LogTrace($"[V16.3] Chequeando {pendingAccounts.Count} órdenes pendientes en Legacy...");

            foreach (var account in pendingAccounts)
            {
                var status = await legacyRepo.GetMuestraStatusAsync(account.LegacyOrderId!.Value, ct);
                
                // Si Muestra es 1, significa que ya fue procesada/tomada
                if (status == 1)
                {
                    _logger.LogInformation($"[V16.3] Orden {account.LegacyOrderId} detectada como PROCESADA en Legacy. Actualizando cuenta {account.Id}");
                    
                    account.ActualizarProcesamiento(EstadoConstants.ProcesamientoProcesada);
                    
                    // Notificar via SignalR
                    await notification.SendNotificationToGroupAsync(
                        "ProcessingOrders", 
                        "Orden Procesada", 
                        $"La orden #{account.LegacyOrderId} ha sido procesada correctamente.",
                        "Laboratory",
                        new { CuentaId = account.Id, LegacyOrderId = account.LegacyOrderId, Status = "PROCESADA" },
                        ct);
                }
            }

            await context.SaveChangesAsync(ct);
        }
    }
}
