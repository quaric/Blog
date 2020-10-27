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
    public class PostsController : Controller
    {
        private IPostRepository _repository;
        private UserManager<IdentityUser> _userManager;
        private IAuthorizationService _authorizationService;
        public PostsController(IPostRepository repository, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService = null)
        {
            _userManager = userManager;
            _repository = repository;
            _authorizationService = authorizationService;
        }

        // GET: Posts
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? id)
        {
            IEnumerable<Post> posts = _repository.GetAll(id) as IEnumerable<Post>;
            return View(posts);
        }

        // GET: Posts/RedirectToPost/5
        public async Task<IActionResult> RedirectToPost(int? id)
        {
            if (_repository.Get(id) == null) return NotFound();
            return RedirectToRoute(new { controller = "Comments", action = "Index", id = id });
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create(int id)
        {
            var blog = _repository.GetBlog(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, blog, BlogOperations.Open);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            PostEditViewModel viewModel = new PostEditViewModel { ParentBlogId = id };
            return View(viewModel);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Text,ParentBlogId")] PostEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var blog = _repository.GetBlog(viewModel.ParentBlogId);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, blog, BlogOperations.Open);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                await _repository.Add(User, viewModel);
                return RedirectToRoute(new {controller = "Posts", action = "Index", id = viewModel.ParentBlogId});
            }
            catch
            {
                return NotFound();
            }
     
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound();
            }

            var viewModel = _repository.GetViewModel(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, viewModel, BlogOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(viewModel);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Text,ParentBlogId,OwnerId")] PostEditViewModel viewModel)
        {
            if (id != viewModel.PostId)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, viewModel, BlogOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            
            _repository.Update(viewModel);
            return RedirectToRoute(new { controller = "Posts", action = "Index", id = viewModel.ParentBlogId });
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            PostEditViewModel post = _repository.GetViewModel(id);

            if (post == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, post, BlogOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            
          

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            PostEditViewModel post = _repository.GetViewModel(id);

            if (post == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, post, BlogOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            _repository.Delete(id);
            return RedirectToRoute(new {controller="Posts", action="Index", id=post.ParentBlogId});
        }
    }
}
