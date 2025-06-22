using Microsoft.AspNetCore.SignalR;
using TommyChat.Shared.DTOs;

namespace TommyChat.Api.Hubs;

public class NotifyHub : Hub<IUsersConnected>
{
    private static readonly List<UsersConnectedDTO> _usersConnecteds = [];
    private static readonly object _lock = new();

    public override async Task OnConnectedAsync()
    {
        string? UserSellerId = Context.GetHttpContext()?.Request.Query["userSellerId"];
        string? UserSellerName = Context.GetHttpContext()?.Request.Query["userSellerName"];

        Console.WriteLine($"Conectado: {Context.ConnectionId} - {UserSellerId} - {UserSellerName}");

        lock (_lock)
        {
            UsersConnectedDTO? userDisconnected = _usersConnecteds.FirstOrDefault(u => u.UserName == UserSellerName);
            if (userDisconnected != null) _usersConnecteds.Remove(userDisconnected);

            _usersConnecteds.Add(new()
            {
                UserId = UserSellerId,
                UserName = UserSellerName,
                ConnectionId = Context.ConnectionId
            });
        }

        await Clients.Caller.ReciveSystemMessage($"{Context.ConnectionId} - Te Conectaste - {UserSellerId} - {UserSellerName}");
        await Clients.All.UpdateUserList(_usersConnecteds);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        UsersConnectedDTO? userDisconnected = new();

        lock (_lock)
        {
            userDisconnected = _usersConnecteds.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (userDisconnected != null)
            {
                _usersConnecteds.Remove(userDisconnected);
            }
        }

        if (userDisconnected != null)
        {
            await Clients.All.UpdateUserList(_usersConnecteds);
        }

        await base.OnDisconnectedAsync(exception);
    }



}
