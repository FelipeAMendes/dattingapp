using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Api.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}