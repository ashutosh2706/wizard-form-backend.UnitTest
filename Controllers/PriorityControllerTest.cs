using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Controllers;
using WizardFormBackend.Data.Dto;
using WizardFormBackend.Services;
using WizardFormBackend.Tests.MockData;

namespace WizardFormBackend.Tests.Controllers
{
    public class PriorityControllerTest
    {
        [Fact]
        public async Task GetPriorities_ShouldReturnStatus200_WhenDataPresent()
        {
            // Arrange
            var mockPriortiyService = new Mock<IPriorityService>();
            mockPriortiyService.Setup(_ => _.GetPrioritiesAsync()).ReturnsAsync(PriorityMockData.ListOfPriortiyDto());
            var sut = new PriorityController(mockPriortiyService.Object);

            // Act
            var result = await sut.GetPriorities();

            // Assert
            result.GetType().Should().Be(typeof(OkObjectResult));
            (result as OkObjectResult)?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetPriorities_ShouldReturnStatus204_WhenDataNotPresent()
        {
            // Arrange
            var mockPriortiyService = new Mock<IPriorityService>();
            mockPriortiyService.Setup(_ => _.GetPrioritiesAsync()).ReturnsAsync(PriorityMockData.EmptyListOfPriorityDto());
            var sut = new PriorityController(mockPriortiyService.Object);

            // Act
            var result = await sut.GetPriorities();

            // Assert
            result.GetType().Should().Be(typeof(NoContentResult));
            (result as NoContentResult)?.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task AddPriority_ShouldReturnStatus201()
        {
            // Arrange
            var mockPriorityService = new Mock<IPriorityService>();
            PriorityDto priorityDto = new()
            {
                PriorityCode = 1,
                Description = "Test",
            };

            mockPriorityService.Setup(_ => _.AddPriorityAsync(priorityDto));
            var sut = new PriorityController(mockPriorityService.Object);
            // Act
            var result = await sut.AddPriority(priorityDto);

            // Assert
            result.GetType().Should().Be(typeof(CreatedResult));
            (result as CreatedResult)?.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task DeletePriority_ShouldReturnStatus204()
        {
            // Arrange
            var mockPriorityService = new Mock<IPriorityService>();
            mockPriorityService.Setup(_ => _.DeletePriorityAsync(0));
            var sut = new PriorityController(mockPriorityService.Object);

            // Act
            var result = await sut.DeletePriority(0);

            // Assert
            result.GetType().Should().Be(typeof(NoContentResult));
            (result as NoContentResult)?.StatusCode.Should().Be(204);
        }

    }
}
