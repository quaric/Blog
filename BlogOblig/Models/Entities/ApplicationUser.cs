using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    /// <summary>
    /// Utvidelse av IdentityUser for å gi oss tilgang til Join-table med ApplicationUser og Blogs.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// En samling av ApplicationUserBlogs som hører til en enkelt bruker.
        /// </summary>
        public ICollection<ApplicationUserBlog> ApplicationUserBlogs { get; set; }
    }
}
