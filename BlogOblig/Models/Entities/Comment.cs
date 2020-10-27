using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public virtual Post ParentPost { get; set; }
        public virtual IdentityUser Owner { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
