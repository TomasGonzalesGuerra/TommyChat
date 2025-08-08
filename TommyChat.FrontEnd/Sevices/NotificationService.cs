using TommyChat.Shared.DTOs;

namespace TommyChat.FrontEnd.Sevices;

public class NotificationService : INotificationService
{
    public bool HasNewNotification { get; set; }
    public UserDTO? NewContat { get; set; }

    public event Action? OnChange;


    public void NotifyNewContat(UserDTO newContat)
    {
        HasNewNotification = true;
        NewContat = newContat;
        NotifyStateChange();
    }

    public void Clear()
    {
        HasNewNotification = false;
        NewContat = null;
        NotifyStateChange();
    }

    private void NotifyStateChange() => OnChange?.Invoke();
}
