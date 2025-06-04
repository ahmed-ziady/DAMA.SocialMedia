using DAMA.Application.DTOs.Profile;
using DAMA.Application.Interfaces;
using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class ProfileServices(DamaContext _context) : IProfile
    {
        public async Task<ProfileResponseDto> GetProfileAsync(int userId)
        {
            bool isFriend = await _context.Friendships.AnyAsync(u => u.RequesterId == userId || u.ReceiverId == userId);


            var profile = await _context.Users.AsNoTracking().Where(u => u.Id == userId).Select(



                u => new ProfileResponseDto
                {

                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = (DateOnly)u.DateOfBirth,
                    ProfileImageUrl = u.ProfileImageUrl,
                    CoverImageUrl = u.CoverImageUrl,
                    IsFriend = isFriend

                }

                ).FirstOrDefaultAsync() ?? throw new Exception("Profile not found");


            return profile;
        }
    }

}
