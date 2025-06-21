using Microsoft.AspNetCore.SignalR;

namespace TommyChat.Api.Hubs;

public class NotifyHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - Te Conectaste {Context.UserIdentifier}");
        Console.WriteLine($"{Context.UserIdentifier}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("AllClientsNotify", $"{Context.ConnectionId} - Arrugaste - {exception}");

    }


    public async Task SendPrivateMessage(string senderUserId, string receiverUserId, string message)
    {
        //var mySenderUserId = Context.UserIdentifier;
        //if (mySenderUserId != senderUserId)
        //{
        //    throw new HubException("You are not authorized to send this message.");
        //}
        try
        {
            await Clients.User(receiverUserId).SendAsync("SendPrivateMessage", senderUserId, message);
        }
        catch (Exception en)
        {
            Console.WriteLine(en);
        }
    }



}
