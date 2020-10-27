using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Controllers;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public BlogStatus Status { get; set; }
        public virtual List<Post> Posts { get; set; }

        public enum BlogStatus 
        {
            Open,
            Closed
        }
    }
}
