// RegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.AuthDtos
{
    public record LogoutDto
    {
        [Required(ErrorMessage = "User ID is required")]

        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "User ID must be a number")]
        public int UserId { get; set; }
    }
}