namespace TommyChat.Shared.Entities;

public class ChatSession
{
    public Peer Peer { get; init; } = default!;
    public List<ChatMessage>? Messages { get; } = new();
    public bool IsMinimized { get; set; }

    /// Cantidad de mensajes que el usuario aún no leyó
    public int UnreadCount { get; set; }

    /// true mientras el compañero "está escribiendo"
    public bool IsTyping   { get; set; }
}
