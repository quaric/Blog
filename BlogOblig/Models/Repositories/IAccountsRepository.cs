using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;

namespace BlogOblig.Models.Repositories
{
    public interface IAccountsRepository
    {
        Task<User> VerifyCredentials(User user);
        string GenerateJwtToken(User user);
    }
}
