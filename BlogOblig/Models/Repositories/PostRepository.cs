using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Data;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogOblig.Models
{
    public class PostRepository : IPostRepository
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public IEnumerable<Post> GetAll(int? id)
        {
            if (id != null)
            {
                return _context.Posts.Include(x=>x.Owner).Where(x => x.ParentBlog.BlogId == id).ToList();
            }

            return null;
        }

        public Post Get(int? id)
        {
            if (id != null)
            {
                return _context.Posts.Include(x=>x.Owner).Include(x=>x.ParentBlog).FirstOrDefault(x => x.PostId == id);
            }

            return null;
        }

        public PostEditViewModel GetViewModel(int id)
        {
            var post = _context.Posts.Include(x => x.Owner).Include(x => x.ParentBlog).ThenInclude(x=>x.Owner).FirstOrDefault(x => x.PostId == id);
            PostEditViewModel viewModel = new PostEditViewModel{ ParentBlogId = post.ParentBlog.BlogId, Title = post.Title, Text = post.Text, PostId = post.PostId, OwnerId = post.ParentBlog.Owner.Id};
            return viewModel;
        }

   
        public async Task Add(IPrincipal p, PostEditViewModel viewModel)
        {

            Blog blog = _context.Blogs.Include(x=>x.Posts).First(x => x.BlogId == viewModel.ParentBlogId);
            Post post = new Post
            {
                Title = viewModel.Title,
                Text = viewModel.Text,
                ParentBlog = blog,
                Owner = await _userManager.FindByNameAsync(p.Identity.Name),
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            await _context.AddAsync(post);
            blog.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public void Update(PostEditViewModel viewModel)
        {
            try
            {
                var post = _context.Posts.First(x => x.PostId == viewModel.PostId);
                post.Title = viewModel.Title;
                post.Text = viewModel.Text;
                post.Modified = DateTime.Now;
                _context.Update(post);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DBConcurrencyException("Failure when updating post");
            }
        }

        public void Delete(int? id)
        {
            try
            {
                Post post = _context.Posts.First(x => x.PostId == id);
                var comments = _context.Comments.Where(x => x.ParentPost == post).ToList();
                _context.RemoveRange(comments);
                _context.Remove(post);
                _context.SaveChanges();
            }
            catch (DBConcurrencyException)
            {
                throw new DBConcurrencyException("Failure when deleting post");
            }
        }

        public BlogEditViewModel GetBlog(int id)
        {
            Blog blog = _context.Blogs.Include(x => x.Owner).FirstOrDefault(x => x.BlogId == id);
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

    }
}
