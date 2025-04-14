using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;

public class PostService : IPostService
{
    private readonly DamaContext _context;

    public PostService(DamaContext context)
    {
        _context = context;
    }

    public async Task<PostDto> AddPost(int userId, CreatePostDto postDto)
    {
        var post = new Post
        {
            Id = userId,
            PostContent = postDto.PostContent,
            PostTypeId = postDto.PostTypeId,
            PostDate = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return new PostDto
        {
            PostId = post.PostId,
            PostContent = post.PostContent,
            PostTypeId = post.PostTypeId,
            PostDate = post.PostDate ?? DateTime.UtcNow
        };
    }

    public async Task<List<PostDto>> GetAllPosts()
    {
        return await _context.Posts
            .OrderByDescending(p => p.PostDate)
            .Select(p => new PostDto
            {
                PostId = p.PostId,
                PostContent = p.PostContent,
                PostTypeId = p.PostTypeId,
                PostDate = p.PostDate ?? DateTime.UtcNow
            }).ToListAsync();
    }
}
