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
    public class RequestRepositoryTest : IDisposable
    {
        private readonly WizardFormDbContext _dbContext;
        public RequestRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<WizardFormDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _dbContext = new WizardFormDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllRequestAsync_ShouldReturnRequestList()
        {
            // Arrabge
            _dbContext.AddRange(RequestMockData.RequestList());
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            var result = await sut.GetAllRequestAsync("");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(RequestMockData.RequestList().Count);
        }


        [Fact]
        public async Task GetRequestByRequestIdAsync_ShouldReturnRequest_WhenRequestIdIsValid()
        {
            // Arrange
            _dbContext.AddRange(RequestMockData.RequestList());
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            var result = await sut.GetRequestByRequestIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Request>();
            result?.RequestId.Should().Be(1);
        }

        [Fact]
        public async Task GetRequestByRequestIdAsync_ShouldReturnNull_WhenRequestIdIsInValid()
        {
            // Arrange
            _dbContext.AddRange(RequestMockData.RequestList());
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            var result = await sut.GetRequestByRequestIdAsync(-1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddRequestAsync_ShouldAddRequestToDatabase()
        {
            // Arrange
            Request mockRequest = new() { RequestId = 1, UserId = 1, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1 };
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            var result = await sut.AddRequestAsync(mockRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Request>();
            var addedRequest = await _dbContext.Requests.FirstOrDefaultAsync(r => r.RequestId == mockRequest.RequestId);
            addedRequest.Should().NotBeNull();
            addedRequest?.RequestId.Should().Be(mockRequest.RequestId);
        }

        [Fact]
        public async Task UpdateRequestAsync_ShouldUpdateRequestInDatabase()
        {
            // Arrange
            Request mockRequest = new() { RequestId = 1, UserId = 1, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1 };
            _dbContext.Requests.Add(mockRequest);
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            mockRequest.UserId = 2;
            mockRequest.Title = "New Title";

            // Act
            await sut.UpdateRequestAsync(mockRequest);

            // Assert
            var updatedRequest = await _dbContext.Requests.FirstOrDefaultAsync(r => r.RequestId == mockRequest.RequestId);
            updatedRequest.Should().NotBeNull();
            updatedRequest?.UserId.Should().Be(2);
            updatedRequest?.Title.Should().Be($"{mockRequest.Title}");
        }

        [Fact]
        public async Task DeleteRequestAsync_ShouldDeleteRequestFromDatabase()
        {
            // Arrange
            Request mockRequest = new() { RequestId = 1, UserId = 1, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1 };
            _dbContext.Requests.Add(mockRequest);
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            await sut.DeleteRequestAsync(mockRequest);

            // Assert
            var deletedUser = await _dbContext.Requests.FirstOrDefaultAsync(r => r.RequestId == mockRequest.RequestId);
            deletedUser.Should().BeNull();
        }


        [Fact]
        public async Task GetAllRequestByUserIdAsync_ShouldReturnRequestList()
        {
            // Arrange
            long mockUserId = 1;
            _dbContext.Requests.AddRange(RequestMockData.RequestList());
            await _dbContext.SaveChangesAsync();
            RequestRepository sut = new RequestRepository(_dbContext);

            // Act
            var result = await sut.GetAllRequestByUserIdAsync(mockUserId, string.Empty);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().BeOfType<List<Request>>();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
