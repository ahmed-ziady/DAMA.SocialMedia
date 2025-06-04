using Microsoft.AspNetCore.Identity;

namespace DAMA.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<FriendRequest> SentFriendRequests { get; set; } = [];
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = [];
        public ICollection<Friendship> FriendshipsRequester { get; set; } = [];
        public ICollection<Friendship> FriendshipsReceiver { get; set; } = [];
        public ICollection<Post> Posts { get; set; } = [];


    }
}
