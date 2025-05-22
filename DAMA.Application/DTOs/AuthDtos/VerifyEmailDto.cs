using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.AuthDtos
{
    
    public record VerifyEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [MinLength(5, ErrorMessage = "Email must be at least 5 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Verification code is required")]
        [Compare("Code", ErrorMessage = "Verification code do not match")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be exactly 6 digits")]
        public string Code { get; set; } = default!;
    }
}