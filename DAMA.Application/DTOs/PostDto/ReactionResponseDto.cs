namespace DAMA.Application.DTOs.PostDto
{
    public record ReactionResponseDto
    {
        public string ReactionType { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

    }
}


