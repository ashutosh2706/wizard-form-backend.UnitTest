using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Models;

namespace WizardFormBackend.Tests.MockData
{
    public class StatusMockData
    {
        public static List<Status> StatusList()
        {
            return new List<Status>()
            {
                new() { StatusCode = (int)Constants.StatusCode.Pending, Description = "pending" },
                new() { StatusCode = (int)Constants.StatusCode.Approved, Description = "approved" },
                new() { StatusCode = (int)Constants.StatusCode.Rejected, Description = "rejected" },
            };
        }
    }
}
