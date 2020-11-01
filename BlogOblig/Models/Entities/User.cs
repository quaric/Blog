using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogOblig.Models.Entities
{
    /// <summary>
    /// En bruker, brukes til å hente JSON-data som objekt.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Brukerens Id, settes ikke på request
        /// </summary>
        /// <example>null</example>
        public string Id { get; set; }
        /// <summary>
        /// Brukerens brukernavn (e-post)
        /// </summary>
        /// <example>admin@admin.com</example>
        public string Username { get; set; }
        /// <summary>
        /// Brukerens passord
        /// </summary>
        /// <example>123@Lolol</example>
        public string Passwd { get; set; }
    }
}
