using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.ViewModel
{
    public class CommentsEditViewModel
    {
        public int CommentId { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public int ParentPostId { get; set; }
        public string OwnerId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
