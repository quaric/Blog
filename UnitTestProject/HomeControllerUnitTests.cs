using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Controllers;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTestProject
{
    public class HomeControllerUnitTests
    {
        [Fact]
        public async Task CanGetAllSubscriptions()
        {
            //Arrange
            Mock<IBlogRepository> repo = new Mock<IBlogRepository>();
            Mock<UserManager<ApplicationUser>> mgr = MockHelpers.MockUserManager<ApplicationUser>();
            Mock<ILogger<HomeController>> logger = new Mock<ILogger<HomeController>>();

            repo.Setup(x => x.GetBlogSubscriptions(It.IsAny<IPrincipal>()))
                .ReturnsAsync(() => new List<SubscriptionViewModel> {new SubscriptionViewModel()});
            var controller = new HomeController(logger.Object, repo.Object, mgr.Object);
            //Act
            var result = await controller.Index() as ViewResult;
            var subscriptions = result.ViewData.Model as List<SubscriptionViewModel>;
            //Assert
            Assert.NotNull(result);
            Assert.Single(subscriptions);
        }
    }
}
