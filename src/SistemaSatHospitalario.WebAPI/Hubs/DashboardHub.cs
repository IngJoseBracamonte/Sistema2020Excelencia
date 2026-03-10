using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task SendTicketUpdate(object ticketData)
        {
            await Clients.All.SendAsync("ReceiveTicketUpdate", ticketData);
        }
    }
}
