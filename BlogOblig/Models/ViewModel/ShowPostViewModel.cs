using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;

namespace BlogOblig.Models.ViewModel
{
    public class ShowPostViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Owner { get; set; }
        public DateTime Modified { get; set; }
        public List<Comment> Comments { get; set; }
        public int BlogId { get; set; }
    }
}
