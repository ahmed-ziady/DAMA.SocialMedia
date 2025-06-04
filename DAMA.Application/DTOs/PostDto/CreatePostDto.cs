using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public record CreatePostDto
{
    [Required]
    [Display(Name = "Post Title")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters long.")]
    [RegularExpression(@"^[\u0600-\u06FFa-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain Arabic/English letters, numbers, and spaces.")]
    public string Title { get; set; } = default!;

    [Required]
    [Display(Name = "Post Content")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters long.")]
    [DataType(DataType.MultilineText)]
    [RegularExpression(@"^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFFa-zA-Z0-9\s.,!?;:'""-]+$", ErrorMessage = "Content can only contain Arabic/English letters, numbers, spaces, and punctuation.")]
    public string Content { get; set; } = default!;

    public IFormFile? Image { get; set; }
    public IFormFile? Video { get; set; }
}
