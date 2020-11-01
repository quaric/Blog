using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    /// <summary>
    /// En kommentar som kan lages i Post
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// CommentId, primærnøkkel
        /// </summary>
        /// <example>1</example>
        public int CommentId { get; set; }
        /// <summary>
        /// Navn(Tittel) på kommentaren
        /// </summary>
        /// <example>"En fin dag"</example>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Tekstinnhold i kommentaren
        /// </summary>
        /// <example>"lorem ipsum"</example>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Foreldre Post som kommentaren hører til
        /// </summary>
        public virtual Post ParentPost { get; set; }
        /// <summary>
        /// Eier av kommentarobjektet
        /// </summary>
        public virtual ApplicationUser Owner { get; set; }
        /// <summary>
        /// Tid kommentaren ble laget
        /// </summary>
        /// <example>20.20.2020 21:23:39</example>
        public DateTime Created { get; set; }
        /// <summary>
        /// Tid kommentaren ble sist endret
        /// </summary>
        /// <example>20.20.2020 21:23:39</example>
        public DateTime Modified { get; set; }
    }
}
