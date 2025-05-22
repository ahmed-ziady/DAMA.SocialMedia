using DAMA.Application.DTOs.AuthDtos;
using System.Threading.Tasks;

namespace DAMA.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegisterDto registerDto);
        Task<bool> ConfirmVerificationCode(string email, string code);
        Task<bool> ResendVerificationCode(string email);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(string email, string verificationCode, string newPassword);
        Task<TokenResponseDto> LoginUser(LoginDto loginDto);
        Task<bool> Logout(int userId);
        Task<bool> DeleteAccount(int userId);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

    }
}
