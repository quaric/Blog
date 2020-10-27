using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.ViewModel
{
    public class PostEditViewModel
    {
        public int PostId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Må være under 20 bokstaver")]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public int ParentBlogId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string OwnerId { get; set; }

    }
}
