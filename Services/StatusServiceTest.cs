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
    public class StatusServiceTest
    { 
        [Fact]
        public async Task GetStatusesAsync_ShouldReturnStatusDtoList()
        {
            // Arrange
            var mockRepo = new Mock<IStatusRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            List<Status> status = new List<Status>()
            {
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Pending,
                    Description = "pending"
                },
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Approved,
                    Description = "approved"
                },
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Rejected,
                    Description = "rejected"
                }
            };
            List<StatusDto> statusDto = new List<StatusDto>()
            {
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Pending,
                    Description = "pending"
                },
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Approved,
                    Description = "approved"
                },
                new()
                {
                    StatusCode = (int)Constants.StatusCode.Rejected,
                    Description = "rejected"
                }
            };
            mockRepo.Setup(_ => _.GetAllStatusAsync()).ReturnsAsync(status);
            mockMapper.Setup(_ => _.Map<IEnumerable<Status>, List<StatusDto>>(status)).Returns(statusDto);
            StatusService sut = new StatusService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.GetStatusesAsync();

            // Assert
            mockRepo.Verify(_ => _.GetAllStatusAsync(), Moq.Times.Once);
            mockMapper.Verify(_ => _.Map<IEnumerable<Status>, List<StatusDto>>(status), "Failed");
            Assert.NotNull(result);
            Assert.IsType<List<StatusDto>>(result);
            Assert.Equal(status.Count, result.Count());
            Assert.Equal(result, statusDto);
        }


        [Fact]
        public async Task AddStatusAsync_ShouldReturnStatusDto()
        {
            // Arrange
            var mockRepo = new Mock<IStatusRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            StatusDto statusDto = new StatusDto()
            {
                StatusCode = (int)Constants.StatusCode.Approved,
                Description = "approved"
            };
            Status status = new Status()
            {
                StatusCode = (int)Constants.StatusCode.Approved,
                Description = "approved"
            };
            
            mockRepo.Setup(_ => _.AddStatusAsync(status)).ReturnsAsync(status);
            mockMapper.Setup(_ => _.Map<StatusDto, Status>(statusDto)).Returns(status);
            mockMapper.Setup(_ => _.Map<Status, StatusDto>(status)).Returns(statusDto);
            StatusService sut = new StatusService(mockRepo.Object, mockMapper.Object);

            // Act
            var result = await sut.AddStatusAsync(statusDto);

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(_ => _.AddStatusAsync(status), Moq.Times.Once);
            mockMapper.Verify(_ => _.Map<Status, StatusDto>(status), "Failed");
            mockMapper.Verify(_ => _.Map<StatusDto, Status>(statusDto), "Failed");
            Assert.IsType<StatusDto>(result);
            Assert.Equal(result, statusDto);
        }


        [Fact]
        public async Task DeleteStatusAsync_ShouldDeleteStatus()
        {
            // Arrange
            var mockRepo = new Mock<IStatusRepository>(MockBehavior.Strict);
            var mockMapper = new Mock<IMapper>();
            int statusCode = (int)Constants.StatusCode.Pending;
            Status status = new Status()
            {
                StatusCode = (int)Constants.StatusCode.Approved,
                Description = "approved"
            };

            mockRepo.Setup(_ => _.GetStatusByStatusCodeAsync(statusCode)).ReturnsAsync(status);
            mockRepo.Setup(_ => _.DeleteStatusAsync(status)).Returns(Task.CompletedTask);

            StatusService sut = new StatusService(mockRepo.Object, mockMapper.Object);

            // Act
            await sut.DeleteStatusAsync(statusCode);

            // Assert
            mockRepo.Verify(_ => _.GetStatusByStatusCodeAsync(statusCode), Moq.Times.Once);
            mockRepo.Verify(_ => _.DeleteStatusAsync(status), Moq.Times.Once);
        }
    }
}
