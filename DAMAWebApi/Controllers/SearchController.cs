using DAMA.Application.DTOs.SearchDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAMAWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController(
        ISearchService searchService,
        UserManager<User> userManager) : ControllerBase
    {
        private readonly ISearchService _searchService = searchService;
        private readonly UserManager<User> _userManager = userManager;

        private int GetCurrentUserId()
        {
            return int.TryParse(_userManager.GetUserId(User), out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchDto searchDto)
        {
            if (string.IsNullOrWhiteSpace(searchDto.SearchTerm))
                return BadRequest("Search term cannot be empty.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized("Invalid user.");

            var results = await _searchService.SearchAsync(searchDto.SearchTerm, userId);

            if (results is null || results.Count == 0)
                return NotFound("No users found matching the search criteria.");

            return Ok(results);
        }
    }
}
