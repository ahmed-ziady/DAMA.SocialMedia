using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAMA.Domain.Entities;

public class User : IdentityUser<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public new int Id { get; set; }

    public string? RefreshToken { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public DateOnly? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ProfileImageUrl { get; set; }

    public string? PortfolioImageUrl { get; set; }
    public string? Bio { get; set; }

    public string? VerificationCode { get; set; }

    public bool IsVerified { get; set; } = false;

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<FlaggedPost> FlaggedPosts { get; set; } = [];

    public ICollection<FriendRequest> FriendRequestsSent { get; set; } = [];
    public ICollection<FriendRequest> FriendRequestsReceived { get; set; } = [];

    // Friendships
    public ICollection<Friendship> FriendshipRequestsSent { get; set; } = new List<Friendship>();

    // Friendships where this user received the request
    public ICollection<Friendship> FriendshipRequestsReceived { get; set; } = new List<Friendship>();

    public virtual ICollection<Notification> Notifications { get; set; } = [];

    public virtual ICollection<PostReaction> PostReactions { get; set; } = [];

    public virtual ICollection<PostReport> PostReports { get; set; } = [];

    public virtual ICollection<Post> Posts { get; set; } = [];

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = [];

}
