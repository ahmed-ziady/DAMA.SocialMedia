using DAMA.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DAMAWebApi.Controllers
{
    [Route("api/friends")]
    [ApiController]
    [Authorize]
    public class FriendController(IFriendService friendService) : ControllerBase
    {
        private readonly IFriendService _friendService = friendService;

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("nameid")?.Value;
            return int.TryParse(userIdClaim, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Invalid token.");
        }

        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequest([FromQuery] int receiverId)
        {
            int senderId = GetCurrentUserId();
            var success = await _friendService.SendFriendRequest(senderId, receiverId);
            return success ? Ok("Friend request sent.") : BadRequest("Request already sent or invalid.");
        }

        [HttpPost("accept-request")]
        public async Task<IActionResult> AcceptFriendRequest([FromQuery] int requestId)
        {
            int userId = GetCurrentUserId();
            var success = await _friendService.AcceptFriendRequest(requestId, userId);
            return success ? Ok("Friend request accepted.") : NotFound("Request not found or unauthorized.");
        }

        [HttpPost("reject-request")]
        public async Task<IActionResult> RejectFriendRequest([FromQuery] int requestId)
        {
            int userId = GetCurrentUserId();
            var success = await _friendService.RejectFriendRequest(requestId, userId);
            return success ? Ok("Friend request rejected.") : NotFound("Request not found or unauthorized.");
        }

        [HttpDelete("remove-friend")]
        public async Task<IActionResult> RemoveFriend([FromQuery] int friendId)
        {
            int userId = GetCurrentUserId();
            var success = await _friendService.RemoveFriend(userId, friendId);
            return success ? Ok("Friend removed.") : NotFound("Friendship not found.");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFriendsList()
        {
            int userId = GetCurrentUserId();
            var friends = await _friendService.GetFriendsList(userId);
            return Ok(friends);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            int userId = GetCurrentUserId();
            var requests = await _friendService.GetFriendRequests(userId);
            return Ok(requests);
        }
    }
}
