using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Models;

namespace WizardFormBackend.Tests.MockData
{
    public class RequestMockData
    {
        public static List<Request> RequestList()
        {
            return new List<Request>()
            {
                new() {RequestId = 1, UserId = 1, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1},
                new() {RequestId = 2, UserId = 2, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1},
                new() {RequestId = 3, UserId = 3, Title = "Test", GuardianName = "Test", Phone = "123", RequestDate = DateOnly.MinValue, PriorityCode = 1, StatusCode = 1},
            };
        }
    }
}
