using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{       
    /// <summary>
    /// En post i blogg
    /// </summary>

    public class Post
    {
        /// <summary>
        /// Postens ID, primærnøkkel
        /// </summary>
        /// <example>1</example>
        public int PostId { get; set; }
        /// <summary>
        /// Postens tittel
        /// </summary>
        /// <example>En post</example>
        public string Title { get; set; }
        /// <summary>
        /// Postens tekstinnhold
        /// </summary>
        /// <example>I dag gjorde jeg lekser</example>
        public string Text { get; set; }
        /// <summary>
        /// Bloggen som posten tilhører.
        /// </summary>
        /// <example>blog objekt</example>
        public virtual Blog ParentBlog { get; set; }
        /// <summary>
        /// Når posten ble opprettet
        /// </summary>
        /// <example>20.10.2020 21:39:21</example>
        public DateTime Created { get; set; }
        /// <summary>
        /// Når posten ble sist endret
        /// </summary>
        /// <example>20.10.2020 21:39:21</example>
        public DateTime Modified { get; set; }
        /// <summary>
        /// ApplicationUser som laget posten
        /// </summary>
        /// <example>ApplicationUser objekt</example>
        public virtual ApplicationUser Owner { get; set; }
        /// <summary>
        /// En liste av Comments objekter som hører til Posten.
        /// </summary>
        /// <example>List<Comments> comments</example>
        public virtual List<Comment> Comments { get; set; }
    }
}
