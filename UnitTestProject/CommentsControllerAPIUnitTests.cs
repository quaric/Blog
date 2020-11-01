using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlogOblig.Controllers.API;
using BlogOblig.Hubs;
using BlogOblig.Models;
using BlogOblig.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace UnitTestProject
{
    public class CommentsControllerAPIUnitTests
    {
        [Fact]
        public async Task OkResultOnGetAllComments()
        {
            //Arrange
            var repository = new Mock<ICommentRepository>();
            repository.Setup(x => x.GetAll(It.IsAny<int>())).ReturnsAsync(() => new List<Comment> { new Comment(), new Comment() });
            var commentsHub = new Mock<IHubContext<CommentsHub>>();
            var controller = new CommentsController(repository.Object, commentsHub.Object);

            //Act
            var result = await controller.GetComments(It.IsAny<int>());

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task OkResultOnSendComment()
        {
            //Arrange
            var repository = new Mock<ICommentRepository>();
            var commentsHub = new Mock<IHubContext<CommentsHub>>();
            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            commentsHub.Setup(x=>x.Clients).Returns(() => mockClients.Object);


            var controller = new CommentsController(repository.Object, commentsHub.Object);
            Comment comment = new Comment()
            {
                Name = "Name",
                Text = "Text"
            };
            
            //Act
            var result = await controller.SendComment(comment, It.IsAny<int>());

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
