using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task SendTicketUpdate(object ticketData)
        {
            await Clients.All.SendAsync("ReceiveTicketUpdate", ticketData);
        }

        public async Task JoinGroup(string groupName)
        {
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task LeaveGroup(string groupName)
        {
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
        }
    }
}
