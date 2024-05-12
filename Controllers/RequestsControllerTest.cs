using FluentAssertions;
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
using WizardFormBackend.Utils;

namespace WizardFormBackend.Tests.Controllers
{
    public class RequestsControllerTest
    {
        private readonly Mock<IRequestService> _requestServiceMock;
        private readonly RequestsController sut;
        public RequestsControllerTest()
        {
            _requestServiceMock = new Mock<IRequestService>();
            sut = new RequestsController(_requestServiceMock.Object);
        }

        [Fact]
        public async Task GetAllRequest_ShouldReturnStatus200_WithPaginatedResponseDto()
        {
            // Arrange
            var queryParams = new QueryParams
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 10,
                SortField = "fieldName",
                SortDirection = "ascending",
            };

            var pagedResponseDto = new PagedResponseDto<RequestDto>
            {
                PageNumber = 1,
                PageSize = 10,
                TotalPage = 1,
                Items = new List<RequestDto> { new RequestDto { RequestId = 1, UserId = 1, RequestDate = DateOnly.MinValue } }
            };

            _requestServiceMock.Setup(_ => _.GetAllRequestAsync(queryParams.SearchTerm, queryParams.PageNumber, queryParams.PageSize, queryParams.SortField, queryParams.SortDirection)).ReturnsAsync(pagedResponseDto);


            /// Act
            var result = await sut.GetAllRequest(queryParams);

            /// Assert
            result.GetType().Should().Be(typeof(OkObjectResult));
            (result as OkObjectResult)?.StatusCode.Should().Be(200);
            (result as OkObjectResult)?.Value.Should().BeEquivalentTo(pagedResponseDto);
        }

        [Fact]
        public async Task GetAllRequestByUserId_ShouldReturnStatus200_WithPaginatedResponseDto()
        {
            // Arrange
            var queryParams = new QueryParams
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 10,
                SortField = "fieldName",
                SortDirection = "ascending",
            };

            var pagedResponseDto = new PagedResponseDto<RequestDto>
            {
                PageNumber = 1,
                PageSize = 10,
                TotalPage = 1,
                Items = new List<RequestDto> { new RequestDto { RequestId = 1, UserId = 1, RequestDate = DateOnly.MinValue } }
            };

            _requestServiceMock.Setup(_ => _.GetAllRequestByUserIdAsync(1, queryParams.SearchTerm, queryParams.PageNumber, queryParams.PageSize, queryParams.SortField, queryParams.SortDirection)).ReturnsAsync(pagedResponseDto);
            
            // Act
            var result = await sut.GetAllRequestByUserId(1, queryParams);

            // Assert
            result.GetType().Should().Be(typeof(OkObjectResult));
            (result as OkObjectResult)?.StatusCode.Should().Be(200);
            (result as OkObjectResult)?.Value.Should().BeEquivalentTo(pagedResponseDto);
        }


        [Fact]
        public async Task GetRequestByRequestId_ShouldReturnStatus200_WithRequestDto_WhenRequestIdIsValid()
        {
            /// Arrange
            RequestDto mockRequestDto = new()
            {
                RequestId = 1,
                UserId = 1,
                RequestDate = DateOnly.MinValue,
                Title = "Title",
                PriorityCode = 1,
                StatusCode = 2,
                GuardianName = "Test"
            };
            _requestServiceMock.Setup(_ => _.GetRequestByRequestIdAsync(0)).ReturnsAsync(mockRequestDto);
            /// Act
            var result = await sut.GetRequestByRequestId(0);
            /// Assert
            result.GetType().Should().Be(typeof(OkObjectResult));
            (result as OkObjectResult)?.StatusCode.Should().Be(200);
            (result as OkObjectResult)?.Value.Should().BeEquivalentTo(mockRequestDto);
        }

        [Fact]
        public async Task GetRequestByRequestId_ShouldReturnStatus404_WhenRequestIdIsInvalid()
        {
            // Arrange
            _requestServiceMock.Setup(_ => _.GetRequestByRequestIdAsync(-1)).ReturnsAsync((RequestDto)null);

            // Act
            var result = await sut.GetRequestByRequestId(-1);
            
            // Assert
            result.GetType().Should().Be(typeof(NotFoundResult));
            (result as NotFoundResult)?.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task AddRequest_ShouldReturnStatus201()
        {
            RequestDto mockRequestDto = new()
            {
                RequestId = 1,
                UserId = 1,
                RequestDate = DateOnly.MinValue,
                Title = "Title",
                PriorityCode = 1,
                StatusCode = 2,
                GuardianName = "Test"
            };
            _requestServiceMock.Setup(_ => _.AddRequestAsync(mockRequestDto));
            var result = await sut.AddRequest(mockRequestDto);
            result.GetType().Should().Be(typeof(CreatedResult));
            (result as CreatedResult)?.StatusCode.Should().Be(201);

        }

        [Fact]
        public async Task UpdateRequestStatus_ShouldReturnStatus200_WhenRequestIdIsValid()
        {
            _requestServiceMock.Setup(_ => _.UpdateRequestStatusAsync(1, 1)).ReturnsAsync(true);

            var result = await sut.UpdateRequestStatus(1, 1);
            result.GetType().Should().Be(typeof(OkResult));
            (result as OkResult)?.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateRequestStatus_ShouldReturnStatus404_WhenRequestIdIsInvalid()
        {
            _requestServiceMock.Setup(_ => _.UpdateRequestStatusAsync(-1, 1)).ReturnsAsync(false);

            var result = await sut.UpdateRequestStatus(-1, 1);
            result.GetType().Should().Be(typeof(NotFoundResult));
            (result as NotFoundResult)?.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteRequest_ShouldReturnStatus200_WhenRequestIdIsValid()
        {
            _requestServiceMock.Setup(_ => _.DeleteRequestAsync(1)).ReturnsAsync(true);
            var result = await sut.DeleteRequest(1);
            result.GetType().Should().Be(typeof(NoContentResult));
            (result as NoContentResult)?.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task DeleteRequest_ShouldReturnStatus404_WhenRequestIdIsInvalid()
        {
            _requestServiceMock.Setup(_ => _.DeleteRequestAsync(1)).ReturnsAsync(true);
            var result = await sut.DeleteRequest(2);
            result.GetType().Should().Be(typeof(NotFoundResult));
            (result as NotFoundResult)?.StatusCode.Should().Be(404);
        }
    }
}
