using TommyChat.Shared.DTOs;

namespace TommyChat.FrontEnd.Sevices
{
    public interface INotificationService
    {
        bool HasNewNotification { get; set; }
        UserDTO? NewContat { get; set; }

        event Action? OnChange;

        void NotifyNewContat(UserDTO newContat);
        void Clear();
    }
}