namespace TommyChat.FrontEnd.Models;

public class ChatMessage
{
    public string Text { get; init; } = "";
    public bool IsMe { get; init; }
    public string Time { get; init; } = "";
}
