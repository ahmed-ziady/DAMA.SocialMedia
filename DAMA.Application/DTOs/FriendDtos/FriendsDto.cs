namespace DAMA.Application.DTOs.FriendDtos
{
    public record FriendsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int FriendId { get; set; }

    }
}
