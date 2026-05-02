using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TommyChat.Api.Services;

namespace TommyChat.Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PresenceHub : Hub
{
    private readonly IUserPresenceService _presence;

    public PresenceHub(IUserPresenceService presence)
    {
        _presence = presence;
    }

    // ── Se dispara cuando un cliente se conecta ──────────────────────────
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;        // claim sub / nameidentifier
        var userName = Context.User?.Identity?.Name ?? userId;

        // Registra la conexión en el servicio en memoria
        _presence.UserConnected(userId, userName, Context.ConnectionId);

        // Avisa a TODOS los demás que este usuario está online
        await Clients.Others.SendAsync("UserOnline", userId, userName);

        // Envía al que acaba de conectarse la lista completa de online
        var onlineUsers = _presence.GetOnlineUsers();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);

        await base.OnConnectedAsync();
    }

    // ── Se dispara cuando un cliente se desconecta ───────────────────────
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;

        _presence.UserDisconnected(Context.ConnectionId);

        // Avisa a todos que se fue
        await Clients.Others.SendAsync("UserOffline", userId);

        await base.OnDisconnectedAsync(exception);
    }

    // ── Enviar mensaje de chat ───────────────────────────────────────────
    // El cliente llama: hubConnection.InvokeAsync("SendMessage", toUserId, text)
    public async Task SendMessage(string toUserId, string text)
    {
        var fromUserId = Context.UserIdentifier!;
        var fromName = Context.User?.Identity?.Name ?? fromUserId;
        var timestamp = DateTime.UtcNow;

        // Solo lo recibe el destinatario (y el propio emisor para confirmar)
        await Clients.User(toUserId).SendAsync("ReceiveMessage", new
        {
            FromId = fromUserId,
            FromName = fromName,
            Text = text,
            Timestamp = timestamp
        });

        // Echo al emisor para que vea el mensaje en su propia ventana
        await Clients.Caller.SendAsync("MessageSent", new
        {
            ToId = toUserId,
            Text = text,
            Timestamp = timestamp
        });
    }
}