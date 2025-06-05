namespace DAMA.Domain.Entities
{
    public class Post
    {

        public int PostId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Reaction> Reactions { get; set; } = [];

    }
}
