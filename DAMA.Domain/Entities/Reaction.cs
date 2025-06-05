using System.ComponentModel.DataAnnotations;

namespace DAMA.Domain.Entities
{
    public class Reaction
    {

        [Key]
        public int ReactionId { get; set; }

        [Required]
        public string ReactionType { get; set; } = string.Empty;
        public int ReactionsNumber = 0;

        public int UserId { get; set; }
        public int PostId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public virtual User User { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
