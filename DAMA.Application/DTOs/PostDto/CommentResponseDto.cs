namespace DAMA.Application.DTOs.PostDto
{
    public record CommentResponseDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

    }
}
