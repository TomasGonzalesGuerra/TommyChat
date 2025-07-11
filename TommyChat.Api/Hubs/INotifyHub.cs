namespace TommyChat.Api.Hubs;

public interface INotifyHub
{
    Task UsuariosConectados(List<string> usuarios);
    Task RecibirMensajePrivado(string destinatarioEmail, string mensaje);
}