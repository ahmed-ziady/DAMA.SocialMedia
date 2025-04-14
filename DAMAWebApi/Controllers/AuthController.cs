using DAMA.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DAMAWebApi.Controllers
{
   
    [Route("api/auth")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

private int GetCurrentUserId()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    Console.WriteLine($"Extracted User ID: {userIdClaim ?? "NULL"}");

    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    {
        throw new UnauthorizedAccessException("Invalid token: missing or invalid user ID.");
    }

    return userId;
}
        [AllowAnonymous]

        // Register User (Images are Optional)
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] DateOnly dateOfBirth, // Required
            [FromQuery] string email,
            [FromQuery] string password,
            [FromQuery] string? profileImageUrl = null, // Optional
            [FromQuery] string? portfolioImageUrl = null) // Optional
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("First name, last name, email, password, and date of birth are required.");
            }

            var success = await _authService.RegisterUser(firstName, lastName, dateOfBirth, email, password, profileImageUrl, portfolioImageUrl);
            if (!success)
            {
                return BadRequest("Registration failed. Email may already be registered.");
            }
            return Ok("Please check your email for the verification code.");
        }



        [AllowAnonymous]

        // Verify Email with Code
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] string email,
            [FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                return BadRequest("Email and verification code are required.");

            var success = await _authService.ConfirmVerificationCode(email, code);
            if (!success)
                return BadRequest("Invalid verification code.");

            return Ok("Email verified successfully. You can now log in.");
        }


        [AllowAnonymous]

        // Resend Verification Code
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var success = await _authService.ResendVerificationCode(email);
            if (!success)
                return BadRequest("Verification code could not be resent, maybe it's already verified.");

            return Ok("Verification code resent successfully.");
        }

        // Forgot Password (Sends Verification Code)
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var success = await _authService.ForgotPassword(email);
            return success
                ? Ok("A password reset verification code has been sent to your email.")
                : BadRequest("User not found.");
        }

        // Reset Password with Verification Code
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromQuery] string email,
            [FromQuery] string verificationCode,
            [FromQuery] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(verificationCode) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest("Email, verification code, and new password are required.");
            }

            var success = await _authService.ResetPassword(email, verificationCode, newPassword);
            return success
                ? Ok("Password reset successfully.")
                : BadRequest("Invalid or expired verification code.");
        }

        // Login User
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromQuery] string email,
            [FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest("Email and password are required.");

            var token = await _authService.LoginUser(email, password);
            return Ok(new { Token = token });
        }

        // Logout User
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            int userId = GetCurrentUserId();
            Console.WriteLine("ASDFFFFAFDASFDA:" + userId);
            var success = await _authService.Logout(userId);
            return success ? Ok("User logged out successfully.") : BadRequest("Logout failed.");
        }

        // Delete Account (Using token's user ID)
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            int userId = GetCurrentUserId();
            var success = await _authService.DeleteAccount(userId);
            return success ? Ok("User account deleted successfully.") : NotFound("User not found or could not be deleted.");
        }
    }
}
