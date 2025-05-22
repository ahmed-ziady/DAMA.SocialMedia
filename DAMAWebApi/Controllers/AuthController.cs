using DAMA.Application.DTOs.AuthDtos;
using DAMA.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAMAWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;



        // Register User (images optional)
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUser(registerDto);
            if (!result)
                return BadRequest("Registration failed. Email may already be registered.");

            return Ok("Please check your email for the verification code.");
        }

        // Verify Email with Code
        [AllowAnonymous]
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            if (string.IsNullOrWhiteSpace(verifyEmailDto.Email) || string.IsNullOrWhiteSpace(verifyEmailDto.Code))
                return BadRequest("Email and verification code are required.");

            var result = await _authService.ConfirmVerificationCode(verifyEmailDto.Email, verifyEmailDto.Code);
            return result ? Ok("Email verified successfully. You can now log in.") : BadRequest("Invalid verification code.");
        }

        // Resend Verification Code
        [AllowAnonymous]
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var result = await _authService.ResendVerificationCode(email);
            return result
                ? Ok("Verification code resent successfully.")
                : BadRequest("Verification code could not be resent. It may already be verified.");
        }

        // Forgot Password (send verification code)
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var result = await _authService.ForgotPassword(email);
            return result
                ? Ok("A password reset code has been sent to your email.")
                : BadRequest("User not found.");
        }

        // Reset Password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDto.Email) || string.IsNullOrWhiteSpace(resetPasswordDto.VerificationCode) || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
                return BadRequest("Email, verification code, and new password are required.");

            var result = await _authService.ResetPassword(resetPasswordDto.Email, resetPasswordDto.VerificationCode, resetPasswordDto.NewPassword);
            return result
                ? Ok("Password reset successfully.")
                : BadRequest("Invalid or expired verification code.");
        }

        // Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest("Email and password are required.");

            try
            {
                var token = await _authService.LoginUser(loginDto);
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // Logout
        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout([FromBody] int id)
        //{
        //    var userId = id;
        //    var result = await _authService.Logout(userId);
        //    return result ? Ok("User logged out successfully.") : BadRequest("Logout failed.");
        //}

        //// Delete Account
        //[Authorize]
        //[HttpDelete("delete-account")]
        //public async Task<IActionResult> DeleteAccount([FromBody] int id)
        //{
        //    var userId = id;
        //    var result = await _authService.DeleteAccount(userId);
        //    return result ? Ok("User account deleted successfully.") : NotFound("User not found.");
        //}

        // Refresh Token
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return result is not null ? Ok(result) : Unauthorized("Invalid or expired refresh token.");
        }

    }
}
