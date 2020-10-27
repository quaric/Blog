using System.Threading.Tasks;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BlogOblig.Authorization
{
    public class CommentAdministratorsAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CommentsEditViewModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CommentsEditViewModel resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.BlogAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}