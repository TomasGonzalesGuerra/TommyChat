namespace TommyChat.Api.Services;

public interface IUserPresenceService
{
    void UserConnected(string userId, string userName, string connectionId);
    void UserDisconnected(string connectionId);
    IReadOnlyList<OnlineUser> GetOnlineUsers();
    bool IsOnline(string userId);
}

public record OnlineUser(string UserId, string UserName);