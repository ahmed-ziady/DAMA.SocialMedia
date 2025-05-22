using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAMA.Domain.Entities
{
    public class Friendship
    {
        [Key]
        public int FriendshipId { get; set; }

        [Required]
        [ForeignKey("Requester")]
        public int RequesterId { get; set; }

        [Required]
        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User Requester { get; set; } = null!;
        public virtual User Receiver { get; set; } = null!;

    }
}