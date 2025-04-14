using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class Post
{
    public int PostId { get; set; }

    public int Id { get; set; }

    public string PostContent { get; set; } = null!;

    public int PostTypeId { get; set; }

    public DateTime? PostDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<FlaggedPost> FlaggedPosts { get; set; } = new List<FlaggedPost>();

    public virtual ICollection<ModerationAction> ModerationActions { get; set; } = new List<ModerationAction>();

    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();

    public virtual ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();

    public virtual PostType PostType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
