using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.Setting
{
    public record ResetPasswordRequestDto
    {

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=]).{8,}$",
                 ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]

        public string NewPassword { get; set; } = default!;
        [Required(ErrorMessage = "Verification code is required")]
        [DataType(DataType.Text)]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be exactly 6 digits")]
        public string VerificationCode { get; set; } = default!;


    }
}