using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.Entities
{
    public class ApplicationUserBlog
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
