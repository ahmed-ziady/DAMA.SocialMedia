using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class FriendRecommendationService(DamaContext _context) : IFriendRecommendationService
    {



        public async Task<List<User>> GetFriendRecommendations(int userId, int maxResults = 10)
        {
            return await _context.Users
                .FromSqlInterpolated($@"
                WITH MutualFriends AS (
                    SELECT f2.ReceiverId AS MutualId
                    FROM Friendships f1
                    JOIN Friendships f2 ON f1.ReceiverId = f2.RequesterId
                    WHERE f1.RequesterId = {userId}
                    EXCEPT
                    SELECT ReceiverId FROM Friendships WHERE RequesterId = {userId}
                )
                SELECT TOP({maxResults}) u.*
                FROM MutualFriends mf
                JOIN Users u ON mf.MutualId = u.Id
                ORDER BY NEWID()")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}