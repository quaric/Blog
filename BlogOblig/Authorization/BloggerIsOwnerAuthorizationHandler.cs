using System.Threading.Tasks;
using BlogOblig.Authorization;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BlogOblig.Authorization
{
    public class BloggerIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, BlogEditViewModel>
    {
        UserManager<IdentityUser> _userManager;

        public BloggerIsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, BlogEditViewModel resource)
        {
            if (context.User == null || resource == null)
            {
                // Return Task.FromResult(0) if targeting a version of
                // .NET Framework older than 4.6:
                return Task.CompletedTask;
            }

            if (resource.Status == BlogEditViewModel.BlogStatus.Open && requirement.Name == Constants.OpenOperationName)
            {
                context.Succeed(requirement);
            }
            // If we're not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName &&
                requirement.Name != Constants.OpenOperationName &&
                requirement.Name != Constants.ClosedOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}