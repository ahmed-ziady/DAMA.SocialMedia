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
    public class ProfileController(IProfile _profile, IFriendsServices _friendsServices) : ControllerBase
    {
        private int? CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

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
            if (CurrentUserId is null)
                return Unauthorized("User is not authenticated");
            var isFriend = await _friendsServices.CheckIsFriend(CurrentUserId.Value, friendId);
            if (isFriend == false)
                return NotFound(new { IsFriend = isFriend });
            return Ok(new { IsFriend = isFriend });
        }


        [HttpGet("friends")]
        public async Task<ActionResult<FriendsResponseDto>> GetFriends()
        {
            var result = await _friendsServices.GetFriends(CurrentUserId.Value);
            return Ok(new
            {
                Message = "Friends retrieved successfully",
                result.TotalCount,
                Data = result.Friends
            });
        }
    }
}
