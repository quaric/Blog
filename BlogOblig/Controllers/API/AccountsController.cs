using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using BlogOblig.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogOblig.Controllers.API
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsRepository _repo;

        public AccountsController(IAccountsRepository repo)
        {
            _repo = repo;
        }


        /// <summary>
        ///     Logger inn bruker ved korrekt brukernavn og passord
        /// </summary>
        /// <example>
        ///     POST:
        ///     {
        ///         "username": "admin@admin.com"
        ///         "passwd": "123@Lolol"
        ///     }
        /// </example>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("verifyLogin")]
        public async Task<IActionResult> VerifyLogin([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User res = await _repo.VerifyCredentials(user);

            if (res == null)
            {
                return Ok(new {res = "Brukernavn/Passord er feil"}); 
            }

            return Ok(new { token = _repo.GenerateJwtToken(res)});
        }
    }
}
