using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Controllers;
using WizardFormBackend.Data.Dto;
using WizardFormBackend.Data.Models;
using WizardFormBackend.Services;
using WizardFormBackend.Tests.MockData;
using WizardFormBackend.Utils;

namespace WizardFormBackend.Tests.Controllers
{
    public class UsersControllerTest
    {
        private readonly UsersController _sut;
        private readonly Mock<IUserService> _userServiceMock;
        public UsersControllerTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _sut = new UsersController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnStatus200()
        {
            // Arrange
            var queryParams = new QueryParams
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 10,
                SortField = "fieldName",
                SortDirection = "ascending",
            };

            var pagedResponseDtoMock = new PagedResponseDto<UserResponseDto>
            {
                PageNumber = 1,
                PageSize = 10,
                TotalPage = 1,
                Items = new List<UserResponseDto> { new UserResponseDto { UserId = 1, RoleId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", IsAllowed = "Allowed" } }
            };

            _userServiceMock.Setup(_ => _.GetUsersAsync(queryParams.SearchTerm, queryParams.PageNumber, queryParams.PageSize)).ReturnsAsync(pagedResponseDtoMock);

            // Act
            var actionResult = await _sut.GetUsers(queryParams);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.GetUsersAsync(queryParams.SearchTerm, queryParams.PageNumber, queryParams.PageSize), Times.Once);
            Assert.IsType<OkObjectResult>(actionResult);
            var output = actionResult as OkObjectResult;
            Assert.NotNull(output);
            Assert.Equal(200, output.StatusCode);
            Assert.Equal(pagedResponseDtoMock, output.Value);
            
        }

        [Fact]
        public async Task LoginUser_ReturnStatus200_WhenLoginCredentialsAreCorrect()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@abc.z",
                Password = "password",
            };

            String token = "token";

            _userServiceMock.Setup(_ => _.AuthenticateUserAsync(loginDto)).ReturnsAsync(token);

            // Act
            var actionResult = await _sut.LoginUser(loginDto);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AuthenticateUserAsync(loginDto), Times.Once);
            Assert.IsType<OkObjectResult>(actionResult);
            var output = actionResult as OkObjectResult;
            Assert.NotNull(output);
            Assert.Equal(200, output.StatusCode);
            Assert.Equal(token, output.Value);
        }

        [Fact]
        public async Task LoginUser_ReturnStatus401_WhenAccountNotActive()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@abc.z",
                Password = "password",
            };

            string token = "";

            _userServiceMock.Setup(_ => _.AuthenticateUserAsync(loginDto)).ReturnsAsync(token);

            // Act
            var actionResult = await _sut.LoginUser(loginDto);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AuthenticateUserAsync(loginDto), Times.Once);
            Assert.IsType<UnauthorizedResult>(actionResult);
            var output = actionResult as UnauthorizedResult;
            Assert.NotNull(output);
            Assert.Equal(401, output.StatusCode);
        }

        [Fact]
        public async Task LoginUser_ReturnStatus400_WhenLoginCredentialsAreIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@abc.z",
                Password = "password",
            };

            _userServiceMock.Setup(_ => _.AuthenticateUserAsync(loginDto));

            // Act
            var actionResult = await _sut.LoginUser(loginDto);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AuthenticateUserAsync(loginDto), Times.Once);
            Assert.IsType<BadRequestResult>(actionResult);
            var output = actionResult as BadRequestResult;
            Assert.NotNull(output);
            Assert.Equal(400, output.StatusCode);
        }


        [Fact]
        public async Task RegisterUser_ReturnStatus201()
        {
            // Arrange
            UserDto userDtoMock = new()
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@x.y",
                Password = "password",
                RoleId = 1,
                IsActive = false,
            };

            User userMock = new()
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@x.y",
                Password = "password",
                RoleId = 2,
                Active = true
            };

            _userServiceMock.Setup(_ => _.AddUserAsync(userDtoMock)).ReturnsAsync(userMock);

            // Act
            var actionResult = await _sut.RegisterUser(userDtoMock);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AddUserAsync(userDtoMock), Times.Once);
            Assert.IsType<CreatedResult>(actionResult);
            var output = actionResult as CreatedResult;
            Assert.NotNull(output);
            Assert.Equal(201, output.StatusCode);
            Assert.Equal(userMock, output.Value);
        }

        [Fact]
        public async Task AllowUser_ReturnStatus200_WhenUserIdIsValid()
        {
            // Arrange
            _userServiceMock.Setup(_ => _.AllowUserAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _sut.AllowUser(1);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AllowUserAsync(1), Times.Once);
            Assert.IsType<OkResult>(actionResult);
            var output = (OkResult)actionResult;
            Assert.NotNull(output);
            Assert.Equal(200, output.StatusCode);
        }

        [Fact]
        public async Task AllowUser_ReturnStatus404_WhenUserIdIsInvalid()
        {
            // Arrange
            _userServiceMock.Setup(_ => _.AllowUserAsync(0)).ReturnsAsync(false);

            // Act
            var actionResult = await _sut.AllowUser(0);

            // Assert
            Assert.NotNull(actionResult);
            _userServiceMock.Verify(_ => _.AllowUserAsync(0), Times.Once);
            Assert.IsType<NotFoundResult>(actionResult);
            var output = (NotFoundResult)actionResult;
            Assert.NotNull(output);
            Assert.Equal(404, output.StatusCode);
        }

        [Fact]
        public async Task ChangeRole_ReturnStatus200()
        {
            // Arrange
            ChangeRoleDto changeRoleDtoMock = new()
            {
                UserId = 1,
                RoleId = 2,
            };

            _userServiceMock.Setup(_ => _.ChangeRoleAsync(1, 2));

            // Act
            var actionResult = await _sut.ChangeRole(changeRoleDtoMock);

            // Assert
            actionResult.GetType().Should().Be(typeof(OkResult));
            _userServiceMock.Verify(_ => _.ChangeRoleAsync(1, 2), Times.Once);
            (actionResult as OkResult)?.StatusCode.Should().Be(200);
        }


        [Fact]
        public async Task DeleteUser_ReturnStatus404_WhenUserIdIsValid()
        {
            // Arrange
            _userServiceMock.Setup(_ => _.DeleteUserAsync(1)).ReturnsAsync(true);

            // Act
            var actionResult = await _sut.DeleteUser(1);


            // Assert
            actionResult.GetType().Should().Be(typeof(NoContentResult));
            _userServiceMock.Verify(_ => _.DeleteUserAsync(1), Times.Once);
            (actionResult as NoContentResult)?.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteUser_ReturnStatus400_WhenUserIdIsInvalid()
        {
            _userServiceMock.Setup(_ => _.DeleteUserAsync(1)).ReturnsAsync(false);
            var result = await _sut.DeleteUser(1);

            result.GetType().Should().Be(typeof(NotFoundResult));
            _userServiceMock.Verify(_ => _.DeleteUserAsync(1), Times.Once);
            (result as NotFoundResult)?.StatusCode.Should().Be(404);
        }
    }
}
