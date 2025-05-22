using Microsoft.AspNetCore.Http;

namespace DAMA.Application.DTOs.Setting
{
    public record ChangeProfileImageeDto
    {


        public IFormFile ProfileImage { get; set; } = default!;


    }
}