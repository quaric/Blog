using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogOblig.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : Controller
    {
        private ICommentRepository _repository;

        public CommentsController(ICommentRepository repository)
        {
            _repository = repository;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComments([FromRoute] int id)
        {
            return Ok(await _repository.GetAll(id));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SendComment([FromBody] Comment comment, [FromRoute] int id)
        {
            var newViewModel = new CommentsEditViewModel
            {
                Name = comment.Name,
                Text = comment.Text,
                ParentPostId = id
            };
            await _repository.Add(User, newViewModel);
            return Ok();

        }
    }
}
