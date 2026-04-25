using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Hubs
{
    public class TasaHub : Hub
    {
        public async Task BroadcastTasaUpdate(decimal nuevaTasa)
        {
            await Clients.All.SendAsync("TasaActualizada", nuevaTasa);
        }
    }
}
