using DAMA.Application.DTOs.PostDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAMAWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostServicess post;
        private readonly UserManager<User> userManager;

        public PostController(IPostServicess post, UserManager<User> userManager)
        {
            this.post = post;
            this.userManager = userManager;
        }

        private int? CurrentUserId
        {
            get
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                    return null;

                if (int.TryParse(userIdString, out int userId))
                    return userId;

                return null;
            }
        }

        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePostAsync([FromForm] CreatePostDto createPostDto)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");

            if (createPostDto == null)
                return BadRequest("Create post data is required.");

            try
            {
                await post.CreatePostAsync(createPostDto, CurrentUserId.Value);
                return Ok("Post created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("deletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");

            if (id <= 0)
                return BadRequest("Post not found.");

            try
            {
                await post.DeletePostAsync(id);
                return Ok("Post deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getFriendPosts")]
        public async Task<IActionResult> GetFriendPosts(int page = 1, int pageSize = 50)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");

            try
            {
                var posts = await post.GetFriendPostsAsync(CurrentUserId.Value, page, pageSize);

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
