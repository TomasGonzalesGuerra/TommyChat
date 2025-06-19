using Microsoft.AspNetCore.SignalR;

namespace TommyChat.Api.Hubs;

public class NotifyHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - Te Conectaste");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - Arrugaste - {exception}");
    }

    public async Task SendMessageToUserAsync(string receiverUserId, string message)
    {
        string? senderId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(senderId))
        {
            await Clients.User(receiverUserId).SendAsync("SendMessageToUserAsync", senderId, message);
        }
    }


}
