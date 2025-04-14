using System.Threading.Tasks;

namespace DAMA.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(string firstName, string lastName, DateOnly dateOfBirth, string email, string password, string? profileImageUrl, string? portfolioImageUrl);
        Task<bool> ConfirmVerificationCode(string email, string code);
        Task<bool> ResendVerificationCode(string email);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(string email, string verificationCode, string newPassword);
        Task<string> LoginUser(string email, string password);
        Task<bool> Logout(int userId);
        Task<bool> DeleteAccount(int userId);
        string? GetCurrentUserId();

        Task<bool> IsTokenRevoked(string token);
    }
}
