namespace DAMA.Application.DTOs.Profile
{
    public record ProfileResponseDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateOnly DateOfBirth { get; init; }
        public string ProfileImageUrl { get; init; } = string.Empty;
        public string CoverImageUrl { get; init; } = string.Empty;

        public bool IsFriend { get; init; }
    }
}
