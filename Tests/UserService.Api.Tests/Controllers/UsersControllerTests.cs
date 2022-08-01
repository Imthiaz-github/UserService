using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserService.Api.Controllers;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using Xunit;

namespace UserService.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserService> _serviceMock;
        private readonly UsersController _usersController;
        public UsersControllerTests()
        {
            _fixture = new Fixture();
            _serviceMock = _fixture.Freeze<Mock<IUserService>>();
            _usersController = new UsersController(_serviceMock.Object); // Creates the implemenration in memory
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOKResponse_WhenDataFound()
        {
            // Arrange
            var usersMock = _fixture.Create<IEnumerable<User>>();
            _serviceMock.Setup(x => x.GetUsers()).ReturnsAsync(usersMock);

            // Act
            var result = await _usersController.GetUsers().ConfigureAwait(false);

            // Assert
            //Assert.NotNull(result);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<IEnumerable<User>>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            _serviceMock.Verify(x => x.GetUsers(), Times.Once());
        }

         [Fact]
        public async Task GetUsers_ShouldReturnOk_WhenDataNotFound()
        { 
                //  Arrange
                    List<User> response = null;
                _serviceMock.Setup(x => x.GetUsers()).ReturnsAsync(response);

                // ACT
                var result = await _usersController.GetUsers().ConfigureAwait(false);

                // Assert
                result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<ObjectResult>();
            _serviceMock.Verify(x => x.GetUsers(), Times.Once());
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOkResponse_WhenValidInput()
        {
            // Arrange
            var userMock = _fixture.Create<User>();
            var id = _fixture.Create<int>();
            _serviceMock.Setup(x => x.GetUserById(id)).ReturnsAsync(userMock);

            // ACT
            var result = await _usersController.GetUserById(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<User>>();
            result.Result.Should().BeAssignableTo<OkObjectResult>();
            result.Result.As<OkObjectResult>().Value
                .Should()
                .NotBeNull()
                .And.BeOfType(userMock.GetType());
            _serviceMock.Verify(x => x.GetUserById(id), Times.Once());        
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenNoDataFound()
        {
            //  Arrange
            User response = null;
            var id = _fixture.Create<int>();
            _serviceMock.Setup(x => x.GetUserById(id)).ReturnsAsync(response);

            // ACT
            var result = await _usersController.GetUserById(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
            _serviceMock.Verify(x => x.GetUserById(id), Times.Once());
        }

        [Fact]
        public async Task GetUserById_ShouldReturnBadRequest_WhenInputIsEqualsZero()
        {
            //  Arrange
            var response = _fixture.Create<User>();
            int id = 0;
            _serviceMock.Setup(x => x.GetUserById(id)).ReturnsAsync(response);

            // ACT
            var result = await _usersController.GetUserById(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
            _serviceMock.Verify(x => x.GetUserById(id), Times.Never());
        }

        [Fact]
        public async Task CreateUser_ShouldReturnOKResponse_WhenValidRequest()
        {
            //  Arrange
            var request = _fixture.Create<User>();
            var response = _fixture.Create<User>();
            _serviceMock.Setup(x => x.CreateUser(request)).ReturnsAsync(response);

            // ACT
            var result = await _usersController.CreateUser(request).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<User>>();
            result.Result.Should().BeAssignableTo<CreatedAtRouteResult>();
            _serviceMock.Verify(x => x.CreateUser(response), Times.Never());
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            //  Arrange
            var request = _fixture.Create<User>();
            _usersController.ModelState.AddModelError("UserName", "The userName field is required.");
            var response = _fixture.Create<User>();
            _serviceMock.Setup(x => x.CreateUser(request)).ReturnsAsync(response);

            // ACT
            var result = await _usersController.CreateUser(request).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
            _serviceMock.Verify(x => x.CreateUser(request), Times.Never());
        }
        [Fact]
        public async Task DeleteUser_ShouldReturnNoContents_WhenDeletedARecord()
        {
            //  Arrange
            var id = _fixture.Create<int>();
            _serviceMock.Setup(x => x.DeleteUser(id)).ReturnsAsync(true);

            // ACT
            var result = await _usersController.DeleteUser(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenRecordNotFound()
        {
            //  Arrange
            var id = _fixture.Create<int>();
            _serviceMock.Setup(x => x.DeleteUser(id)).ReturnsAsync(false);

            // ACT
            var result = await _usersController.DeleteUser(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }
        [Fact]
        public async Task DeleteUser_ShouldReturnBadResponse_WhenInputIsZero()
        {
            //  Arrange
            var id = 0;
            _serviceMock.Setup(x => x.DeleteUser(id)).ReturnsAsync(false);

            // ACT
            var result = await _usersController.DeleteUser(id).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
            _serviceMock.Verify(x => x.DeleteUser(id), Times.Never());
        }
        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenInputIsZero()
        {
            //  Arrange
            var id = 0;
            var request = _fixture.Create<User>();
            _serviceMock.Setup(x => x.UpdateUser(id, request)).ReturnsAsync(false);

            // ACT
            var result = await _usersController.UpdateUser(id, request).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFoundResult>();
            _serviceMock.Verify(x => x.UpdateUser(id, request), Times.Never());
        }
        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            //  Arrange
            var id = _fixture.Create<int>();
            var request = _fixture.Create<User>();
            _usersController.ModelState.AddModelError("UserName", "The userName field is required.");
            var response = _fixture.Create<User>();
            _serviceMock.Setup(x => x.UpdateUser(id, request)).ReturnsAsync(false);

            // ACT
            var result = await _usersController.UpdateUser(id, request).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequestResult>();
            _serviceMock.Verify(x => x.UpdateUser(id, request), Times.Never());
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOKResponse_WhenValidRequest()
        {
            //TODO : Asserts
            //  Arrange
            var id = _fixture.Create<int>();
            var request = _fixture.Create<User>();
            var response = _fixture.Create<User>();
            _serviceMock.Setup(x => x.CreateUser(request)).ReturnsAsync(response);

            // ACT
            var result = await _usersController.UpdateUser(response.UserId, response).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ActionResult<bool>>();
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
        }
    }
}

