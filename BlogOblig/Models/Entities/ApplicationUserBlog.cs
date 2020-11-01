using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.Entities
{       
    /// <summary>
    /// En join table av Application User og Blog 
    /// </summary>
    public class ApplicationUserBlog
    {
        /// <summary>
        /// ID til ApplicationUser, primærnøkkel
        /// </summary>
        /// <example>"af-43243-fdsf"</example>
        public string ApplicationUserId { get; set; }
        /// <summary>
        /// ApplicationUser-objekt til ApplicationUserId
        /// </summary>
        public ApplicationUser ApplicationUser { get; set; }
        /// <summary>
        /// Primærnøkkel til en Blog
        /// </summary>
        /// <example>2</example>
        public int BlogId { get; set; }
        /// <summary>
        /// Blog-objektet til BlogId
        /// </summary>
        public Blog Blog { get; set; }
    }
}
