using DAMA.Application.DTOs.PostDto;

namespace DAMA.Application.Interfaces
{
    public interface IPostServicess
    {
        public Task CreatePostAsync(CreatePostDto createPostDto, int id);
        public Task DeletePostAsync(int id);

    }
}
