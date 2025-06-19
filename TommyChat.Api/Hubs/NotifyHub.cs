using Microsoft.AspNetCore.SignalR;

namespace TommyChat.Api.Hubs
{
    public class NotifyHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - ConnectedAsync");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - DisconnectedAsync - {exception}");
        }
    }
}
