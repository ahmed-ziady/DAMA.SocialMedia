using DAMA.Application.DTOs.PostDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DAMA.Infrastructure.Services
{
    public class PostServices(DamaContext context, UserManager<User> userManager) : IPostServicess
    {
        public async Task CreatePostAsync(CreatePostDto createPostDto, int id)
        {
            var mediaName = string.Empty;

            if (createPostDto.Image != null)
            {
                ValidateImage(createPostDto.Image);
                var imagesFolder = Path.Combine("wwwroot", "uploads", "posts", "images");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);
                mediaName = await FileHelper.SaveImageAsync(createPostDto.Image, imagesFolder);

            }
            if (createPostDto.Video != null)
            {
                ValidateVideo(createPostDto.Video);
                var videosFolder = Path.Combine("wwwroot", "uploads", "posts", "videos");
                if (!Directory.Exists(videosFolder))
                    Directory.CreateDirectory(videosFolder);
                mediaName = await FileHelper.SaveImageAsync(createPostDto.Video, videosFolder);
            }

            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                MediaUrl = createPostDto.Image != null ? $"/uploads/posts/images/{mediaName}" :
                createPostDto.Video != null ? $"/uploads/posts/videos/{mediaName}" : null,
                CreatedAt = DateTime.UtcNow,
                UserId = id,


            };

            context.Posts.Add(post);
            await context.SaveChangesAsync();




        }

        public async Task DeletePostAsync(int postId)
        {
            var post = await context.Posts.FindAsync(postId);
            if (post == null) return;


            if (post.MediaUrl != null)
            {
                await DeleteMediaFileIfExists(post.MediaUrl);

            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }

        private static async Task DeleteMediaFileIfExists(string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl)) return;

            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                          mediaUrl.TrimStart('/', '\\'));

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));

                    var directory = Path.GetDirectoryName(filePath);
                    if (Directory.Exists(directory) &&
                        !Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        Directory.Delete(directory);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Media deletion failed: {ex.Message}");
            }
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
        private static void ValidateVideo(IFormFile video)
        {
            if (video == null) return;
            var allowedExtensions = new[] { ".mp4", ".avi", ".mov" };
            const long maxSize = 50 * 1024 * 1024; // 50MB
            var ext = Path.GetExtension(video.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Video must be MP4/AVI/MOV format.");
            if (video.Length > maxSize)
                throw new InvalidOperationException("Video exceeds 50MB size limit.");
        }


    }
}
