// RegisterDto.cs
namespace DAMA.Application.DTOs.AuthDtos
{
    // VerifyEmailDto.cs
    public class VerifyEmailDto
    {
        public string Email { get; set; } = default!;
        public string Code { get; set; } = default!;
    }
}