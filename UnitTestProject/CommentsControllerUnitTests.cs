using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class CommentsControllerUnitTests
    {

        private Mock<ICommentRepository> _mockRepository;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private ICollection<Comment> _fakeComments;
        private CommentsEditViewModel _viewModel;
        private BlogEditViewModel _fakeBlog;

        [TestInitialize]
        public void SetupContext()
        {
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            _mockRepository = new Mock<ICommentRepository>();
            _fakeComments = new List<Comment>
            {
                new Comment
                {
                    Name = "enComment", Text = "Tekst", Created = DateTime.Now, Modified = DateTime.Now, CommentId = 1

                }
            };
            _viewModel = new CommentsEditViewModel
            {
                CommentId = 1,
                Name = "Moq",
                Text = "EnMoqTestComment",
                OwnerId = "userId",
                Created = DateTime.Now,
                ParentPostId = 1
            };
            _fakeBlog = new BlogEditViewModel
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
        public async Task IndexShouldShowAllComments()
        {
            //arrange
            _mockRepository.Setup(x => x.GetAll(1)).ReturnsAsync(_fakeComments);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            //Act
            var result = await controller.Index(1) as ViewResult;

            //Assert
            CollectionAssert.AllItemsAreInstancesOfType((ICollection)result.ViewData.Model, typeof(Comment));
            Assert.IsNotNull(result, "view result is null");
            var Comments = result.ViewData.Model as List<Comment>;
            Assert.AreEqual(1, Comments.Count, "Got wrong number of products");
        }

        [TestMethod]
        public async Task CreateShouldReturnViewForAuthorizedUser()
        {
            //Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Create", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            _mockRepository.Setup(x => x.GetBlog(1)).Returns(_fakeBlog);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var result = await controller.Create(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }
        [TestMethod]
        public async Task CreateShouldShowLoginViewForUnauthorizedUser()
        {
            //Arrange
            //Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Create", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            _fakeBlog.Status = BlogEditViewModel.BlogStatus.Closed;
            _mockRepository.Setup(x => x.GetBlog(1)).Returns(_fakeBlog);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Create(1) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void AddIsAddedOnCreateComment()
        {
            //Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Create", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            _mockRepository.Setup(x => x.GetBlog(1)).Returns(_fakeBlog);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var result = controller.Create(_viewModel);

            //Assert
            _mockRepository.VerifyAll();
            _mockRepository.Verify(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<CommentsEditViewModel>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task CreateCommentRedirectsToIndexOnSuccess()
        {
            //Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, BloggerIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, BlogAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Create", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            _mockRepository.Setup(x => x.GetBlog(1)).Returns(_fakeBlog);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var result = await controller.Create(_viewModel) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(result.RouteValues["controller"] == "Comments" && result.RouteValues["action"] == "Index");
        }
        [TestMethod]
        public async Task CreateCommentReturnsViewModelOnInvalidModel()
        {
            //Arrange
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<CommentsEditViewModel>()));
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ModelState.AddModelError("error", "fakeerror");
            //act

            var result = await controller.Create(_viewModel) as BadRequestResult;
            // Assert
            Assert.IsNotNull(result, "Expected bad request object result");
        }



        [TestMethod]
        public async Task CreateCommentReturnsViewModelOnInvalidSave()
        {
            //Arrange
            _mockRepository.Setup(x => x.GetBlog(It.IsAny<int>())).Throws(new Exception());
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);


            //Act
            var result = await controller.Create(_viewModel) as NotFoundResult;


            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task EditGetRequestInvalidIdReturnsNotFound()
        {
            _mockRepository.Setup(x => x.Get(It.IsAny<int>()));


            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Edit(It.IsAny<int>()) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task EditGetRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(_fakeComments.ElementAt(0));
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Edit(It.IsAny<int>()) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task EditGetRequestReturnsView_Authorized()
        {
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(_fakeComments.ElementAt(0));
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var result = await controller.Edit(1) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task EditCommentInvalidIDReturnsNotFound()
        {
            //arrange
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.Edit(2, _viewModel);

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task EditCommentRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Add(It.IsAny<ClaimsPrincipal>(), It.IsAny<CommentsEditViewModel>()));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Acts
            var result = await controller.Edit(1, _viewModel) as ChallengeResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task EditCommentRequestValidModelReturnsActionResult_Authorized()
        {
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            //Act
            var result = await controller.Edit(1, _viewModel) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.RouteValues["action"] == "Index");
        }
        [TestMethod]
        public async Task EditCommentRequestInvalidModelReturnsActionResult_Authorized()
        {
            var viewModel = new CommentsEditViewModel
            {
                CommentId = 1,
                Name = "",
                OwnerId = "userId"
            };
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(viewModel);

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);
            controller.ModelState.AddModelError("error", "error");
            //Act
            var result = await controller.Edit(1, viewModel) as BadRequestResult;

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task UpdateIsCalledFromEditComment_Authorized()
        {
            //Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Update", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var res = await controller.Edit(1, _viewModel);

            // Assert
            _mockRepository.Verify(x => x.Update(It.IsAny<CommentsEditViewModel>()));
        }
        [TestMethod]
        public async Task DeleteGetRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeComments.ElementAt(0));
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(false);

            //Act
            var result = await controller.Delete(1);

            //Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task DeleteGetRequestReturnsView_Authorized()
        {
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeComments.ElementAt(0));
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");

            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });


            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
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
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.Delete(2) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task DeleteCommentInvalidIDReturnsNotFound()
        {
            //arrange
            var _mockAuthService = new Mock<IAuthorizationService>();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), _viewModel, "Update"))
                .ReturnsAsync(AuthorizationResult.Success);
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, _mockAuthService.Object);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //act
            var result = await controller.DeleteConfirmed(2) as NotFoundResult;

            //Assert
            Assert.IsNotNull(result);

        }
        [TestMethod]
        public async Task DeleteCommentRequestReturnsLoginView_Unauthorized()
        {
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeComments.ElementAt(0));
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
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
            var Comment = _fakeComments.ElementAt(0);
            _mockRepository.Setup(x => x.Get(1)).Returns(_fakeComments.ElementAt(0));
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("userId");
            _mockRepository.Setup(x => x.GetViewModel(1)).Returns(_viewModel);
            var authService = BuildAuthorizationService(services =>
            {
                services.AddScoped(sp => _mockUserManager.Object);
                services.AddScoped<IAuthorizationHandler, CommentIsOwnerAuthorizationHandler>();
                services.AddScoped<IAuthorizationHandler, CommentAdministratorsAuthorizationHandler>();

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Delete", policy => policy.Requirements.Add(new OperationAuthorizationRequirement()));
                });
            });
            var controller = new CommentsController(_mockRepository.Object, _mockUserManager.Object, authService);
            controller.ControllerContext = MockHelpers.FakeControllerContext(true);

            //Act
            var res = await controller.DeleteConfirmed(1);

            // Assert
            _mockRepository.Verify(x => x.Delete(1));
        }

    }

}


