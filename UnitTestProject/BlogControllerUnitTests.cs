using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogOblig.Authorization;
using BlogOblig.Controllers;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using BlogOblig.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class BlogControllerUnitTests
    {
        private Mock<IBlogRepository> _mockRepository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private List<Blog> _fakeBlogs;
        private BlogEditViewModel _viewModel;
        
        [TestInitialize]
        public void SetupContext()
        {
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            _mockRepository = new Mock<IBlogRepository>();
            _fakeBlogs = new List<Blog>
            {
                new Blog
                {
                    Name = "ENBLog", Description = "DESKRIPSJON", Created = DateTime.Now, Modified = DateTime.Now, BlogId = 1

                },
                new Blog
                {
                    Name = "TOBLog", Description = "TODESKRIPSJON", Created = DateTime.Now, Modified = DateTime.Now, BlogId = 2
                }
            };
            _viewModel = new BlogEditViewModel
            {
                BlogId = 1,
                Name = "Moq",
                Description = "EnMoqTestBlog",
                OwnerId = "userId",
                Created = DateTime.Now,
                Status = BlogEditViewModel.BlogStatus.Open
            };
        }
        private IAuthorizationService BuildAuthorizationService(Action<IServiceCollection> setupServices = null)
        {
            var services = new ServiceCollection();
            services.AddAuthorization();
            services.AddLogging();
            services.AddOptions();
            setupServices?.Invoke(services);
            return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        }

        [TestMethod]
        public async Task IndexShouldShowAllBlogs()
        {
            //arrange
            _mockRepository.Setup(x => x.GetAll()).Returns(_fakeBlogs);
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            //Act
            var result = await controller.Index() as ViewResult;

            //Assert
            CollectionAssert.AllItemsAreInstancesOfType((ICollection)result.ViewData.Model, typeof(Blog));
            Assert.IsNotNull(result, "view result is null");
            var blogs = result.ViewData.Model as List<Blog>;
            Assert.AreEqual(2, blogs.Count, "Got wrong number of products");
        }

        [TestMethod]
        public void CreateShouldReturnViewForAuthorizedUser()
        {
            //Arrange
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var result = controller.Create();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }
        [TestMethod]
        public void CreateShouldShowLoginViewForUnauthorizedUser()
        {
            //Arrange
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = controller.Create() as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }
        [TestMethod]
        public void BlogIsAddedOnCreatePOST()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(),It.IsAny<Blog>()));
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);


            //Act
            var result = controller.Create(_viewModel);

            //Assert
            _mockRepository.VerifyAll();
            _mockRepository.Verify(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task CreatePostRedirectsToIndexOnSuccess()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>()  ,It.IsAny<Blog>()));
            var controller = new BlogsController(_mockRepository.Object,_mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            
            //Act
            var result = await controller.Create(_viewModel) as RedirectToActionResult;

            //Assert
            Assert.IsNotNull(result, "view result is null");
            Assert.AreEqual("Index", result.ActionName);
        }
        [TestMethod]
        public async Task CreatePostReturnsViewModelOnInvalidModel()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var badModel = new BlogEditViewModel
            {
                Name = "", Description = ""
            };
            var validationContext = new ValidationContext(badModel, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(badModel, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }

            var result = await controller.Create(badModel) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result, "Expected bad request object result");
            Assert.IsInstanceOfType(result.Value, typeof(SerializableError));
        }



        [TestMethod]
        public async Task CreatePostReturnsViewModelOnInvalidSave()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>())).Throws(new Exception());
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);


            //Act
            var result = await controller.Create(_viewModel) as BadRequestObjectResult;


            //Assert
            Assert.IsNotNull(result, "Expected bad request object result");
            Assert.IsInstanceOfType(result.Value, typeof(string));

        }

        [TestMethod]
        public void CreateGetReturnsNotNullView()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            // act
            var result = (ViewResult)controller.Create();
            //assert
            Assert.IsNotNull(result, "view result is null");
        }

        [TestMethod]
        public async Task EditGetRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Edit(It.IsAny<int>()) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task EditGetRequestReturnsView_Authorized()
        {
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            
           
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            //Act
            var result = await controller.Edit(1) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }

        [TestMethod]
        public async Task EditPostInvalidIDReturnsNotFound()
        {
            //arrange
            var _mockAuthService = new Mock<IAuthorizationService>();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _viewModel, "Update"))
                .ReturnsAsync(AuthorizationResult.Success);
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, _mockAuthService.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.Edit(2, _viewModel) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task EditPostRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Acts
            var result = await controller.Edit(1, _viewModel);

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task EditPostRequestValidModelReturnsActionResult_Authorized()
        {
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            //Act
            var result = await controller.Edit(1, _viewModel) as RedirectToActionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ActionName == "Index");
        }
        [TestMethod]
        public async Task EditPostRequestInvalidModelReturnsActionResult_Authorized()
        {
            var viewModel = new BlogEditViewModel
            {
                BlogId = 1, Name = "", Description = "", OwnerId = "userId"
            };
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ModelState.AddModelError("fakeerror", "fakeerror");
            //Act
            var result = await controller.Edit(1, viewModel) as BadRequestResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task SaveChangesIsCalledFromEditPOST_Authorized()
        {
            //Arrange
            var blog = _fakeBlogs.ElementAt(0);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var res = await controller.Edit(1, _viewModel);

            // Assert
            _mockRepository.Verify(x => x.SaveChanges(It.IsAny<Blog>()));
        }
        [TestMethod]
        public async Task DeleteGetRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Delete(It.IsAny<int>()) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task DeleteGetRequestReturnsView_Authorized()
        {
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            //Act
            var result = await controller.Delete(1) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }

        [TestMethod]
        public async Task DeleteGetInvalidIDReturnsNotFound()
        {
            //arrange
            var _mockAuthService = new Mock<IAuthorizationService>();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _viewModel, "Update"))
                .ReturnsAsync(AuthorizationResult.Success);
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, _mockAuthService.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.Delete(2) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task DeletePostInvalidIDReturnsNotFound()
        {
            //arrange
            var _mockAuthService = new Mock<IAuthorizationService>();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _viewModel, "Update"))
                .ReturnsAsync(AuthorizationResult.Success);
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, _mockAuthService.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.DeleteConfirmed(2) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task DeletePostRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<Blog>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Acts
            var result = await controller.DeleteConfirmed(1) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteFromRepoIsCalledOnDeleteConfirmedCall_Authorized()
        {
            //Arrange
            var blog = _fakeBlogs.ElementAt(0);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            _mockRepository.Setup(x => x.GetBlogEditViewModel(1)).Returns(_viewModel);
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var res = await controller.DeleteConfirmed(1);

            // Assert
            _mockRepository.Verify(x => x.Delete(1));
        }

        [TestMethod]
        public async Task RedirectsToPostsOnCorrectId()
        {
            //Arrange
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeBlogs.ElementAt(0));
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var res = await controller.RedirectToBlog(1) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(res);
            Assert.IsTrue(res.RouteValues["controller"] == "Posts" && res.RouteValues["action"] == "Index");
        }
        [TestMethod]
        public async Task RedirectsToPostsOnInvalidId()
        {
            //Arrange
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeBlogs.ElementAt(0));
            var controller = new BlogsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var res = await controller.RedirectToBlog(3) as NotFoundResult;

            //Assert
            Assert.IsNotNull(res);
        }
    }
    
}

