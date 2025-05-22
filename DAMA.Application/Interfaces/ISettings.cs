using Microsoft.AspNetCore.Http;

public interface ISettings
{
    Task ChangeFirstName(string newFirstName, int userId);
    Task ChangeLastName(string lastName, int userId);
    Task ChangeProfileImage(IFormFile profileImage, int userId);
    Task ChangeCoverImage(IFormFile coverImage, int userId);
}