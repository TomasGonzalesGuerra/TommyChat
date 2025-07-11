using Microsoft.AspNetCore.SignalR;
using TommyChat.Api.Hubs;

namespace TommyChat.Api.Services;

public class NotifyHubService(IHubContext<NotifyHub> hubContext) : INotifyHub
{
    private readonly IHubContext<NotifyHub> _hubContext = hubContext;


    public Task RecibirMensajePrivado(string destinatarioEmail, string mensaje)
    {
        return _hubContext.Clients.User(destinatarioEmail).SendAsync("RecibirMensajePrivado", "Sistema", mensaje);
    }

    public Task UsuariosConectados(List<string> usuarios)
    {
        return _hubContext.Clients.All.SendAsync("UsuariosConectados");
    }
}