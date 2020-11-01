using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Controllers;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Models.Entities
{
    /// <summary>
    /// Et blogobjekt
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// BlogId, primærnøkkel
        /// </summary>
        /// <example>1</example>
        public int BlogId { get; set; }
        /// <summary>
        /// Navn(Tittel) på Blog
        /// </summary>
        /// <example>"En blog"</example>
        public string Name { get; set; }
        /// <summary>
        /// Beskrivelse av bloggen
        /// </summary>
        /// <example>"Dette er en blog om UiT"</example>
        public string Description { get; set; }
        /// <summary>
        /// Eier av Blog-objektet
        /// </summary>
        public virtual ApplicationUser Owner { get; set; }
        /// <summary>
        /// Tid bloggen ble laget
        /// </summary>
        /// <example>20.20.2020 21:23:39</example>
        public DateTime Created { get; set; }
        /// <summary>
        /// Tid bloggen ble sist endret
        /// </summary>
        /// <example>20.20.2020 21:23:39</example>
        public DateTime Modified { get; set; }
        /// <summary>
        /// Status bestemmer om bloggen er åpen for endring eller ikke
        /// </summary>
        /// <example>Open</example>
        public BlogStatus Status { get; set; }
        /// <summary>
        /// Liste av Post-elementer som er tilknyttet bloggen
        /// </summary>
        public virtual List<Post> Posts { get; set; }
        /// <summary>
        /// Liste av ApplicationUserBlog elementer, som er en join table av ApplicationUser og Blog objektene
        /// </summary>
        public ICollection<ApplicationUserBlog> ApplicationUserBlogs { get; set; }
        /// <summary>
        /// Alternativer til status på Blog, Open eller Closed
        /// Brukes til IAuthorization sjekken
        /// </summary>
        public enum BlogStatus 
        {
            Open,
            Closed
        }
    }
}
