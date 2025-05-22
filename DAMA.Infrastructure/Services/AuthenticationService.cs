using DAMA.Application.DTOs.AuthDtos;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DAMA.Infrastructure.Services
{
    public class AuthenticationService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IEmailService emailService,
        IConfiguration config, DamaContext _context) : IAuthService
    {
        private static readonly ConcurrentDictionary<string, (User user, string verificationCode, string password)> _pendingUsers = new();
        private static readonly ConcurrentDictionary<string, string> _passwordResetCodes = new();
        private readonly ConcurrentDictionary<int, string> _activeTokens = new();

        public async Task<bool> RegisterUser(RegisterDto registerDto)
        {
            //ValidateImages(registerDto.ProfileImage, registerDto.CoverImage);

            if (registerDto.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new InvalidOperationException("Date of birth cannot be in the future.");

            var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return false;

            ValidateImages(registerDto.ProfileImage, registerDto.CoverImage);
            var imagesFolder = Path.Combine("wwwroot", "uploads", "users");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);
            var profileImageName = await FileHelper.SaveImageAsync(registerDto.ProfileImage, imagesFolder);
            var coverImageName = await FileHelper.SaveImageAsync(registerDto.CoverImage, imagesFolder);

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                DateOfBirth = registerDto.DateOfBirth,
                PhoneNumber = registerDto.PhoneNumber,
                ProfileImageUrl = $"/uploads/users/{profileImageName}",
                CoverImageUrl = coverImageName != null ? $"/uploads/users/{coverImageName}" : null
            };

            var verificationCode = GenerateVerificationCode();
            _pendingUsers[registerDto.Email] = (user, verificationCode, registerDto.Password);

            var emailBody = $"""
            <h3>Welcome to DAMA!</h3>
            <p>Your verification code is: <strong>{verificationCode}</strong></p>
            """;

            await emailService.SendEmailAsync(registerDto.Email, "Verify Your Email", emailBody);

            return true;
        }

        public async Task<bool> ConfirmVerificationCode(string email, string code)
        {
            if (!_pendingUsers.TryGetValue(email, out var userInfo) || userInfo.verificationCode != code)
                return false;

            var (user, _, password) = userInfo;
            const string defaultRole = "USER";

            if (!await roleManager.RoleExistsAsync(defaultRole))
            {
                await roleManager.CreateAsync(new Role(defaultRole));
            }

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;

            await userManager.AddToRoleAsync(user, defaultRole);
            await userManager.SetLockoutEnabledAsync(user, false);
            await userManager.UpdateSecurityStampAsync(user);

            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            _pendingUsers.TryRemove(email, out _);
            return true;
        }

        public async Task<bool> ResendVerificationCode(string email)
        {
            if (_pendingUsers.TryGetValue(email, out var userInfo))
            {
                var newCode = GenerateVerificationCode();
                _pendingUsers[email] = (userInfo.user, newCode, userInfo.password);
                var emailBody = $"<h3>Your new verification code is: <strong>{newCode}</strong></h3>";
                await emailService.SendEmailAsync(email, "Resend: Verify Your Email", emailBody);
                return true;
            }
            return false;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var verificationCode = GenerateVerificationCode();
            _passwordResetCodes[email] = verificationCode;
            var emailBody = $"<h3>Password Reset</h3><p>Your verification code is: <strong>{verificationCode}</strong></p>";
            await emailService.SendEmailAsync(email, "Password Reset Code", emailBody);
            return true;
        }

        public async Task<bool> ResetPassword(string email, string verificationCode, string newPassword)
        {
            if (!_passwordResetCodes.TryGetValue(email, out var storedCode) || storedCode != verificationCode)
                return false;

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                _passwordResetCodes.TryRemove(email, out _);
                return true;
            }

            return false;
        }

        public async Task<TokenResponseDto> LoginUser(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.EmailConfirmed || !await userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (_activeTokens.ContainsKey(user.Id))
                throw new UnauthorizedAccessException("User is already logged in.");

            return await CreateTokenResponseAsync(user);
        }

        public async Task<bool> Logout(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            await userManager.UpdateSecurityStampAsync(user);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userManager.UpdateAsync(user);

            _activeTokens.TryRemove(userId, out _);

            return true;
        }


        public async Task<bool> DeleteAccount(int userId)
        {
            var user = await userManager.Users
                .Include(u => u.SentFriendRequests)
                .Include(u => u.ReceivedFriendRequests)
                .Include(u => u.FriendshipsRequester)
                .Include(u => u.FriendshipsReceiver)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            // Delete related friend requests
            _context.FriendRequests.RemoveRange(user.SentFriendRequests);
            _context.FriendRequests.RemoveRange(user.ReceivedFriendRequests);

            // Delete friendships
            _context.Friendships.RemoveRange(user.FriendshipsRequester);
            _context.Friendships.RemoveRange(user.FriendshipsReceiver);

            // If user has posts, likes, comments — delete them here as well
            // Example:
            // _context.Posts.RemoveRange(user.Posts);

            // Delete user
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return false;

            await _context.SaveChangesAsync();
            _activeTokens.TryRemove(userId, out _);

            return true;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.RefreshToken))
                return null;

            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return await CreateTokenResponseAsync(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponseAsync(User user) => new()
        {
            AccessToken = GenerateJwtToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            Id = user.Id
        };

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Email!),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["AppSettings:Token"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: config["AppSettings:Issuer"],
                audience: config["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);
            return refreshToken;
        }

        private static void ValidateImages(IFormFile? profileImage, IFormFile? coverImage)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            const long maxSize = 5 * 1024 * 1024;

            void Validate(IFormFile? file, string name)
            {
                if (file == null) return;

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    throw new InvalidOperationException($"{name} must be JPG/JPEG/PNG.");

                if (file.Length > maxSize)
                    throw new InvalidOperationException($"{name} exceeds 5MB limit.");
            }

            Validate(profileImage, "Profile image");
            Validate(coverImage, "Cover image");
        }

        private static string GenerateVerificationCode() => RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}