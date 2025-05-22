using DAMA.Application.DTOs.Profile;

namespace DAMA.Application.Interfaces
{
    public interface IProfile
    {
        public Task<ProfileResponseDto> GetProfileAsync(int userId);


    }
}
