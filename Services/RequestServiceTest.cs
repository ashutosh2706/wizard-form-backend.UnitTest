using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public class RequestServiceTest
    {
        private readonly Mock<IRequestRepository> _requestRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly RequestService _sut;

        public RequestServiceTest()
        {
            _requestRepositoryMock = new Mock<IRequestRepository>();
            _mapperMock = new Mock<IMapper>();
            _fileServiceMock = new Mock<IFileService>();
            _sut = new RequestService(_requestRepositoryMock.Object, _fileServiceMock.Object, _mapperMock.Object);
        }


        [Fact]
        public async Task GetAllRequestAsync_ShouldReturnPaginatedResponseOfTypeRequestDto()
        {

            IEnumerable<Request> mockRequests = new List<Request>()
            {
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" },
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" },
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" }
            };


            List<RequestDto> mockResponse = new List<RequestDto>()
            {
                new RequestDto()
                {
                    RequestDate = DateOnly.MinValue,
                    RequestId = 1,
                    UserId = 1,
                    GuardianName = "Test",
                    Phone = "123",
                    PriorityCode = 1,
                    StatusCode = 1,
                    Title = "Test"
                },

                new RequestDto()
                {
                    RequestDate = DateOnly.MinValue,
                    RequestId = 1,
                    UserId = 1,
                    GuardianName = "Test",
                    Phone = "123",
                    PriorityCode = 1,
                    StatusCode = 1,
                    Title = "Test"
                },

            };

            _requestRepositoryMock.Setup(_ => _.GetAllRequestAsync("test")).ReturnsAsync(mockRequests);
            _mapperMock.Setup(_ => _.Map<IEnumerable<Request>, List<RequestDto>>(mockRequests)).Returns(mockResponse);

            var result = await _sut.GetAllRequestAsync("test", 1, 10, It.IsAny<string>(), It.IsAny<string>());

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponseDto<RequestDto>>();
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPage.Should().Be(1);
            result.Items.Should().NotBeNull().And.HaveCount(2);

        }

        [Fact]
        public async Task AddRequestAsync_ShouldReturnRequestDto()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 9, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };

            RequestDto mockRequestDto = new()
            {
                RequestDate = DateOnly.MinValue,
                RequestId = 1,
                UserId = 1,
                GuardianName = "Test",
                Phone = "123",
                PriorityCode = 1,
                StatusCode = 3,
                Title = "Test"
            };

            _requestRepositoryMock.Setup(_ => _.AddRequestAsync(mockRequest)).ReturnsAsync(mockRequest);
            _mapperMock.Setup(_ => _.Map<RequestDto, Request>(mockRequestDto)).Returns(mockRequest);

            var response = await _sut.AddRequestAsync(mockRequestDto);

            response.Should().NotBeNull();
            response.Should().BeOfType<RequestDto>();
            response.StatusCode.Should().Be(mockRequest.StatusCode);
            response.RequestId.Should().Be(mockRequest.RequestId);
        }


        [Fact]
        public async Task UpdateRequestAsync_ShouldReturnTrue_WhenRequestIdIsValid()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 9, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };

            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(9)).ReturnsAsync(mockRequest);
            var result = await _sut.UpdateRequestStatusAsync(9, It.IsAny<int>());

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateRequestAsync_ShouldReturnFalse_WhenRequestIdIsInValid()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 9, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };

            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(1)).ReturnsAsync(mockRequest);
            var result = await _sut.UpdateRequestStatusAsync(9, It.IsAny<int>());
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllRequestByUserIdAsync_ShouldReturnPaginatedResponseOfTypeRequestDto()
        {
            IEnumerable<Request> mockRequests = new List<Request>()
            {
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" },
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" },
                new() {RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" }
            };

            List<RequestDto> mockResponse = new List<RequestDto>()
            {
                new RequestDto()
                {
                    RequestDate = DateOnly.MinValue,
                    RequestId = 1,
                    UserId = 1,
                    GuardianName = "Test",
                    Phone = "123",
                    PriorityCode = 1,
                    StatusCode = 1,
                    Title = "Test"
                },

                new RequestDto()
                {
                    RequestDate = DateOnly.MinValue,
                    RequestId = 1,
                    UserId = 1,
                    GuardianName = "Test",
                    Phone = "123",
                    PriorityCode = 1,
                    StatusCode = 1,
                    Title = "Test"
                },

            };

            _requestRepositoryMock.Setup(_ => _.GetAllRequestByUserIdAsync(1, "test")).ReturnsAsync(mockRequests);
            _mapperMock.Setup(_ => _.Map<IEnumerable<Request>, List<RequestDto>>(mockRequests)).Returns(mockResponse);

            var result = await _sut.GetAllRequestByUserIdAsync(1, "test", 1, 10, It.IsAny<string>(), It.IsAny<string>());

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedResponseDto<RequestDto>>();
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPage.Should().Be(1);
            result.Items.Should().NotBeNull().And.HaveCount(2);
        }


        [Fact]
        public async Task GetRequestByRequestIdAsync_ShouldReturnRequestDto_WhenRequestIdIsValid()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };
            RequestDto requestDtoMock = new RequestDto()
            {
                RequestDate = DateOnly.MinValue,
                RequestId = 1,
                UserId = 1,
                GuardianName = "Test",
                Phone = "123",
                PriorityCode = 1,
                StatusCode = 1,
                Title = "Test"
            };

            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(1)).ReturnsAsync(mockRequest);
            _mapperMock.Setup(_ => _.Map<Request, RequestDto>(mockRequest)).Returns(requestDtoMock);

            var result = await _sut.GetRequestByRequestIdAsync(1);
            result.Should().NotBeNull();
            result.Should().BeOfType<RequestDto>();
            result?.RequestId.Should().Be(mockRequest.RequestId);
        }


        [Fact]
        public async Task GetRequestByRequestIdAsync_ShouldReturnNull_WhenRequestIdIsInValid()
        {
            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(1));
            var result = await _sut.GetRequestByRequestIdAsync(1);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteRequestAsync_ShouldReturnTrue_WhenRequestIdIsValid()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };
            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(1)).ReturnsAsync(mockRequest);
            var result = await _sut.DeleteRequestAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteRequestAsync_ShouldReturnFalse_WhenRequestIdIsInValid()
        {
            Request mockRequest = new() { RequestDate = DateOnly.MinValue, RequestId = 1, UserId = 1, GuardianName = "Test", Phone = "123", PriorityCode = 1, StatusCode = 1, Title = "Test" };
            _requestRepositoryMock.Setup(_ => _.GetRequestByRequestIdAsync(1)).ReturnsAsync(mockRequest);
            var result = await _sut.DeleteRequestAsync(2);
            result.Should().BeFalse();
        }
    }
}
