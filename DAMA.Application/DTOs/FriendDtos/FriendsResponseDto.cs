namespace DAMA.Application.DTOs.FriendDtos
{
    // In DAMA.Application.DTOs.FriendDtos
    public record FriendsResponseDto
    {
        public List<FriendsDto> Friends { get; set; } = new();
        public int TotalCount { get; set; }

    }
}
