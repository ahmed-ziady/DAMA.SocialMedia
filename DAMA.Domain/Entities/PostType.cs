using System;
using System.Collections.Generic;

namespace DAMA.Domain.Entities;

public partial class PostType
{
    public int PostTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
