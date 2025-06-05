using DAMA.Application.DTOs.PostDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAMAWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

            try
            {
                var isDeleted = await post.DeletePostAsync(id, CurrentUserId.Value);
                if (isDeleted)
                    return Ok("Post deleted successfully.");

                return NotFound("Post not found. It may have been deleted or you do not have permission to delete it.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("comment")]
        public async Task<IActionResult> AddCommentAsync(AddComment addComment)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");
            if (addComment == null || string.IsNullOrWhiteSpace(addComment.Comment) || addComment.Id <= 0)
                return BadRequest("Invalid comment data.");
            try
            {
                await post.AddCommentAsync(addComment.Comment, addComment.Id, CurrentUserId.Value);
                return Ok("Comment added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("deleteComment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");

            try
            {
                var isDeleted = await post.DeletePostAsync(id, CurrentUserId.Value);
                if (isDeleted)
                    return Ok("Comment deleted successfully.");

                return NotFound("Post not found. It may have been deleted or you do not have permission to delete it.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("reaction")]
        public async Task<IActionResult> AddReactionAsync(AddReaction addReaction)
        {
            if (CurrentUserId == null)
                return Unauthorized(new { message = "Unauthorized user." });

            if (addReaction == null || string.IsNullOrWhiteSpace(addReaction.Type) || addReaction.Id <= 0)
                return BadRequest(new { message = "Invalid reaction data." });

            try
            {
                var result = await post.AddReactionAsync(addReaction.Type, addReaction.Id, CurrentUserId.Value);

                return result switch
                {
                    ReactionAction.Added => Ok(new { message = "Reaction added successfully." }),
                    ReactionAction.Updated => Ok(new { message = "Reaction updated successfully." }),
                    ReactionAction.Removed => Ok(new { message = "Reaction removed successfully." }),
                    _ => BadRequest(new { message = "No reaction performed." }),
                };
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpGet("NewsFeed")]
        public async Task<IActionResult> GetNewsFeed(int page = 1, int pageSize = 50)
        {
            if (CurrentUserId == null)
                return Unauthorized("Unauthorized user");

            try
            {
                var posts = await post.NewsFeed(CurrentUserId.Value, page, pageSize);

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
