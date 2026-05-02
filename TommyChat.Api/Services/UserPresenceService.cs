namespace TommyChat.Api.Services;

public class UserPresenceService : IUserPresenceService
{
// connectionId → (userId, userName)
// Un usuario puede tener varias pestañas abiertas → múltiples connectionIds
private readonly Dictionary<string, (string UserId, string UserName)> _connections = new();
private readonly object _lock = new();

public void UserConnected(string userId, string userName, string connectionId)
{
    lock (_lock)
    {
        _connections[connectionId] = (userId, userName);
    }
}

public void UserDisconnected(string connectionId)
{
    lock (_lock)
    {
        _connections.Remove(connectionId);
    }
}

public IReadOnlyList<OnlineUser> GetOnlineUsers()
{
    lock (_lock)
    {
        // Agrupa por userId para no duplicar si hay varias pestañas
        return _connections.Values
            .DistinctBy(c => c.UserId)
            .Select(c => new OnlineUser(c.UserId, c.UserName))
            .ToList();
    }
}

public bool IsOnline(string userId)
{
    lock (_lock)
    {
        return _connections.Values.Any(c => c.UserId == userId);
    }
}
}