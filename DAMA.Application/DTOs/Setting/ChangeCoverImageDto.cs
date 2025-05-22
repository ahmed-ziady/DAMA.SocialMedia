using Microsoft.AspNetCore.Http;

namespace DAMA.Application.DTOs.Setting
{
    public record ChangeCoverImageDto
    {
        public IFormFile CoverImage { get; set; } = default!;

    }
}