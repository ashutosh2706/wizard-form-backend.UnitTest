using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Models;

namespace WizardFormBackend.Tests.MockData
{
    public class UserMockData
    {
        public static List<User> UserList()
        {
            return new List<User>()
            {
                new() { UserId = 1, FirstName = "Test", LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true },
                new() { UserId = 2, FirstName = "Test",  LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true },
                new() { UserId = 3, FirstName = "Test",  LastName = "Test", Email = "abc@x.y", Password = "password", RoleId = 1, Active = true  }
            };
        }
    }
}
