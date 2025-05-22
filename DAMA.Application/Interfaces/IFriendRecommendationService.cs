// IFriendService.cs
using DAMA.Domain.Entities;
// IFriendRecommendationService.cs
public interface IFriendRecommendationService
{
    Task<List<User>> GetFriendRecommendations(int userId, int maxResults = 10);
}
