using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public record RegisterDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(12, MinimumLength = 3, ErrorMessage = "الاسم الأول يجب أن يكون بين 3 و 12 حرفًا.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "الاسم الأول يمكن أن يحتوي فقط على حروف عربية أو إنجليزية ومسافات.")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "الاسم الأخير يجب أن يكون بين 2 و 50 حرفًا.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z\s]+$", ErrorMessage = "الاسم الأخير يمكن أن يحتوي فقط على حروف عربية أو إنجليزية ومسافات.")]
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
