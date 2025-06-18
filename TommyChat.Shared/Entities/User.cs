using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TommyChat.Shared.Enums;

namespace TommyChat.Shared.Entities
{
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


        public ICollection<FriendShip>? FriendShips { get; set; }
        public ICollection<GroupMember>? Groups { get; set; }
        public ICollection<FriendRequest>? SentRequests { get; set; }
        public ICollection<FriendRequest>? ReceivedRequests { get; set; }
    }
}
