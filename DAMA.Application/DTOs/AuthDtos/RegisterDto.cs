// RegisterDto.cs
namespace DAMA.Application.DTOs.AuthDtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? PortfolioImageUrl { get; set; }
    }
}