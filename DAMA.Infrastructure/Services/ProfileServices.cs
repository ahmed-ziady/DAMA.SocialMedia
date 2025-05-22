using DAMA.Application.DTOs.Profile;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Infrastructure.Services
{
    public class ProfileServices(DamaContext _context, UserManager<User> userManager) : IProfile
    {
        public async Task<ProfileResponseDto> GetProfileAsync(int userId)
        {

            var profile = await _context.Users.AsNoTracking().Where(u => u.Id == userId).Select(

                u => new ProfileResponseDto
                {

                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = (DateOnly)u.DateOfBirth,
                    ProfileImageUrl = u.ProfileImageUrl,
                    CoverImageUrl = u.CoverImageUrl,

                }

                ).FirstOrDefaultAsync() ?? throw new Exception("Profile not found");


            return profile;
        }
    }

}
