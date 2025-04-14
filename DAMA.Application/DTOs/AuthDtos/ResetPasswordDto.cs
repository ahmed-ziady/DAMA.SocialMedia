// RegisterDto.cs
namespace DAMA.Application.DTOs.AuthDtos
{

    // ResetPasswordDto.cs
    public class ResetPasswordDto
    {
        public string Email { get; set; } = default!;
        public string VerificationCode { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}