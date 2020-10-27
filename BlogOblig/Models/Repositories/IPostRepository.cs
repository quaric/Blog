using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;

namespace BlogOblig.Models
{
    public interface IPostRepository
    {
        public IEnumerable<Post> GetAll(int? id);
        public Post Get(int? id);
        public PostEditViewModel GetViewModel(int id);

        public Task Add(IPrincipal p, PostEditViewModel viewModels);
        public void Update(PostEditViewModel viewModel);
        public void Delete(int? id);
        public BlogEditViewModel GetBlog(int id);

    }
}
