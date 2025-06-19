using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TommyChat.Shared.Enums;

namespace TommyChat.Shared.Entities;

public class User : IdentityUser
{
    [Display(Name = "Nombre")]
    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string? FullName { get; set; }

    [Display(Name = "Foto")]
    public string? Photo { get; set; }

    [Display(Name = "Tipo de usuario")]
    public UserType UserType { get; set; }

    // Conversaciones donde el usuario es uno de los participantes
    public ICollection<Conversation>? ConversationsAsParticipantA { get; set; }
    public ICollection<Conversation>? ConversationsAsParticipantB { get; set; }

    // Mensajes enviados por este usuario
    public ICollection<Message>? MessagesSent { get; set; }

    [NotMapped]
    public IEnumerable<Conversation> AllConversations =>
        (ConversationsAsParticipantA ?? Enumerable.Empty<Conversation>())
            .Concat(ConversationsAsParticipantB ?? Enumerable.Empty<Conversation>());
}