using DAMA.Application.DTOs.SearchDto;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class SearchService(DamaContext context) : ISearchService
    {
        public async Task<List<SearchResponseDto>> SearchAsync(string term ,int id)
        {
            var requesterId = id;

            // Get requester's friends list once
            var requesterFriendIds = await context.Friendships.AsNoTracking()
                .Where(f => f.RequesterId == requesterId || f.ReceiverId == requesterId)
                .Select(f => f.RequesterId == requesterId ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            var query = context.Users.AsNoTracking()
                .Where(u => u.Id != requesterId); 

            if (!string.IsNullOrWhiteSpace(term))
            {
                var searchTerm = term.Trim().ToLower();
                var nameParts = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (nameParts.Length >= 2)
                {
                    var firstNamePart = nameParts[0];
                    var lastNamePart = string.Join(" ", nameParts.Skip(1));

                    query = query.Where(u =>
                        (u.FirstName != null && u.FirstName.ToLower().Contains(firstNamePart) &&
                         u.LastName != null && u.LastName.ToLower().Contains(lastNamePart)) ||
                        (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                        (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                        (u.NormalizedEmail != null && u.NormalizedEmail.Contains(searchTerm.ToUpper())));
                }
                else
                {
                    query = query.Where(u =>
                        (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                        (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                        (u.NormalizedEmail != null && u.NormalizedEmail.Contains(searchTerm.ToUpper())));
                }
            }

            var users = await query
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.ProfileImageUrl
                })
                .ToListAsync();

            var results = users
                .Select(u =>
                {
                    var mutualCount = context.Friendships.AsNoTracking()
                        .Where(f =>
                            (f.RequesterId == u.Id && requesterFriendIds.Contains(f.ReceiverId)) ||
                            (f.ReceiverId == u.Id && requesterFriendIds.Contains(f.RequesterId)))
                        .Select(f => f.RequesterId == u.Id ? f.ReceiverId : f.RequesterId)
                        .Distinct()
                        .Count();

                    return new SearchResponseDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName ?? string.Empty,
                        LastName = u.LastName ?? string.Empty,
                        ProfileImageUrl = u.ProfileImageUrl,
                        MutualFriendsCount = mutualCount
                    };
                })
                .OrderByDescending(u => u.MutualFriendsCount)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToList();

            return results;
        }
    }
}