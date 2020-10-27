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
    public class CommentsController : Controller
    {
        private ICommentRepository _repository;
        private UserManager<IdentityUser> _userManager;
        private IAuthorizationService _authorizationService;

        public CommentsController(ICommentRepository repository, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService = null)
        {
            _userManager = userManager;
            _repository = repository;
            _authorizationService = authorizationService;
        }

        // GET: Comments
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? id)
        {
            IEnumerable<Comment> comments = _repository.GetAll(id) as IEnumerable<Comment>;
            return View(comments);
        }


        // GET: Comments/Create
        public async Task<IActionResult> Create(int id)
        {
            var blog = _repository.GetBlog(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, blog, BlogOperations.Open);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            CommentsEditViewModel viewModel = new CommentsEditViewModel {ParentPostId = id};
            return View(viewModel);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Name,Text,ParentPostId")] CommentsEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();

            }

            try
            {
                var blog = _repository.GetBlog(viewModel.ParentPostId);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, blog, BlogOperations.Open);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                await _repository.Add(User, viewModel);
                return RedirectToRoute(new {controller = "Comments", action = "Index", id = viewModel.ParentPostId});
            }
            catch
            {
                return NotFound();
            }

        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            if (_repository.Get(id) == null)
            {
                return NotFound();
            }

            var comment = _repository.GetViewModel(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, comment, BlogOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,Name,Text,ParentPostId,OwnerId")] CommentsEditViewModel viewModel)
        {

            if (id != viewModel.CommentId)
            {
                return NotFound("Not found error");
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
            return RedirectToRoute(new { controller = "Comments", action = "Index", id = viewModel.ParentPostId });
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound();
            }
            CommentsEditViewModel comment = _repository.GetViewModel(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, comment, BlogOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound();
            }
            CommentsEditViewModel comment = _repository.GetViewModel(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, comment, BlogOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            _repository.Delete(id);
            return RedirectToRoute(new { controller = "Comments", action = "Index", id = comment.ParentPostId });
        }
    }
}
