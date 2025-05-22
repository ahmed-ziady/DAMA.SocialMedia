using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class SettingServices(DamaContext context) : ISettings
    {
        public async Task ChangeFirstName(string newFirstName, int userId)
        {
            var user = await GetUser(userId);
            user.FirstName = newFirstName;
            await context.SaveChangesAsync();
        }

        public async Task ChangeLastName(string lastName, int userId)
        {
            var user = await GetUser(userId);
            user.LastName = lastName;
            await context.SaveChangesAsync();
        }

        public async Task ChangeProfileImage(IFormFile profileImage, int userId)
        {
            var user = await GetUser(userId);
            ValidateImage(profileImage);
            user.ProfileImageUrl = await SaveImage(profileImage, "profile", user.ProfileImageUrl!);
            await context.SaveChangesAsync();
        }

        public async Task ChangeCoverImage(IFormFile coverImage, int userId)
        {
            var user = await GetUser(userId);
            ValidateImage(coverImage);
            user.CoverImageUrl = await SaveImage(coverImage, "cover", user.CoverImageUrl!);
            await context.SaveChangesAsync();
        }

        private async Task<User> GetUser(int userId)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("User not found");
        }

        private static async Task<string> SaveImage(IFormFile image, string imageType, string existingUrl)
        {
            var uploadsRoot = Path.Combine("wwwroot", "uploads", "users", imageType);
            Directory.CreateDirectory(uploadsRoot);

            if (!string.IsNullOrEmpty(existingUrl))
            {
                var existingPath = Path.Combine("wwwroot", existingUrl.TrimStart('/'));
                if (File.Exists(existingPath)) File.Delete(existingPath);
            }

            var uniqueFileName = await FileHelper.SaveImageAsync(image, uploadsRoot);
            return Path.Combine("uploads", "users", imageType, uniqueFileName).Replace("\\", "/");
        }

        private static void ValidateImage(IFormFile image)
        {
            if (image == null) return;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            const long maxSize = 5 * 1024 * 1024;

            var ext = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Image must be JPG/JPEG/PNG format.");

            if (image.Length > maxSize)
                throw new InvalidOperationException("Image exceeds 5MB size limit.");
        }
    }

}

