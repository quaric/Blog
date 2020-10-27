using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlogOblig.Authorization;
using BlogOblig.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

namespace BlogOblig.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context =
                new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@admin.com");
                await EnsureRole(serviceProvider, adminID, Constants.BlogAdministratorsRole);
                var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
                IdentityUser user = await userManager.FindByIdAsync(adminID);
                SeedDB(context, user);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw,
            string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, IdentityUser user)
        {
            if (context.Blogs.Any())
            {
                return; // DB has been seeded
            }
          
            Blog blog = new Blog
            {
                Owner = user,
                Name = "SeedBLog",
                Description = "Blogg via Seed",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Status = Blog.BlogStatus.Open
            };
            Post post = new Post
            {
                Title = "EnPost",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Owner = user,
                Text = "En eksempel tekst her",
                ParentBlog = blog
            };

            context.Blogs.Add(blog);
            context.Posts.Add(post);
            context.Comments.Add(new Comment
            {
                Created = DateTime.Now, Modified = DateTime.Now, Name = "Hei", Text = "En kommentartekst her",
                Owner = user, ParentPost = post
            });
            context.SaveChanges();
        }
    }
}
