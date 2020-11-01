using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Hubs;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BlogOblig.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : Controller
    {
        private ICommentRepository _repository;
        private IHubContext<CommentsHub> _hubContext;

        public CommentsController(ICommentRepository repository, IHubContext<CommentsHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        /// <summary>
        ///     Returnerer alle kommentarer til en gitt Post. PostId(int) må inkluderes i request.
        /// </summary>
        /// <example>
        /// Eksempel request:
        ///     GET
        ///      {
        ///         "id": 1
        ///      }
        /// </example>
        /// <param name="id">Parent Post Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComments([FromRoute] int id)
        {
            return Ok(await _repository.GetAll(id));
        }


        /// <summary>
        ///      Legger til en kommentar.
        /// </summary>
        /// <example>
        ///Eksempel Request:
        ///     POST:
        ///     {
        ///        "name": "tittel"
        ///        "Text": "kommentartekst"
        ///     }
        /// </example>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> SendComment([FromBody] Comment comment, [FromRoute] int id)
        {
            var newViewModel = new CommentsEditViewModel
            {
                Name = comment.Name,
                Text = comment.Text,
                ParentPostId = id
            };
            //TODO LEGG TIL fail på add
            await _repository.Add(User, newViewModel);
            await _hubContext.Clients.All.SendAsync("ReceiveComment", comment);
            return Ok();

        }
    }
}
