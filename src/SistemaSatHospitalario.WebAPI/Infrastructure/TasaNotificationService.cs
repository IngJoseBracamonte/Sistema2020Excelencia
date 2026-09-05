using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Infrastructure.Hubs;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Infrastructure
{
    public class TasaNotificationService : ITasaNotificationService
    {
        private readonly IHubContext<TasaHub> _hubContext;

        public TasaNotificationService(IHubContext<TasaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTasaUpdatedAsync(decimal nuevaTasa, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.All.SendAsync("TasaActualizada", nuevaTasa, cancellationToken);
        }
    }
}
