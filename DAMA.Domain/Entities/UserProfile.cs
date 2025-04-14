using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class UserProfile
{
    public int UserProfileId { get; set; }

    public int? Id { get; set; }

    public string? Bio { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public virtual User? User { get; set; }
}
