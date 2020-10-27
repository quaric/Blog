using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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




    }
}
