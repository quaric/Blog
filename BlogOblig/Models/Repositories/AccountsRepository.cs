using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
/*
 * Author: kc
 */
namespace BlogOblig.Models.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _conf;

        public AccountsRepository(SignInManager<ApplicationUser> _manager, UserManager<ApplicationUser> _userManager, IConfiguration conf)
        {
            _conf = conf;
            this._signInManager = _manager;
            this._userManager = _userManager;
        }
        public async Task<User> VerifyCredentials(User user)
        {
            if (user.Username == null || user.Passwd == null || user.Username.Length == 0 || user.Passwd.Length == 0)
            {
                return null;
            }

            var thisUser = await _userManager.FindByNameAsync(user.Username);
            if (thisUser == null)
                return (null);

            var result = await _signInManager.PasswordSignInAsync(user.Username, user.Passwd, false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return null;
            }

            var role = await _userManager.GetRolesAsync(thisUser);
            return new User() { Id = thisUser.Id, Username = user.Username };
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var confKey = _conf.GetSection("TokenSettings")["SecretKey"];
            var key = Encoding.ASCII.GetBytes(confKey);
            var cIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                });

            //claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = cIdentity,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
