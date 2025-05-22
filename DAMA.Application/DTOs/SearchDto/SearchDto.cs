using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.SearchDto
{
    public record SearchDto
    {
        [Required(ErrorMessage = "Search term is required")]

        [MinLength(3, ErrorMessage = "Search term must be at least 3 characters")]
        [MaxLength(50, ErrorMessage = "Search term cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s@._+-]+$",
             ErrorMessage = "Search term can contain letters, numbers, spaces, and email symbols (@ . _ + -)")]
        public string SearchTerm { get; set; } = null!;


    }
}

