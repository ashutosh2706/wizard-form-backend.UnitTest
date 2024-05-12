using AutoMapper;
using FluentAssertions;
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
using WizardFormBackend.Tests.MockData;

namespace WizardFormBackend.Tests.Services
{
    public class PriorityServiceTest
    {
        [Fact]
        public async Task GetPrioritesAsync_ShouldReturnPriorityDtos()
        {
            /* Arrange **/
            var mockPriorityRepository = new Mock<IPriorityRepository>();
            var mockMapper = new Mock<IMapper>();
            var priorities = new List<Priority>
            {
                new()
                {
                    PriorityCode = 1,
                    Description = "Test",
                },
                new()
                {
                    PriorityCode = 2,
                    Description = "Test",
                }
            };
            mockPriorityRepository.Setup(_ => _.GetAllPriorityAsync()).ReturnsAsync(priorities);

            var expectedPriortiyDtos = new List<PriorityDto>
            {
                new()
                {
                    PriorityCode = 1,
                    Description = "Test",
                },
                new()
                {
                    PriorityCode = 2,
                    Description = "Test",
                }
            };

            mockMapper.Setup(_ => _.Map<IEnumerable<Priority>, List<PriorityDto>>(priorities)).Returns(expectedPriortiyDtos);
            var sut = new PriorityService(mockPriorityRepository.Object, mockMapper.Object);

            /* Act **/
            var expectedResult = await sut.GetPrioritiesAsync();

            /* Assert **/
            expectedResult.Should().NotBeNull();
            expectedResult.Should().BeEquivalentTo(expectedPriortiyDtos);
        }
    }
}
