using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.ViewModel
{
    public class BlogEditViewModel
    {
        public int BlogId { get; set; }
        [StringLength(20, ErrorMessage = "Må være under 20 bokstaver")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Blognavn må fylles ut")]
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string OwnerId { get; set; }
        public BlogStatus Status { get; set; }

        public enum BlogStatus
        {
            Open,
            Closed
        }
    }
}
