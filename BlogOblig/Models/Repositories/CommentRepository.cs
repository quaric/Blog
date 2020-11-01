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
    public class CommentRepository : ICommentRepository
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Comment>> GetAll(int? id)
        {
            if (id != null)
            {
                return await _context.Comments.Include(x=>x.Owner).Where(x => x.ParentPost.PostId == id).ToListAsync();
            }

            return null;
        }

        public Comment Get(int? id)
        {
            if (id != null)
            {
                return _context.Comments.Include(x=>x.Owner).Include(x=>x.ParentPost).FirstOrDefault(x => x.CommentId == id);
            }

            return null;
        }

        public CommentsEditViewModel GetViewModel(int id)
        {
            CommentsEditViewModel viewModel;
            if (Get(id) == null)
            {
                viewModel = new CommentsEditViewModel {ParentPostId = id};
            }
            else
            {
                var comment = _context.Comments.Include(x => x.Owner).Include(x => x.ParentPost)
                    .FirstOrDefault(x => x.CommentId == id);
                viewModel = new CommentsEditViewModel
                {
                    ParentPostId = comment.ParentPost.PostId, Name = comment.Name, Text = comment.Text,
                    CommentId = comment.CommentId, OwnerId = comment.Owner.Id
                };
            }

            return viewModel;
        }

        public async Task Add(IPrincipal p, CommentsEditViewModel viewModel)
        {

            Comment comment = new Comment {
                Name = viewModel.Name,
                Text = viewModel.Text,
                ParentPost = _context.Posts.First(x => x.PostId == viewModel.ParentPostId),
                Owner = await _userManager.FindByNameAsync(p.Identity.Name),
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            await _context.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        
        public void Update(CommentsEditViewModel viewModel)
        {
            try
            {
                var comment = _context.Comments.First(x => x.CommentId == viewModel.CommentId);
                comment.Name = viewModel.Name;
                comment.Text = viewModel.Text;
                comment.Modified = DateTime.Now;
                _context.Update(comment);
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
                Comment comment = _context.Comments.First(x => x.CommentId == id);
                _context.Remove(comment);
                _context.SaveChanges();
            }
            catch (DBConcurrencyException)
            {
                throw new DBConcurrencyException("Failure when deleting post");
            }
        }
        public BlogEditViewModel GetBlog(int id)
        {
            Post post = _context.Posts.Include(x => x.ParentBlog).First(x => x.PostId == id);
            Blog blog = _context.Blogs.Include(x => x.Owner).First(x => x.BlogId == post.ParentBlog.BlogId);
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
