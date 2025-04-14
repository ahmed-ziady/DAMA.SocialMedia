using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAMA.Domain.Entities
{
    public class Friendship
    {
        public int FriendshipId { get; set; }

        public int RequesterId { get; set; }  // User who sent the request
        public int ReceiverId { get; set; }   // User who received the request

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? Requester { get; set; }  // Navigation to the sender
        public User? Receiver { get; set; }
    }
}
