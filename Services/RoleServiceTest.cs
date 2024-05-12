using AutoMapper;
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

namespace WizardFormBackend.Tests.Services
{
    public class RoleServiceTest
    {

        [Fact]
        public async Task GetRolesAsync_ShouldReturnRoleDtoList()
        {
            // Arrange
            var mockRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            List<RoleDto> roleDtos = new List<RoleDto>()
            {
                new()
                {
                    RoleId = (int)Constants.Roles.User,
                    RoleType = "user"
                },
                new()
                {
                    RoleId = (int)Constants.Roles.Admin,
                    RoleType = "admin"
                }
            };
            IEnumerable<Role> roles = new List<Role>()
            {
                 new()
                {
                    RoleId = (int)Constants.Roles.User,
                    RoleType = "user"
                },
                new()
                {
                    RoleId = (int)Constants.Roles.Admin,
                    RoleType = "admin"
                }
            };
            mockRepo.Setup(_ => _.GetAllRoleAsync()).ReturnsAsync(roles);
            mockMapper.Setup(_ => _.Map<IEnumerable<Role>, List<RoleDto>>(roles)).Returns(roleDtos);
            RoleService sut = new RoleService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.GetRolesAsync();

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(_ => _.GetAllRoleAsync(), Times.Once);
            mockMapper.Verify(_ => _.Map<IEnumerable<Role>, List<RoleDto>>(roles), Times.Once);
            Assert.Equal(roleDtos.Count, result.Count());
            Assert.Equal(roleDtos, result);
        }


        [Fact]
        public async Task AddRoleAsync_ShouldReturnRoleDto()
        {
            // Arrange
            var mockRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            RoleDto roleDto = new RoleDto()
            {
                RoleId = (int)Constants.Roles.User,
                RoleType = "user"
            };
            Role role = new Role()
            {
                RoleId = (int)Constants.Roles.User,
                RoleType = "user"
            };
            mockRepo.Setup(_ => _.AddRoleAsync(role)).ReturnsAsync(role);
            mockMapper.Setup(_ => _.Map<Role, RoleDto>(role)).Returns(roleDto);
            mockMapper.Setup(_ => _.Map<RoleDto, Role>(roleDto)).Returns(role);
            RoleService sut = new RoleService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.AddRoleAsync(roleDto);

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(_ => _.AddRoleAsync(role), Times.Once);
            mockMapper.Verify(_ => _.Map<Role, RoleDto>(role), Times.Once);
            mockMapper.Verify(_ => _.Map<RoleDto, Role>(roleDto), Times.Once);
            Assert.Equal(roleDto, result);
        }


        [Fact]
        public async Task GetRoleTypeAsync_ShouldReturnRoleType_WhenRoleIdIsValid()
        {
            // Arrange
            int roleId = (int)Constants.Roles.User;
            var mockRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            Role role = new Role() { RoleId = roleId, RoleType = "user" };
            mockRepo.Setup(_ => _.GetRoleByRoleIdAsync(roleId)).ReturnsAsync(role);
            RoleService sut = new RoleService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.GetRoleTypeAsync(roleId);

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(_ => _.GetRoleByRoleIdAsync(roleId), Times.Once);
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetRoleTypeAsync_ShouldReturnEmptyString_WhenRoleIdIsInValid()
        {
            // Arrange
            int roleId = (int)Constants.Roles.User;
            var mockRepo = new Mock<IRoleRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            Role role = new Role() { RoleId = roleId, RoleType = "user" };
            mockRepo.Setup(_ => _.GetRoleByRoleIdAsync(roleId)).ReturnsAsync((Role)null);
            RoleService sut = new RoleService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.GetRoleTypeAsync(roleId);

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(_ => _.GetRoleByRoleIdAsync(roleId), Times.Once);
            Assert.IsType<string>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldDeleteRole()
        {
            // Arrange
            int roleId = (int)Constants.Roles.User;
            var mockRepo = new Mock<IRoleRepository>();
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            Role role = new Role() { RoleId = roleId, RoleType = "user" };
            mockRepo.Setup(_ => _.GetRoleByRoleIdAsync(roleId)).ReturnsAsync(role);
            mockRepo.Setup(_ => _.DeleteRoleAsync(role));
            RoleService sut = new RoleService(mockRepo.Object, mockMapper.Object);

            // Act
            await sut.DeleteRoleAsync(roleId);

            // Assert
            mockRepo.Verify(_ => _.GetRoleByRoleIdAsync(roleId), Times.Once);
            mockRepo.Verify(_ => _.DeleteRoleAsync(role), Times.Once);
        }
    }
}
