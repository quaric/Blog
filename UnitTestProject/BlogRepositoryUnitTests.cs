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
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTestProject
{
    public abstract class BlogRepositoryUnitTests
    {
        private IBlogRepository _repository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;

        protected BlogRepositoryUnitTests(DbContextOptions<ApplicationDbContext> contextOptions)
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

                context.Blogs.AddRange(
                    new Blog
                    {
                        Name = "En blog", Description = "En deskripsjon", Created = DateTime.Now,
                        Modified = DateTime.Now, Owner = new ApplicationUser(), Status = Blog.BlogStatus.Open
                    });
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task CanGetAllBlogs()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                var result = _repository.GetAll();
                Assert.AreEqual(result.Count(), 1);
            }
        }

        [Fact]
        public async Task CanGetBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                var result = _repository.Get(1);
                Assert.AreEqual(result.BlogId, 1);
            }
        }

        [Fact]
        public async Task CanGetEmptyBlogEditViewModel()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                var result = _repository.GetBlogEditViewModel();
                Assert.IsInstanceOfType(result, typeof(BlogEditViewModel));
            }
        }

        [Fact]
        public async Task CanGetBlogEditViewModel()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                var result = _repository.GetBlogEditViewModel(1);
                Assert.AreEqual(result.BlogId, 1);
                Assert.AreEqual(result.Name, "En blog");
                Assert.AreEqual(result.Description, "En deskripsjon");

            }
        }
        [Fact]
        public async Task CanAddBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                Blog blog = new Blog
                {
                    Created = DateTime.Now, Modified = DateTime.Now, Description = "Test,", Name = "navn", Status = Blog.BlogStatus.Open
                };
                var result = _repository.Add(It.IsAny<ClaimsPrincipal>(), blog);
                Assert.AreNotEqual( 0, result.Id);
            }
        }
        [Fact]
        public async Task CanSaveBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                Blog blog = new Blog
                {
                    BlogId = 1,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Description = "Test,",
                    Name = "navn",
                    Status = Blog.BlogStatus.Open
                };
                _repository.SaveChanges(blog);
                Assert.IsTrue(context.Set<Blog>().Any(x=>x.Name == "navn"));
            }
        }
        [Fact]
        public async Task CanRemoveBlog()
        {
            using (var context = new ApplicationDbContext(ContextOptions))
            {
                _repository = new BlogRepository(_mockUserManager.Object, context);
                _repository.Delete(1);

                Assert.IsTrue(context.Set<Blog>().Count() == 0);
            }
        }
    }
}
