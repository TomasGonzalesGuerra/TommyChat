namespace TommyChat.Api.Services;

public interface ISignalRAuditLogger
{
    Task LogConnectedAsync(string userEmail, string connectionId, string ip);
    Task LogDisconnectedAsync(string userEmail, string connectionId, string ip);
}
