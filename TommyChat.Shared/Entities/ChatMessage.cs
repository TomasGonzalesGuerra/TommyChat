namespace TommyChat.Shared.Entities;

public class ChatMessage
{
    public string Text      { get; init; } = "";
    public bool   IsMe      { get; init; }
    public string Time      { get; init; } = "";
}
