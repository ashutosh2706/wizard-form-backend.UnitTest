using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Repositories;
using WizardFormBackend.Data;
using WizardFormBackend.Tests.MockData;
using FluentAssertions;

namespace WizardFormBackend.Tests.Repositories
{
    public class PriorityRepositoryTest : IDisposable
    {
        private readonly WizardFormDbContext _dbContext;
        public PriorityRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<WizardFormDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _dbContext = new WizardFormDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllPrioritiesAsync_ShouldReturnPriorityList()
        {
            /* Arrange **/
            _dbContext.AddRange(PriorityMockData.ListOfPriority());
            _dbContext.SaveChanges();
            var sut = new PriorityRepository(_dbContext);

            /* Act **/
            var result = await sut.GetAllPriorityAsync();

            /* Assert  **/
            result.Should().NotBeNull();
            result.Should().HaveCount(PriorityMockData.ListOfPriority().Count);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
