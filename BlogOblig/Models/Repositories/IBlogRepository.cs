using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;

namespace BlogOblig.Models
{
    public interface IBlogRepository
    {
        public IEnumerable<Blog> GetAll();
        public Blog Get(int id);
        public Task<List<SubscriptionViewModel>> GetBlogSubscriptions(IPrincipal principal);

        public Task Add(IPrincipal principal, Blog blog);
        public void SaveChanges(Blog blog);
        public void Delete(int id);
        public BlogEditViewModel GetBlogEditViewModel();
        public BlogEditViewModel GetBlogEditViewModel(int id);

        public Task Subscribe(IPrincipal principal, int id);
    }
}
