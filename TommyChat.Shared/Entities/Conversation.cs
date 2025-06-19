namespace TommyChat.Shared.Entities;

public class Conversation
{
    public int Id { get; set; }

    public User? ParticipantA { get; set; }
    public string? ParticipantAId { get; set; }

    public User? ParticipantB { get; set; }
    public string? ParticipantBId { get; set; }

    public ICollection<Message>? Messages { get; set; }
}
