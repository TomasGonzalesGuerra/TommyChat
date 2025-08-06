using Microsoft.AspNetCore.SignalR;
using TommyChat.Api.Hubs;

namespace TommyChat.Api.Services;

public class NotifyHubService(IHubContext<NotifyHub> hubContext) : INotifyHub
{
    private readonly IHubContext<NotifyHub> _hubContext = hubContext;


    public async Task RecibirMensajePrivado(string destinatarioEmail, string mensaje)
    {
        await _hubContext.Clients.User(destinatarioEmail).SendAsync(nameof(INotifyHub.RecibirMensajePrivado), "Sistema", mensaje);
    }

    public Task UsuariosConectados(List<string> usuarios)
    {
        return _hubContext.Clients.All.SendAsync("UsuariosConectados", usuarios);
    }
}