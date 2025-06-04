using DAMA.Application.DTOs.PostDto;

namespace DAMA.Application.Interfaces
{
    public interface IPostServicess
    {
        public Task CreatePostAsync(CreatePostDto createPostDto, int id);
        Task<bool> DeletePostAsync(int postId);
        public Task<List<NewsFeedDto>> GetFriendPostsAsync(int id, int page = 1, int pageSize = 50);

        public Task<List<NewsFeedDto>> GetUserPostsAsync(int id, int page = 1, int pageSize = 50);
    }
}
