using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TommyChat.Shared.DTOs;

namespace TommyChat.Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotifyHub : Hub<IUsersConnected>
{
    private static readonly List<UsersConnectedDTO> _usersConnecteds = [];
    private static readonly object _lock = new();


    public override async Task OnConnectedAsync()
    {
        var isAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false;
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = Context.User?.Identity?.Name;
        var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anónimo";
        Console.WriteLine($"Email del usuario: {email}");
        Console.WriteLine($"¿Autenticado?: {isAuthenticated}");

        lock (_lock)
        {
            UsersConnectedDTO? userDisconnected = _usersConnecteds.FirstOrDefault(u => u.UserName == userName);
            if (userDisconnected != null) _usersConnecteds.Remove(userDisconnected);

            _usersConnecteds.Add(new()
            {
                UserId = userId,
                UserName = userName,
                UserEmail = Context.GetHttpContext()?.User?.Identity?.Name,
                UserPhoto = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == "Photo")?.Value,
                ConnectionId = Context.ConnectionId
            });
        }

        await TestPrivateMessage(userName, $"{Context.ConnectionId} - Te Conectaste - {userName} - {userId}");
        await Clients.Caller.ReciveSystemMessage($"{Context.ConnectionId} - Te Conectaste - {userName} - {userId}");
        await Clients.All.UpdateUserList(_usersConnecteds);
        await base.OnConnectedAsync();
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

    public async Task SendPrivateMessage(string userReceiverId, string message)
    {
        var senderName = Context.User?.Identity?.Name ?? "Anónimo";

        if (string.IsNullOrEmpty(userReceiverId) || string.IsNullOrEmpty(message)) return;

        // TEMPORAL: mensaje al remitente para saber si llega aquí
        await Clients.Caller.ReciveSystemMessage($"Intentando enviar a {userReceiverId}: {message}");
        await Clients.All.SendPrivateMessage(senderName, message);
    }


    public async Task TestPrivateMessage(string userReceiverId, string message)
    {
        if (string.IsNullOrEmpty(userReceiverId) || string.IsNullOrEmpty(message)) return;

        // TEMPORAL: mensaje al remitente para saber si llega aquí
        await Clients.Caller.ReciveTestMessage($"Esto es desde TestPrivateMessage - userReceiverId: {userReceiverId} -> {message}");
    }
}
