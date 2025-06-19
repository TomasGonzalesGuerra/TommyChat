namespace TommyChat.Shared.Entities;

public class Message
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public Conversation? Conversation { get; set; }
    public int ConversationId { get; set; }

    public User? Sender { get; set; }
    public string? SenderId { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}