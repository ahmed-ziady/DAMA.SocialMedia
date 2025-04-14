public interface IPostService
{
    Task<PostDto> AddPost(int userId, CreatePostDto postDto);
    Task<List<PostDto>> GetAllPosts();
}
