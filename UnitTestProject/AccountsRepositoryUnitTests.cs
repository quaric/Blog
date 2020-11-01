using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Data;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace UnitTestProject
{
    public abstract class AccountsRepositoryUnitTests
    {
        private IAccountsRepository _repository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<IConfigurationSection> _confSection;
        private Mock<IConfiguration> _conf;
        private string username = "admin@admin.com";
        private string password = "123@Lolol";
        protected AccountsRepositoryUnitTests(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Initialize();
        }
        protected DbContextOptions<ApplicationDbContext> ContextOptions { get; }


        public void Initialize()
        {
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            _mockSignInManager = MockHelpers.MockSignInManager<ApplicationUser>(_mockUserManager.Object);
            _conf = new Mock<IConfiguration>();
            var user = new ApplicationUser
            {
                UserName = username,
                Email = username
            };
            _mockUserManager.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
            _conf = new Mock<IConfiguration>();
            _confSection = new Mock<IConfigurationSection>();

        }

        [Fact]
        public async Task CanVerifyCredentials()
        {
            //Arrange
            _repository = new AccountsRepository(_mockSignInManager.Object, _mockUserManager.Object, _conf.Object);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, password, false, true))
                .ReturnsAsync(SignInResult.Success);
            User user = new User
            {
                Username = username,
                Passwd = password
            };
            //Act
            var result = await _repository.VerifyCredentials(user);
            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task VerifyCredentialsReturnsNullOnIncorrectPassword()
        {
            //Arrange
            _repository = new AccountsRepository(_mockSignInManager.Object, _mockUserManager.Object, _conf.Object);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, "wrongpasswd", false, true))
                .ReturnsAsync(SignInResult.Failed);
            User user = new User
            {
                Username = username,
                Passwd = "wrongpasswd"
            };
            //Act
            User result = await _repository.VerifyCredentials(user);
            //Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task CanGenerateJWT()
        {
            //Arrange
            _repository = new AccountsRepository(_mockSignInManager.Object, _mockUserManager.Object, _conf.Object);
            //_conf.SetupGet(x => x[It.Is<string>(s => s == "TokenSettings:SecretKey")]).Returns("secretkeyvalueover16bits");
            _confSection.SetupGet(m => m[It.Is<string>(s => s == "SecretKey")]).Returns("secretkeyvalueover16bits");
            _conf.Setup(a => a.GetSection(It.Is<string>(s => s == "TokenSettings"))).Returns(_confSection.Object);
            User user = new User
            {
                Username = username,
                Passwd = password,
                Id="12"
            };
            //Act
            string result =  _repository.GenerateJwtToken(user);
            
            //Assert
            Assert.NotNull(result);
        }
    }
}
