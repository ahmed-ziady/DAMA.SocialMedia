using DAMA.Application.DTOs.PostDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DAMA.Infrastructure.Services
{
    public class PostServices(DamaContext context) : IPostServicess
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

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await context.Posts.FindAsync(postId);
            if (post is null)
                return false;

            if (post.MediaUrl != null)
            {
                await DeleteMediaFileIfExists(post.MediaUrl);

            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();
            return true;
        }


        public async Task<List<NewsFeedDto>> GetFriendPostsAsync(int userID, int page = 1, int pageSize = 50)
        {

            var friendsIds = await context.Friendships.AsNoTracking()
                .Where(f => f.RequesterId == userID || f.ReceiverId == userID)
                .Select(f => f.RequesterId == userID ? f.ReceiverId : f.RequesterId)
                .ToListAsync();
            if (friendsIds.Count == 0)
                friendsIds.Add(userID); // Include self if no friends
            var posts = await context.Posts.AsNoTracking()
                .Where(p => friendsIds.Contains(p.UserId) || p.UserId == userID)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new NewsFeedDto
                {
                    PostId = p.PostId,
                    PostTitle = p.Title,
                    PostBody = p.Content,
                    MediaUrl = p.MediaUrl
                })
                .ToListAsync();
            return posts;
        }

        public async Task<List<NewsFeedDto>> GetUserPostsAsync(int UserId, int page = 1, int pageSize = 50)
        {
            var posts = await context.Posts.AsNoTracking().Where(p => p.UserId == UserId).OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new NewsFeedDto
                {
                    MediaUrl = p.MediaUrl,

                    PostId = p.PostId,
                    PostTitle = p.Title,
                    PostBody = p.Content

                })
                .ToListAsync();
            return posts;
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
