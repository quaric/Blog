using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Data;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogOblig.Models.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace BlogOblig.Models
{
    public class BlogRepository : IBlogRepository
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public IEnumerable<Blog> GetAll()
        {
            IEnumerable<Blog> blogs = _context.Blogs.Include(x=>x.Owner).ToList();
            return blogs;
        }

        public async Task<List<SubscriptionViewModel>> GetBlogSubscriptions(IPrincipal principal)
        {
            var user1 = await _userManager.FindByNameAsync(principal.Identity.Name);
            var user = await _context.ApplicationUserBlogs.Include(x=>x.Blog).ThenInclude(x=>x.Posts).ThenInclude(z=>z.Comments).Where(x =>
                x.ApplicationUser == user1).ToListAsync();
            List<SubscriptionViewModel> subscriptionViewModels = new List<SubscriptionViewModel>();
            foreach (ApplicationUserBlog ab in user)
            {
                int numberOfComments = 0;
                DateTime lastActivity = ab.Blog.Modified;
                foreach (Post p in ab.Blog.Posts)
                {
                    numberOfComments += p.Comments.Count;
                    if (p.Modified.CompareTo(lastActivity) > 0) lastActivity = p.Modified;
                    foreach (Comment c in p.Comments)
                    {
                        if (c.Modified.CompareTo(lastActivity) > 0) lastActivity = c.Modified;
                    }
                }

                subscriptionViewModels.Add(new SubscriptionViewModel
                {
                    Description = ab.Blog.Description,
                    Name = ab.Blog.Name,
                    NumberOfPosts = ab.Blog.Posts.Count,
                    NumberOfComments = numberOfComments,
                    LastActivity = lastActivity
                });
            }
            return subscriptionViewModels;
        }

        public Blog Get(int id)
        {
            Blog blog =  _context.Blogs.Include(x => x.Owner).First(x=>x.BlogId == id);
            return blog;
        }

        public BlogEditViewModel GetBlogEditViewModel()
        {
            BlogEditViewModel viewModel = new BlogEditViewModel();
            return viewModel;
        }
        public BlogEditViewModel GetBlogEditViewModel(int id)
        {
            Blog blog = _context.Blogs.Include(x=>x.Owner).FirstOrDefault(x => x.BlogId == id);
            BlogEditViewModel viewModel = new BlogEditViewModel
            {
                BlogId = id,
                Name = blog.Name,
                Description = blog.Description,
                Created = blog.Created,
                OwnerId = blog.Owner.Id,
                Status = (BlogEditViewModel.BlogStatus)blog.Status
            };
            return viewModel;
        }


        public async Task Add(IPrincipal principal, Blog blog)
        {
            blog.Owner = await _userManager.FindByNameAsync(principal.Identity.Name);
            blog.Status = Blog.BlogStatus.Open;
            await _context.AddAsync(blog);
            await _context.SaveChangesAsync();
        }
        public void SaveChanges(Blog blog)
        {
            try
            {
                Blog oldBlog = _context.Blogs.First(x => x.BlogId == blog.BlogId);
                oldBlog.Name = blog.Name;
                oldBlog.Description = blog.Description;
                oldBlog.Modified = DateTime.Now;
                oldBlog.Status = blog.Status;
                _context.Update(oldBlog);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DBConcurrencyException("Failure when updating blog");
            }
        }

        public void Delete(int id)
        {
            try
            {
                Blog blog = _context.Blogs.First(x => x.BlogId == id);
                List<Post> posts = _context.Posts.Where(x => x.ParentBlog.BlogId == blog.BlogId).ToList();
                List<Comment> comments = _context.Comments.Include(x=>x.ParentPost).Where(x => posts.Contains(x.ParentPost)).ToList();
                _context.RemoveRange(comments);
                _context.RemoveRange(posts);
                _context.Remove(blog);
                _context.SaveChanges();
            }
            catch (DBConcurrencyException)
            {
                throw new DBConcurrencyException("Failure when deleting blog");
            }
        }

        
        public async Task Subscribe(IPrincipal principal, int id)
        {
            try
            {
                ApplicationUser user = _context.ApplicationUsers.Include(x=>x.ApplicationUserBlogs).First(x=>x.UserName == principal.Identity.Name);
                ApplicationUserBlog applicationUserBlog = _context.ApplicationUserBlogs.FirstOrDefault(x => x.BlogId == id);
                if (user.ApplicationUserBlogs.Contains(applicationUserBlog)) throw new Exception("Already subscribed");
                Blog blog = _context.Blogs.First(x => x.BlogId == id);
                await _context.ApplicationUserBlogs.AddAsync(new ApplicationUserBlog
                {
                    ApplicationUser = user,
                    Blog = blog
                });
                
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Invalid id or user: " + e.ToString());
            }
 
        }

        public async Task Unsubscribe(IPrincipal principal, int id)
        {
            try
            {
                ApplicationUser user = _context.ApplicationUsers.Include(x => x.ApplicationUserBlogs).First(x => x.UserName == principal.Identity.Name);
                ApplicationUserBlog applicationUserBlog = _context.ApplicationUserBlogs.FirstOrDefault(x => x.BlogId == id);
                if (!user.ApplicationUserBlogs.Contains(applicationUserBlog)) throw new Exception("Not subscribed");
                _context.ApplicationUserBlogs.Remove(applicationUserBlog);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Invalid id or user: " + e.ToString());
            }
        }




    }
}
