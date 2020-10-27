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
    public abstract class PostRepositoryUnitTests
    {
        private IPostRepository _repository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;

        protected PostRepositoryUnitTests(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Initialize();
        }
        protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }


        public void Initialize()
        {
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
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
                    Owner = new ApplicationUser(),
                    Status = Blog.BlogStatus.Open
                };
                context.Blogs.AddRange(blog);
                    
                context.Posts.AddRange(
                    new Post
                    {
                        Title = "En Post",
                        Text = "En deskripsjon",
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        Owner = new ApplicationUser(),
                        ParentBlog = blog
                    });
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task CanGetAllPosts()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                var result = _repository.GetAll(1);
                Assert.AreEqual(result.Count(), 1);
            }
        }

        [Fact]
        public async Task CanGetPost()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                var result = _repository.Get(1);
                Assert.AreEqual(result.PostId, 1);
            }
        }

        [Fact]
        public async Task CanGetPostEditViewModel()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                var result = _repository.GetViewModel(1);
                Assert.AreEqual(result.PostId, 1);
                Assert.AreEqual(result.Title, "En Post"); 
                Assert.AreEqual(result.Text, "En deskripsjon");

            }
        }
        [Fact]
        public async Task CanAddPost()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                PostEditViewModel post = new PostEditViewModel()
                {
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Text = "Test,",
                    Title = "navn",
                    ParentBlogId = 1
                };
                var result = _repository.Add(It.IsAny<ClaimsPrincipal>(), post);
                Assert.AreNotEqual(0, result.Id);
            }
        }
        [Fact]
        public async Task CanSavePost()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                PostEditViewModel post = new PostEditViewModel
                {
                    PostId = 1,
                    Modified = DateTime.Now,
                    Text = "Test,",
                    Title = "navn",
                };
                _repository.Update(post);
                Assert.IsNotNull(context.Set<Post>().First(x=>x.Title=="navn"));
            }
        }
        [Fact]
        public async Task CanRemovePost()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                _repository.Delete(1);

                Assert.IsTrue(context.Set<Post>().Count() == 0);
            }
        }

        [Fact]
        public async Task CanGetBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new PostRepository(_mockUserManager.Object, context);
                var result = _repository.GetBlog(1);
                Assert.AreEqual(result.BlogId, 1);
            }
        }
    }
}
