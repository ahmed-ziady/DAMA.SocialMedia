using DAMA.Application.DTOs.FriendDtos;
using DAMA.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAMAWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController(IProfile _profile, IFriendsServices _friendsServices, IPostServicess postServicess) : ControllerBase
    {
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

        [HttpGet("basicInfo")]
        public async Task<IActionResult> GetProfile(int id)
        {
            if (CurrentUserId is null)
                return Unauthorized("User is not authenticated");

            var profile = await _profile.GetProfileAsync(id);
            return Ok(profile);
        }



        [HttpGet("isFriend/{friendId}")]
        public async Task<IActionResult> IsFriend(int friendId)
        {
            var isFriend = await _friendsServices.CheckIsFriend(CurrentUserId!.Value, friendId);


            return Ok(new { IsFriend = isFriend });
        }


        [HttpGet("friends")]
        public async Task<ActionResult<FriendsResponseDto>> GetFriends()
        {
            if (CurrentUserId is null)
                return Unauthorized("User is not authenticated");

            var userId = CurrentUserId.Value;

            var result = await _friendsServices.GetFriends(userId);

            return Ok(new
            {
                Message = "Friends retrieved successfully",
                result.TotalCount,
                Data = result.Friends
            });
        }


        [HttpGet("userPosts")]

        public async Task<IActionResult> GetUserPostsAsync(int UserId, int page = 1, int pageSize = 50)
        {

            try
            {
                var posts = await postServicess.GetUserPostsAsync(UserId, page, pageSize);

                if (posts == null || !posts.Any())
                    return Ok(new { posts = new List<object>() });

                return Ok(new { posts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
