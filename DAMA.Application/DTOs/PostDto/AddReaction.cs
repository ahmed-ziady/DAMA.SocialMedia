namespace DAMA.Application.DTOs.PostDto
{
    public record AddReaction
    {
        public int Id { get; set; }
        public required string Type { get; set; }
    }
}
