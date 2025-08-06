namespace TommyChat.Api.Services;

public class SignalRAuditLogger(ILogger<SignalRAuditLogger> logger) : ISignalRAuditLogger
{
    private readonly ILogger<SignalRAuditLogger> _logger = logger;

    public Task LogConnectedAsync(string userEmail, string connectionId, string ip)
    {
        _logger.LogInformation("🔌 Usuario conectado: {User} | ConnID: {Conn} | IP: {IP} | {Fecha}", userEmail, connectionId, ip, DateTime.UtcNow);
        return Task.CompletedTask;
    }

    public Task LogDisconnectedAsync(string userEmail, string connectionId, string ip)
    {
        _logger.LogInformation("❌ Usuario desconectado: {User} | ConnID: {Conn} | IP: {IP} | {Fecha}", userEmail, connectionId, ip, DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
