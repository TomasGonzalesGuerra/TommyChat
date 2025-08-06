using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using TommyChat.Api.Services;

namespace TommyChat.Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotifyHub(ISignalRAuditLogger auditLogger) : Hub<INotifyHub>
{
    private readonly ISignalRAuditLogger _auditLogger = auditLogger;
    private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (email is not null)
        {
            var conexiones = _userConnections.GetOrAdd(email, _ => []);
            lock (conexiones)
            {
                conexiones.Add(Context.ConnectionId);
            }

            await NotificarUsuariosConectados();
        }

        await base.OnConnectedAsync();
        await _auditLogger.LogConnectedAsync(email!, Context.ConnectionId, ObtenerIp());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (email is not null && _userConnections.TryGetValue(email, out var conexiones))
        {
            lock (conexiones)
            {
                conexiones.Remove(Context.ConnectionId);
                if (conexiones.Count == 0)
                {
                    _userConnections.TryRemove(email, out _);
                }
            }

            await NotificarUsuariosConectados();
        }

        await base.OnDisconnectedAsync(exception);
        await _auditLogger.LogDisconnectedAsync(email!, Context.ConnectionId, ObtenerIp());
    }

    private Task NotificarUsuariosConectados()
    {
        var usuarios = _userConnections.Keys.ToList();
        return Clients.All.UsuariosConectados(usuarios);
    }

    public async Task EnviarMensajePrivado(string destinatarioEmail, string mensaje)
    {
        var remitente = (Context.User?.FindFirst(ClaimTypes.Name)?.Value) ?? throw new HubException("Usuario no autenticado");

        if (_userConnections.TryGetValue(destinatarioEmail, out var conexiones))
        {
            foreach (var connectionId in conexiones)
            {
                await Clients.Client(connectionId).RecibirMensajePrivado(remitente, mensaje);
            }
        }
    }

    private string ObtenerIp()
    {
        var httpContext = Context.GetHttpContext();
        return httpContext?.Connection.RemoteIpAddress?.ToString() ?? "IP no disponible";
    }
}