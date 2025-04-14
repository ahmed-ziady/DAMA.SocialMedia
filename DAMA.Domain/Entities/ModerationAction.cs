using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class ModerationAction
{
    public int ActionId { get; set; }

    public int PostId { get; set; }

    public string? ActionType { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual Post Post { get; set; } = null!;
}
