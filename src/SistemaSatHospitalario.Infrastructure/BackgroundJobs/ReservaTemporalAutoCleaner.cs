using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.BackgroundJobs
{
    public class ReservaTemporalAutoCleaner : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaTemporalAutoCleaner> _logger;

        public ReservaTemporalAutoCleaner(IServiceProvider serviceProvider, ILogger<ReservaTemporalAutoCleaner> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monitor de Limpieza de Reservas Temporales iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                        
                        // Definir el umbral de limpieza (reservas expiradas)
                        var utcNow = DateTime.UtcNow;

                        var expiredReservations = await context.ReservasTemporales
                            .Where(r => r.ExpiracionUtc < utcNow)
                            .ToListAsync(stoppingToken);

                        if (expiredReservations.Any())
                        {
                            _logger.LogInformation("Limpiando {Count} reservas temporales expiradas.", expiredReservations.Count);
                            context.ReservasTemporales.RemoveRange(expiredReservations);
                            await context.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la limpieza automática de reservas.");
                }

                // Esperar 30 minutos antes de la siguiente ejecución
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}
