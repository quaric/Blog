using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IdentityUser Owner { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public BlogStatus Status { get; set; }

        public enum BlogStatus 
        {
            Open,
            Closed
        }
    }
}
