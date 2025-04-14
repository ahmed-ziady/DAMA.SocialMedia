using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class FlaggedPost
{
    public int FlagId { get; set; }

    public int PostId { get; set; }

    public int Id { get; set; }

    public string? FlagReason { get; set; }

    public string? ReviewStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
