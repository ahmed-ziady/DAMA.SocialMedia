using DAMA.Application.DTOs.PostDto;
using DAMA.Domain.Enums;
namespace DAMA.Application.Interfaces
{
    public interface IPostServicess
    {
        public Task CreatePostAsync(CreatePostDto createPostDto, int id);
        Task<bool> DeletePostAsync(int postId, int userId);
        public Task<List<NewsFeedDto>> NewsFeed(int id, int page = 1, int pageSize = 50);

        public Task<List<UserPosts>> GetUserPostsAsync(int id, int page = 1, int pageSize = 50);

        public Task AddCommentAsync(string content, int postId, int userId);
        public Task<bool> DeleteCommentAsync(int commentId, int userId);
        public Task<ReactionAction> AddReactionAsync(string type, int postId, int userId);
    }
}
