namespace DAMA.Application.DTOs.PostDto
{
    public record AddComment
    {
        public int Id { get; set; }
        public required string Comment { get; set; }
    }
}
