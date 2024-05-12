using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardFormBackend.Data.Dto;
using WizardFormBackend.Data.Models;

namespace WizardFormBackend.Tests.MockData
{
    public class PriorityMockData
    {
        public static List<PriorityDto> ListOfPriortiyDto()
        {
            return
            [
                new() {
                    PriorityCode = 1,
                    Description = "Test"
                },
                new()
                {
                    PriorityCode = 2,
                    Description = "Test"
                }
            ];
        }

        public static List<PriorityDto> EmptyListOfPriorityDto()
        {
            return [];
        }

        public static List<Priority> ListOfPriority()
        {
            return new List<Priority>()
            {
                new()
                {
                    PriorityCode = 1,
                    Description = "Test"
                },
                new()
                {
                    PriorityCode = 2,
                    Description = "Test"
                }
            };
        }
    }
}
