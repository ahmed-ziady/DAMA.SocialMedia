using DAMA.Application.DTOs.Setting;
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
    public class SettingController(
        ISettings settings,
        UserManager<User> userManager,
        IAuthService authService) : ControllerBase
    {
        private int GetCurrentUserId()
        {
            var id = userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException("User is not authenticated.");
            return int.Parse(id);
        }

        private string GetCurrentUserEmail()
        {
            var email = userManager.GetUserName(User);
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("User is not authenticated.");
            return email;
        }

        [HttpPut("first-name")]
        public async Task<IActionResult> ChangeFirstName([FromQuery] string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return BadRequest(new { error = "First name cannot be empty." });
            if (firstName.Length < 3 || firstName.Length > 12)
                return BadRequest(new { error = "First name must be between 3 and 12 characters." });
            if (!firstName.All(char.IsLetter))
                return BadRequest(new { error = "First name can only contain letters." });
            if (firstName.Contains(' '))
                return BadRequest(new { error = "First name cannot contain spaces." });

            try
            {
                await settings.ChangeFirstName(firstName, GetCurrentUserId());
                return Ok(new { message = "First name updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("last-name")]
        public async Task<IActionResult> ChangeLastName([FromQuery] string lastName)
        {

            if (string.IsNullOrWhiteSpace(lastName))
                return BadRequest(new { error = "Last name cannot be empty." });
            if (lastName.Length < 2 || lastName.Length > 12)
                return BadRequest(new { error = "Last name must be between 2 and 12 characters." });
            if (!lastName.All(char.IsLetter))
                return BadRequest(new { error = "Last name can only contain letters." });
            if (lastName.Contains(' '))
                return BadRequest(new { error = "Last name cannot contain spaces." });



            try
            {
                await settings.ChangeLastName(lastName, GetCurrentUserId());
                return Ok(new { message = "Last name updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("profile-image")]
        public async Task<IActionResult> ChangeProfileImage([FromForm] ChangeProfileImageeDto request)
        {

            if (request.ProfileImage.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "Profile image size must be less than 5MB." });


            try
            {
                await settings.ChangeProfileImage(request.ProfileImage, GetCurrentUserId());
                return Ok(new { message = "Profile image updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("cover-image")]
        public async Task<IActionResult> ChangeCoverImage([FromForm] ChangeCoverImageDto request)
        {

            if (request.CoverImage.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "Cover image size must be less than 5MB." });

            try
            {
                await settings.ChangeCoverImage(request.CoverImage, GetCurrentUserId());
                return Ok(new { message = "Cover image updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-verification-code")]
        public async Task<IActionResult> SendVerificationCode()
        {
            try
            {
                await authService.ForgotPassword(GetCurrentUserEmail());
                return Accepted(new { message = "Verification code sent to your email." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(GetCurrentUserEmail()) || string.IsNullOrWhiteSpace(resetPasswordDto.VerificationCode) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
                return BadRequest("Email, verification code, and new password are required.");

            var result = await authService.ResetPassword(GetCurrentUserEmail(), resetPasswordDto.VerificationCode, resetPasswordDto.NewPassword);
            return result
                ? Ok("Password reset successfully.")
                : BadRequest("Invalid or expired verification code.");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await authService.Logout(GetCurrentUserId());
            return result ? Ok("User logged out successfully.") : BadRequest("Logout failed.");
        }

        [HttpDelete("account")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                await authService.DeleteAccount(GetCurrentUserId());
                return Ok(new { message = "Account deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
