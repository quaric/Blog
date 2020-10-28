using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogOblig.Models.Entities;

namespace BlogOblig.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ApplicationUserBlog> ApplicationUserBlogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserBlog>()
                .HasKey(ab => new {ab.ApplicationUserId, ab.BlogId});
            builder.Entity<ApplicationUserBlog>()
                .HasOne(ab => ab.ApplicationUser)
                .WithMany(b => b.ApplicationUserBlogs)
                .HasForeignKey(ab => ab.ApplicationUserId);
            builder.Entity<ApplicationUserBlog>()
                .HasOne(ab => ab.Blog)
                .WithMany(c => c.ApplicationUserBlogs)
                .HasForeignKey(ab => ab.BlogId);
        }
}
}
