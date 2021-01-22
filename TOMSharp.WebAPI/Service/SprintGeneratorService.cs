// Copyright (C) 2021 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using TOMSharp_Loader.Model;

namespace TOMSharp_Loader.Service
{
    public class SprintGeneratorService
    {
        private readonly TogglContext _dbContext;
        private readonly ILogger _logger;
        private readonly DateTime _yearStart2019 = DateTime.Parse("2019-01-01");
        private readonly DateTime _yearStart2020 = DateTime.Parse("2020-01-06");
        private readonly DateTime _yearStart2021 = DateTime.Parse("2021-01-04");
        private readonly string[] _firstThreeMonths = new string[] {"Jan", "Feb", "Mar"};

        public SprintGeneratorService(TogglContext dbContext, ILogger<SprintGeneratorService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        private void AddOrUpdateSprint(Sprint newSprint)
        {
            Sprint oldSprint = _dbContext.Sprints.FirstOrDefault(sprint => sprint.Name == newSprint.Name);
            if (oldSprint == null)
            {
                _dbContext.Add(newSprint);
            }
            else
            {
                oldSprint.StartDate = newSprint.StartDate;
                oldSprint.EndDate = newSprint.EndDate;
                oldSprint.FinanceMonth = newSprint.FinanceMonth;
                oldSprint.FinanceYear = newSprint.FinanceYear;
            }
        }

        private Sprint GenerateSprint(string nameFormat, DateTime yearStart, int sprintNumber)
        {
            Sprint sprint = new Sprint();
            sprint.Name = String.Format(nameFormat, sprintNumber);
            _logger.LogInformation($"Generating sprint: {sprint.Name}");
            sprint.StartDate = yearStart.AddDays((sprintNumber - 1) * 14);
            if (sprintNumber == 26 && yearStart == _yearStart2019)
            {
                sprint.EndDate = sprint.StartDate.AddDays(19);
            }
            else
            {
                sprint.EndDate = sprint.StartDate.AddDays(13);
            }
            
            DateTime midpoint = sprint.StartDate.AddDays(7);
            sprint.FinanceMonth = midpoint.ToString("MMM");
            sprint.FinanceYear = "FY" + (midpoint.AddMonths(9).Year - 2000);
            return sprint;
        }

        public int CreateOrUpdateSprintDefinitions()
        {

            for (int i = 1; i < 27; i++)
            {
                AddOrUpdateSprint(GenerateSprint("Sprint 19-{0:00}", _yearStart2019, i));
                AddOrUpdateSprint(GenerateSprint("Sprint 20-{0:00}", _yearStart2020, i));
                AddOrUpdateSprint(GenerateSprint("Sprint 21-{0:00}", _yearStart2021, i));
            }

            _dbContext.SaveChanges();
            _logger.LogInformation($"Done");
            return 0;
        }
    }
}
