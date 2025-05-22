// RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.AuthDtos
{
    // LoginDto.cs
    public record LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [MinLength(5, ErrorMessage = "Email must be at least 5 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]


        public string Password { get; set; } = default!;
    }
}