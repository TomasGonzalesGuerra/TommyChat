namespace TommyChat.Shared.DTOs
{
    public interface IUsersConnected
    {
        Task ReciveSystemMessage(string message);
        Task ReciveTestMessage(string message);
        Task UpdateUserList(List<UsersConnectedDTO> usersConnecteds);
        Task SendPrivateMessage(string userSender, string message);
    }
}
