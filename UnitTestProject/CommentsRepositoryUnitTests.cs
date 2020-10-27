using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Data;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestProject;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTestProject
{
    public abstract class CommentRepositoryUnitTests
    {
        private ICommentRepository _repository;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        protected CommentRepositoryUnitTests(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Initialize();
        }
        protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }


        public void Initialize()
        {
            _mockUserManager = MockHelpers.MockUserManager<IdentityUser>();
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Blog blog = new Blog
                {
                    Name = "En blog",
                    Description = "En deskripsjon",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Owner = new IdentityUser("user"),
                    Status = Blog.BlogStatus.Open
                };
                context.Blogs.AddRange(blog);
                Post post = new Post
                {
                    Title = "En Comment",
                    Text = "En deskripsjon",
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Owner = new IdentityUser("user"),
                    ParentBlog = blog
                };
                context.Posts.AddRange(post);

                context.Comments.AddRange( new Comment
                {
                    Created=DateTime.Now,Modified = DateTime.Now,Name = "kommentar",Owner=new IdentityUser(), Text = "tekst",ParentPost = post
                });
                    
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task CanGetAllComments()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                var result = _repository.GetAll(1);
                Assert.AreEqual(result.Count(), 1);
            }
        }

        [Fact]
        public async Task CanGetComment()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                var result = _repository.Get(1);
                Assert.AreEqual(result.CommentId, 1);
            }
        }

        [Fact]
        public async Task CanGetCommentEditViewModel()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                var result = _repository.GetViewModel(1);
                Assert.AreEqual(result.CommentId, 1);

            }
        }
        [Fact]
        public async Task CanAddComment()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                CommentsEditViewModel Comment = new CommentsEditViewModel()
                {
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Text = "Test,",
                    Name = "navn",
                    ParentPostId = 1
                };
                var result = _repository.Add(It.IsAny<ClaimsPrincipal>(), Comment);
                Assert.AreNotEqual(0, result.Id);
            }
        }
        [Fact]
        public async Task CanSaveComment()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                CommentsEditViewModel Comment = new CommentsEditViewModel
                {
                    CommentId = 1,
                    Modified = DateTime.Now,
                    Text = "Test,",
                    Name = "navn",
                };
                _repository.Update(Comment);
                Assert.IsNotNull(context.Set<Comment>().First(x => x.Name == "navn"));
            }
        }
        [Fact]
        public async Task CanRemoveComment()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                _repository.Delete(1);

                Assert.IsTrue(context.Set<Comment>().Count() == 0);
            }
        }

        [Fact]
        public async Task CanGetBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new CommentRepository(_mockUserManager.Object, context);
                var result = _repository.GetBlog(1);
                Assert.AreEqual(result.BlogId, 1);
            }
        }
    }
}
