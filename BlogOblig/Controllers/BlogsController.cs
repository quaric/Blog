using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogOblig.Data;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Controllers
{
    public class BlogsController : Controller
    {
        private IBlogRepository _repository;
        private UserManager<IdentityUser> _userManager; 
        private IAuthorizationService _authorizationService;

        public BlogsController(IBlogRepository repository, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService = null)
        {
            _userManager = userManager;
            _repository = repository;
            _authorizationService = authorizationService;
        }

        // GET: Blogs
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = _repository.GetAll();
            var kek = 1;
            return Ok(blogs);
        }

        // GET: Blogs/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToBlog(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound();
            }

            return RedirectToRoute(new { controller = "Posts", action = "Index", id = id });

        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            var blog = _repository.GetBlogEditViewModel();
            return View(blog);
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("BlogId,Name,Description")] BlogEditViewModel blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Blog newBlog = new Blog()
            {
                Name = blog.Name,
                Description = blog.Description,
                Created = DateTime.Now,
                Modified = DateTime.Now,
            };

            try
            {
                await _repository.Add(User, newBlog);
                return RedirectToAction("Index");
            }
            catch
            {
                return BadRequest("exception thrown");
            }
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BlogEditViewModel viewModel = _repository.GetBlogEditViewModel(id);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, viewModel, BlogOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(viewModel);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Name,Description,Created,OwnerId,Status")] BlogEditViewModel viewModel)
        {
            if (id != viewModel.BlogId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var blogOwner = await _userManager.FindByIdAsync(viewModel.OwnerId);

            Blog updatedBlog = new Blog
            {
                BlogId = viewModel.BlogId,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Created = viewModel.Created,
                Owner = blogOwner,
                Status = (Blog.BlogStatus)viewModel.Status
            };
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, viewModel, BlogOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            _repository.SaveChanges(updatedBlog);
            return RedirectToAction("Index");
            
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var viewModel = _repository.GetBlogEditViewModel(id);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, viewModel, BlogOperations.Delete);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }
                return View(viewModel);
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                BlogEditViewModel blog = _repository.GetBlogEditViewModel(id);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, blog, BlogOperations.Delete);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                _repository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return NotFound();
            }
           
        }
    }
}
