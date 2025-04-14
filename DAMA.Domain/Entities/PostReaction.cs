using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class PostReaction
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string? ReactionType { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
