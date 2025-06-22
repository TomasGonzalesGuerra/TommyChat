namespace TommyChat.Shared.DTOs
{
    public interface IUsersConnected
    {
        Task ReciveSystemMessage(string message);
        Task UpdateUserList(List<UsersConnectedDTO> usersConnecteds);
    }
}
