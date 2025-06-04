namespace DAMA.Application.DTOs.PostDto
{
    public record NewsFeedDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = String.Empty;
        public string PostBody { get; set; } = String.Empty;
        public string? MediaUrl { get; set; }

    }
}
