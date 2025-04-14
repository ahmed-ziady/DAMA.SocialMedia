using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Infrastructure.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Concurrent;
using System.Security.Claims;


namespace DAMA.Infrastructure.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        private static readonly ConcurrentDictionary<string, (User user, string verificationCode, string password)> _pendingUsers = new();
        private static readonly ConcurrentDictionary<string, string> _passwordResetCodes = new();
        private readonly ConcurrentDictionary<int, string> _activeTokens = new();

        public AuthenticationService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        public async Task<bool> RegisterUser(string firstName, string lastName, DateOnly dateOfBirth, string email, string password, string? profileImageUrl, string? portfolioImageUrl)
        {
            if (await _userManager.FindByEmailAsync(email) is not null)
                return false;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                ProfileImageUrl = profileImageUrl,
                PortfolioImageUrl = portfolioImageUrl
            };

            var verificationCode = GenerateVerificationCode();
            _pendingUsers[email] = (user, verificationCode, password);

            var emailBody = $"<h3>Welcome to DAMA!</h3><p>Your verification code is: <strong>{verificationCode}</strong></p>";
            await _emailService.SendEmailAsync(email, "Verify Your Email", emailBody);
            return true;
        }

        public async Task<bool> ConfirmVerificationCode(string email, string code)
        {
            if (!_pendingUsers.TryGetValue(email, out var userInfo) || userInfo.verificationCode != code)
                return false;

            var (user, _, password) = userInfo;
            const string defaultRole = "USER";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _roleManager.CreateAsync(new Role { Name = defaultRole, NormalizedName = defaultRole.ToUpper() });
            }

            user.CreatedAt = DateTime.UtcNow;
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(user, defaultRole);
            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.UpdateSecurityStampAsync(user);

            user.EmailConfirmed = true;
            user.VerificationCode = null;
            await _userManager.UpdateAsync(user);
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
                await _emailService.SendEmailAsync(email, "Resend: Verify Your Email", emailBody);
                return true;
            }
            return false;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var verificationCode = GenerateVerificationCode();
            _passwordResetCodes[email] = verificationCode;
            var emailBody = $"<h3>Password Reset</h3><p>Your verification code is: <strong>{verificationCode}</strong></p>";
            await _emailService.SendEmailAsync(email, "Password Reset Code", emailBody);
            return true;
        }

        public async Task<bool> ResetPassword(string email, string verificationCode, string newPassword)
        {
            if (!_passwordResetCodes.TryGetValue(email, out var storedCode) || storedCode != verificationCode)
                return false;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (result.Succeeded)
            {
                _passwordResetCodes.TryRemove(email, out _);
                return true;
            }
            return false;
        }

        public async Task<string> LoginUser(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email.ToUpper());
            if (user == null || !user.EmailConfirmed || !await _userManager.CheckPasswordAsync(user, password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (_activeTokens.ContainsKey(user.Id))
                throw new UnauthorizedAccessException("User is already logged in.");

            var roles = await _userManager.GetRolesAsync(user);
            var userEmail = user.Email ?? throw new InvalidOperationException("User email cannot be null.");
            var token = _tokenService.GenerateToken(user.Id, userEmail, roles);
            _activeTokens[user.Id] = token;
            Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaa"+user.Id);
            Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" + token);
            return token;
        }
        public Task<bool> Logout(int userId)
        {
            _activeTokens.TryRemove(userId, out _);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAccount(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public string? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ✅ FIXED: Logging for debugging
            Console.WriteLine($"Extracted User ID: {userId ?? "NULL"}");

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid token: missing user id.");
            }

            return userId;
        }    

        public async Task<bool> IsTokenRevoked(string token)
        {
            // For this demo, token revocation is not implemented.
            return await Task.FromResult(false);
        }

        private static string GenerateVerificationCode() =>
            System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}
