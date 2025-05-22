using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.AuthDtos
{
    public record RegisterDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "الاسم الاول يجب ان يحتوي يكتون من حرفين الي خمسون")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")]
        public string FirstName { get; set; } = default!;

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces.")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(12, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]

        public DateOnly DateOfBirth { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public IFormFile? CoverImage { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        [RegularExpression(@"^\+?[0-9\s]+$", ErrorMessage = "Phone number can only contain numbers and spaces.")]
        public string? PhoneNumber { get; set; } = default!;
    }
}