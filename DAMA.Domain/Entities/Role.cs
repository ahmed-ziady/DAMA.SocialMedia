using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DAMA.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public Role() : base() { }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
