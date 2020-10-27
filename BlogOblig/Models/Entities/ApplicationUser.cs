using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual List<Blog> Subscriptions { get; set; }
    }
}
