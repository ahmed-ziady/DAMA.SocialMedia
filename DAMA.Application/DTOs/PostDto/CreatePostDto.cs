using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DAMA.Application.DTOs.PostDto
{
    public record CreatePostDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces.")]
        [Display(Name = "Post Title")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters long.")]
        public string Title { get; set; } = default!;

        [Required]
        [Display(Name = "Post Content")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters long.")]
        [DataType(DataType.MultilineText)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?;:'""-]+$", ErrorMessage = "Content can only contain letters, numbers, spaces, and punctuation.")]
        public string Content { get; set; } = default!;

        public IFormFile? Image { get; set; }
        public IFormFile? Video { get; set; }
    }
}
