using Microsoft.AspNetCore.SignalR.Client;

namespace TommyChat.FrontEnd.Services;

public class NotifyService()
{
    //public event Action? ConnectionStateChanged;
    //public string? ConnectionState = string.Empty;

    //public async Task StartConnectionAsync()
    //{        
    //    if (_hubConnection.State != HubConnectionState.Connected)
    //    {
    //        await _hubConnection.StartAsync();
    //    }

    //    GetConnectionState();
    //}

    //public async Task CloseConnectionAsync()
    //{
    //    if (_hubConnection.State == HubConnectionState.Connected)
    //    {
    //        await _hubConnection.StopAsync();
    //    }

    //    GetConnectionState();
    //}

    //public void GetConnectionState()
    //{
    //    switch (_hubConnection.State)
    //    {
    //        case HubConnectionState.Connected:
    //            Invoke("Connected");
    //            break;

    //        case HubConnectionState.Disconnected:
    //            Invoke("Disconnected");
    //            break;

    //        case HubConnectionState.Connecting:
    //            Invoke("Connecting");
    //            break;

    //        case HubConnectionState.Reconnecting:
    //            Invoke("Reconnecting");
    //            break;

    //        default:
    //            ConnectionState = "Unknown State";
    //            break;
    //    }

    //    void Invoke(string message)
    //    {
    //        ConnectionState = message;
    //        ConnectionStateChanged?.Invoke();
    //    }
    //}





}
