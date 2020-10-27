using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public virtual Blog ParentBlog { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public virtual IdentityUser Owner { get; set; }

    }
}
