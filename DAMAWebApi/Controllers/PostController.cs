using DAMA.Application.DTOs.PostDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAMAWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(IPostServicess post, UserManager<User> userManager) : ControllerBase
    {
        private int CurrentUserId => int.Parse(userManager.GetUserId(User)!);

        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePostAsync([FromForm] CreatePostDto createPostDto)
        {
            if (createPostDto == null)
            {
                return BadRequest("Create post data is required.");
            }

            try
            {
                await post.CreatePostAsync(createPostDto, CurrentUserId);
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
            if (id <= 0)
                return BadRequest("Post Not Found");

            await post.DeletePostAsync(id);
            return Ok("Post deleted successfully.");
        }
    }
}
