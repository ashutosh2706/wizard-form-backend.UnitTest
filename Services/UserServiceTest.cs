using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Dto;
using WizardFormBackend.Data.Models;
using WizardFormBackend.Data.Repositories;
using WizardFormBackend.Services;
using WizardFormBackend.Utils;

namespace WizardFormBackend.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserService _sut;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _roleServiceMock = new Mock<IRoleService>();
            _configurationMock = new Mock<IConfiguration>();
            _sut = new UserService(_userRepositoryMock.Object, _roleServiceMock.Object, _configurationMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnPaginatedResponseOfTypeUserResponseDto()
        {
            IEnumerable<User> mockUsers = new List<User>()
            {
                new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1, Active = true },
                new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1, Active = true }
            };

            _userRepositoryMock.Setup(_ => _.GetAllUserAsync("test")).ReturnsAsync(mockUsers);

            var response = await _sut.GetUsersAsync("test", 1, 10);
            response.Should().NotBeNull();
            response.Should().BeOfType<PagedResponseDto<UserResponseDto>>();
            response.PageNumber.Should().Be(1);
            response.PageSize.Should().Be(10);
            response.Items.Should().NotBeNull().And.HaveCount(2);
        }

        [Fact]
        public async Task AddUserAsync_ShouldReturnUser()
        {
            UserDto mockUserDto = new() { FirstName = "Test", LastName = "Test", Email = "test@x.y", Password = "password", RoleId = 1 };
            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1 };

            _mapperMock.Setup(_ => _.Map<UserDto, User>(mockUserDto)).Returns(mockUser);
            _userRepositoryMock.Setup(_ => _.AddUserAsync(mockUser)).ReturnsAsync(mockUser);

            var response = await _sut.AddUserAsync(mockUserDto);
            response.Should().NotBeNull();
            response.Should().BeOfType<User>();
            response.Active.Should().BeFalse();
        }


        [Fact]
        public async Task AllowUserAsync_ShouldReturnTrue_WhenUserIdIsValid()
        {
            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1 };
            _userRepositoryMock.Setup(_ => _.GetUserByUserIdAsync(1)).ReturnsAsync(mockUser);

            var response = await _sut.AllowUserAsync(1);
            response.Should().BeTrue();
        }


        [Fact]
        public async Task AllowUserAsync_ShouldReturnFalse_WhenUserIdIsInValid()
        {
            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1 };
            _userRepositoryMock.Setup(_ => _.GetUserByUserIdAsync(2)).ReturnsAsync(mockUser);

            var response = await _sut.AllowUserAsync(1);
            response.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserIdIsValid()
        {
            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1 };
            _userRepositoryMock.Setup(_ => _.GetUserByUserIdAsync(1)).ReturnsAsync(mockUser);
            var response = await _sut.DeleteUserAsync(1);
            response.Should().BeTrue();

        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserIdIsInValid()
        {
            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1 };
            _userRepositoryMock.Setup(_ => _.GetUserByUserIdAsync(2)).ReturnsAsync(mockUser);
            var response = await _sut.DeleteUserAsync(1);
            response.Should().BeFalse();

        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnTokenString_WhenLoginCredentialIsValidAndUserIsAllowed()
        {
            LoginDto loginDtoMock = new() { Email = "abc@y.z", Password = "password" };
            var hashedPassword = Util.GenerateHash(loginDtoMock.Password);
            var roleType = "admin";

            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1, Active = true, Password = hashedPassword };

            _userRepositoryMock.Setup(_ => _.GetUserByEmailAsync(loginDtoMock.Email)).ReturnsAsync(mockUser);
            _roleServiceMock.Setup(_ => _.GetRoleTypeAsync(mockUser.RoleId)).ReturnsAsync(roleType);

            _configurationMock.Setup(_ => _["JWT:Key"]).Returns("test_key:109872345678!@#$%^&*()_+=+qwertymnbvcxzasdfghjklyuiopMNBVCXZASDFGHJKLPOIUYTREW12@!Q<<..,?;;:}}[[{{klkldklkdlkandthisisaverylongkeyooooo!!!!123");
            _configurationMock.Setup(_ => _["JWT:Issuer"]).Returns("Issuer");
            _configurationMock.Setup(_ => _["JWT:Audience"]).Returns("Audience");

            var response = await _sut.AuthenticateUserAsync(loginDtoMock);
            response.Should().NotBeNullOrEmpty();
            response.Should().BeOfType<string>();
        }


        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnEmptyString_WhenLoginCredentialIsValidAndUserIsNotAllowed()
        {
            LoginDto loginDtoMock = new() { Email = "abc@y.z", Password = "password" };
            var hashedPassword = Util.GenerateHash(loginDtoMock.Password);
            var roleType = "admin";

            User mockUser = new() { FirstName = "Test", LastName = "Test", Email = "test@y.z", UserId = 1, RoleId = 1, Active = false, Password = hashedPassword };

            _userRepositoryMock.Setup(_ => _.GetUserByEmailAsync(loginDtoMock.Email)).ReturnsAsync(mockUser);
            _roleServiceMock.Setup(_ => _.GetRoleTypeAsync(mockUser.RoleId)).ReturnsAsync(roleType);


            _configurationMock.Setup(_ => _["JWT:Key"]).Returns("test_key:109872345678!@#$%^&*()_+=+qwertymnbvcxzasdfghjklyuiopMNBVCXZASDFGHJKLPOIUYTREW12@!Q<<..,?;;:}}[[{{klkldklkdlkandthisisaverylongkeyooooo!!!!123");
            _configurationMock.Setup(_ => _["JWT:Issuer"]).Returns("Issuer");
            _configurationMock.Setup(_ => _["JWT:Audience"]).Returns("Audience");

            var response = await _sut.AuthenticateUserAsync(loginDtoMock);
            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }


        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnNull_WhenLoginCredentialIsInValid()
        {
            LoginDto loginDtoMock = new() { Email = "abc@y.z", Password = "password" };
            _userRepositoryMock.Setup(_ => _.GetUserByEmailAsync(loginDtoMock.Email));

            var response = await _sut.AuthenticateUserAsync(loginDtoMock);
            response.Should().BeNull();
        }
    }
}
