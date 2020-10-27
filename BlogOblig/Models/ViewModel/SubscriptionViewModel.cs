using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.ViewModel
{
    public class SubscriptionViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfPosts { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
