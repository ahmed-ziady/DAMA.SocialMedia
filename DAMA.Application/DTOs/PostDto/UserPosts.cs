namespace DAMA.Application.DTOs.PostDto
{
    public record UserPosts
    {
  
        public int PostId { get; set; }
        public string PostTitle { get; set; } = String.Empty;
        public string PostBody { get; set; } = String.Empty;
        public string? MediaUrl { get; set; }

        public int TotalComments { get; set; }
        public int TotalReactions { get; set; }
        public List<CommentResponseDto> Comments { get; set; } = [];
        public List<ReactionResponseDto> Reactions { get; set; } = [];

    }
}
