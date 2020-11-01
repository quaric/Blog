using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Controllers.API;
using BlogOblig.Models.Entities;
using BlogOblig.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Xunit;

namespace UnitTestProject
{
    public class AccountsControllerAPIUnitTests
    {
        [Fact]
        public async Task UnauthorizedResultOnInvalidCredentials()
        {
            //Arrange
            var _repository = new Mock<IAccountsRepository>();
            var controller = new AccountsController(_repository.Object);
            User user = new User
            {
                Username="username",
                Passwd = "pass"
            }; 
            //Act
            var result = await controller.VerifyLogin(user);

            //Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task OkResultOnCorrectCredentials()
        {
            //Arrange
            var repository = new Mock<IAccountsRepository>();
            User user = new User
            {
                Username = "username",
                Passwd = "pass",
                Id = "2"
            };
            repository.Setup(x => x.VerifyCredentials(user)).ReturnsAsync(user);
            var controller = new AccountsController(repository.Object);

            //Act
            var result = await controller.VerifyLogin(user);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
