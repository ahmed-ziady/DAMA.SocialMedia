namespace DAMA.Application.DTOs.AuthDtos
{
    public record LoginResponseDto
    {
        
        public string Token { get; set; } = default!;
       
        public int UserId { get; set; }
    }
}
