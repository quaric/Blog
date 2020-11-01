using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;

namespace BlogOblig.Models
{
    public interface ICommentRepository
    {
        public Task<IEnumerable<Comment>> GetAll(int? id);
        public Comment Get(int? id);
        public CommentsEditViewModel GetViewModel(int id);
        public Task Add(IPrincipal p, CommentsEditViewModel viewModel);
        public void Update(CommentsEditViewModel viewModel);
        public void Delete(int? id);
        public BlogEditViewModel GetBlog(int id);
    }
    }

