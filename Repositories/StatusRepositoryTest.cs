using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data;
using WizardFormBackend.Data.Models;
using WizardFormBackend.Data.Repositories;
using WizardFormBackend.Tests.MockData;

namespace WizardFormBackend.Tests.Repositories
{
    public class StatusRepositoryTest : IDisposable
    {
        private readonly WizardFormDbContext _dbContext;

        public StatusRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<WizardFormDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _dbContext = new WizardFormDbContext(options);
            _dbContext.Database.EnsureCreated();
        }


        [Fact]
        public async Task GetAllStatusAsync_ShouldReturnStatusList()
        {
            // Arrange
            _dbContext.Statuses.AddRange(StatusMockData.StatusList());
            await _dbContext.SaveChangesAsync();
            StatusRepository sut = new StatusRepository(_dbContext);

            // Act
            var result = await sut.GetAllStatusAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Status>>(result);
            Assert.Equal(StatusMockData.StatusList().Count, result.Count());
        }

        [Fact]
        public async Task GetStatusByStatusCodeAsync_ShouldReturnStatus()
        {
            // Arrange
            int statusCode = (int)Constants.StatusCode.Pending;
            _dbContext.Statuses.AddRange(StatusMockData.StatusList());
            await _dbContext.SaveChangesAsync();
            StatusRepository sut = new StatusRepository(_dbContext);

            // Act
            var result = await sut.GetStatusByStatusCodeAsync(statusCode);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Status>(result);
            Assert.Equal(statusCode, result.StatusCode);
        }


        [Fact]
        public async Task AddStatusAsync_ShouldAddStatusToDatabase()
        {
            // Arrange
            Status status = StatusMockData.StatusList()[0];
            StatusRepository sut = new StatusRepository(_dbContext);

            // Act
            var result = await sut.AddStatusAsync(status);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Status>(result);
            var addedStatus = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.StatusCode == status.StatusCode);
            Assert.NotNull(addedStatus);
            Assert.Equal(addedStatus.StatusCode, status.StatusCode);
        }


        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatusInDatabase()
        {
            // Arrange
            Status status = StatusMockData.StatusList()[0];
            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();
            StatusRepository sut = new StatusRepository(_dbContext);
            status.Description = "New Description";

            // Act
            await sut.UpdateStatusAsync(status);

            // Assert
            var updatedStatus = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.StatusCode == status.StatusCode);
            Assert.NotNull(updatedStatus);
            Assert.Equal(updatedStatus.Description, status.Description);
        }


        [Fact]
        public async Task DeleteStatusAsync_ShouldDeleteStatusFromDatabase()
        {
            // Arrange
            Status status = StatusMockData.StatusList()[0];
            _dbContext.Statuses.Add(status);
            await _dbContext.SaveChangesAsync();
            StatusRepository sut = new StatusRepository(_dbContext);
            
            // Act
            await sut.DeleteStatusAsync(status);

            // Assert
            var deletedStatus = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.StatusCode == status.StatusCode);
            Assert.Null(deletedStatus);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

    }
}
