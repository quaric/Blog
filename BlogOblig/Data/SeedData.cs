using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                ApplicationUser user = await userManager.FindByIdAsync(adminID);
                SeedDB(context, user);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw,
            string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser
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

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static async Task SeedDB(ApplicationDbContext context, ApplicationUser user)
        {
            if (context.Blogs.Any())
            {
                return; // DB has been seeded
            }
            Debug.WriteLine("DSADASDSA" + user.UserName);
            Blog blog = new Blog()
            {
                Name ="blog.Name",
                Description = "blog.Description",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Owner = context.Users.First(x=>x.UserName == "admin@admin.com")
            };
            context.Blogs.Add(blog);
            context.SaveChanges();
            user.Subscriptions = new List<Blog>();
            user.Subscriptions.Add(blog);
            context.SaveChanges();
            
            Post post = new Post
            {
                Title = "EnPost",
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Owner = user,
                Text = "En eksempel tekst her",
                ParentBlog = blog
            };

            context.Posts.Add(post);
            context.SaveChanges();

            context.Comments.Add(new Comment
            {
                Created = DateTime.Now, Modified = DateTime.Now, Name = "Hei", Text = "En kommentartekst her",
                Owner = user, ParentPost = post
            });
            await context.SaveChangesAsync();
        }
    }
}
