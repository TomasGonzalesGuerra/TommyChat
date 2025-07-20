using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TommyChat.Shared.DTOs;

namespace TommyChat.Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotifyHub : Hub<INotifyHub>
{
    private static readonly Dictionary<string, List<string>> _userConnections = [];

    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        
        if (email is not null)
        {
            lock (_userConnections)
            {
                if (!_userConnections.ContainsKey(email))
                    _userConnections[email] = [];

                _userConnections[email].Add(Context.ConnectionId);
            }

            await NotificarUsuariosConectados();
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (email is not null)
        {
            lock (_userConnections)
            {
                if (_userConnections.ContainsKey(email))
                {
                    _userConnections[email].Remove(Context.ConnectionId);
                    if (_userConnections[email].Count == 0)
                        _userConnections.Remove(email);
                }
            }

            await NotificarUsuariosConectados();
        }

        await base.OnDisconnectedAsync(exception);
    }

    private Task NotificarUsuariosConectados()
    {
        var usuarios = _userConnections.Keys.ToList();
        return Clients.All.UsuariosConectados(usuarios);
    }

    public async Task EnviarMensajePrivado(string destinatarioEmail, string mensaje)
    {
        var remitente = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anónimo";

        if (_userConnections.TryGetValue(destinatarioEmail, out var conexiones))
        {
            foreach (var connectionId in conexiones)
            {
                await Clients.Client(connectionId).RecibirMensajePrivado(remitente, mensaje);
            }
        }
    }
}