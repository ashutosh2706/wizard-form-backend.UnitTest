using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Context;
using WizardFormBackend.Data.Models;
using WizardFormBackend.Data.Repositories;
using WizardFormBackend.Tests.MockData;

namespace WizardFormBackend.Tests.Repositories
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly WizardFormDbContext _dbContext;
        public UserRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<WizardFormDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _dbContext = new WizardFormDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllUserAsync_ShouldReturnUserList()
        {
            _dbContext.AddRange(UserMockData.UserList());
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);
            var result = await sut.GetAllUserAsync("");
            result.Should().NotBeNull();
            result.Should().HaveCount(UserMockData.UserList().Count);

        }

        [Fact]
        public async Task GetUserByUserIdAsync_ShouldReturnUser_WhenUserIdIsValid()
        {
            _dbContext.AddRange(UserMockData.UserList());
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);
            var result = await sut.GetUserByUserIdAsync(1);
            result.Should().NotBeNull();
            result.Should().BeOfType<User>();
            result?.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetUserByUserIdAsync_ShouldReturnNull_WhenUserIdIsInValid()
        {
            _dbContext.AddRange(UserMockData.UserList());
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);
            var result = await sut.GetUserByUserIdAsync(-1);
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            User mockUser = new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true };

            var sut = new UserRepository(_dbContext);

            var result = await sut.AddUserAsync(mockUser);

            result.Should().NotBeNull();
            var addedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == mockUser.UserId);
            addedUser.Should().NotBeNull();
            addedUser?.UserId.Should().Be(mockUser.UserId);
            addedUser?.Email.Should().Be($"{mockUser.Email}");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserInDatabase()
        {
            User mockUser = new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true };
            _dbContext.Users.Add(mockUser);
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);

            mockUser.FirstName = "New Name";
            mockUser.Email = "new_email@x.y";
            mockUser.Password = "new_password";

            await sut.UpdateUserAsync(mockUser);
            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserId == mockUser.UserId);
            updatedUser.Should().NotBeNull();
            updatedUser?.FirstName.Should().Be(mockUser?.FirstName);
            updatedUser?.Email.Should().Be(mockUser?.Email);
            updatedUser?.Password.Should().Be(mockUser?.Password);
        }


        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUserFromDatabase()
        {
            User mockUser = new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true };
            _dbContext.Users.Add(mockUser);
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);

            await sut.DeleteUserAsync(mockUser);

            var deletedUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.UserId == mockUser.UserId);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenEmailIsValid()
        {
            User mockUser = new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true };
            _dbContext.Users.Add(mockUser);
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);

            var result = await sut.GetUserByEmailAsync(mockUser.Email);
            result.Should().NotBeNull();
            result.Should().BeOfType<User>();
            result?.Email.Should().Be(mockUser.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenEmailIsInValid()
        {
            User mockUser = new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true };
            _dbContext.Users.Add(mockUser);
            await _dbContext.SaveChangesAsync();

            var sut = new UserRepository(_dbContext);

            var result = await sut.GetUserByEmailAsync("test@xyz");
            result.Should().BeNull();
        }


        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
