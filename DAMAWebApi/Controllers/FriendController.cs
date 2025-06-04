using DAMA.Application.DTOs.FriendDtos;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAMAWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendsController(UserManager<User> _userManager, IFriendsServices _friendService) : ControllerBase
    {


        private int CurrentUserId => int.Parse(_userManager.GetUserId(User)!);

        [HttpPost("sendFriendRequest")]
        public async Task<IActionResult> SendRequest([FromBody] FriendRequestDto dto)
        {
            try
            {
                await _friendService.SendFriendRequest(CurrentUserId, dto.ReceiverId);
                return Ok(new { Message = "Friend request sent successfully.", dto.ReceiverId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("requests/{requestId}/accept")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            try
            {
                await _friendService.AcceptFriendRequest(requestId, CurrentUserId);
                return Ok(new { Message = "Friend request accepted.", RequestId = requestId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("requests/{requestId}/reject")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            try
            {
                await _friendService.RejectFriendRequest(requestId, CurrentUserId);
                return Ok(new { Message = "Friend request rejected.", RequestId = requestId });

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("requests/pending")]
        public async Task<ActionResult<List<PendingFriendsDto>>> GetPendingRequests()
        {
            var result = await _friendService.GetPendingRequests(CurrentUserId);
            return Ok(new
            {
                Message = "Pending requests retrieved successfully.",
                result.Count,
                Data = result
            });
        }

        [HttpGet("get-friends")]
        public async Task<ActionResult<FriendsResponseDto>> GetFriends()
        {
            var result = await _friendService.GetFriends(CurrentUserId);
            return Ok(new
            {
                Message = "Friends retrieved successfully",
                result.TotalCount,
                Data = result.Friends
            });
        }
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            await _friendService.RemoveFriend(CurrentUserId, friendId);
            return Ok("Removed Correctlly");
        }

        [HttpGet("GetRequestsSended")]
        public async Task<ActionResult<List<FriendsDto>>> GetRequestsSended()
        {
            var result = await _friendService.GetRequestsSended(CurrentUserId);
            return Ok(new
            {
                Message = "Sent requests retrieved successfully.",
                result.Count,
                Data = result
            });
        }

        [HttpPut("requests/{requestId}/cancel")]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            try
            {
                await _friendService.CancelFriendReuests(requestId);
                return Ok(new { Message = "Friend request cancelled.", RequestId = requestId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}