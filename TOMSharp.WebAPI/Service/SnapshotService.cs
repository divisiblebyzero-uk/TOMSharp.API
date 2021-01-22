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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TOMSharp_Loader.Model;

namespace TOMSharp_Loader.Service
{
    public class SnapshotService
    {
        private readonly ILogger<SnapshotService> _logger;
        private readonly TogglContext _context;

        public SnapshotService(ILogger<SnapshotService> logger, TogglContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        // Copy ACTUAL timebookings for the financial year (up to the number of months), extrapolating the remaining
        // time if specified.
        // Example - suppose the following:
        //   - a scenario exists, showing a forecast spend of 100k on a project
        //   - time bookings show 10k per month have been spent Apr -> Jul
        // If createSnapshot were called with months = 4, extrapolateRemainingTime = false, then
        //   - a new forecast would be created
        //   - the actual time bookings for Apr -> Jul would be copied into the new forecast
        //   - all subsequent months would be blank
        // If instead the method were called with extrapolateRemainingTime = true (and the relevant scenarioId)
        //   - first the scenario would be iterated to work out the TOTAL forecast £s (per project)
        //   - this would then be evenly distributed on the remaining months (Aug -> Mar)
        public void CreateSnapshot(string financeYear, int months = 12, bool extrapolateRemainingTime = false, int? scenarioId = null)
        {
            var firstSprint = _context.Sprints.Where(sprint => sprint.FinanceYear == financeYear).OrderBy(sprint => sprint.StartDate).First();
            var monthNames = GetMonthsToSnapshot(firstSprint, months);
            var sprints = _context.Sprints
                .Where(sprint => sprint.FinanceYear == financeYear && monthNames.Contains(sprint.FinanceMonth)).ToList();

            var scenarioReportActuals = 
                _context.ScenarioReports
                    .Where(sr => sr.FinanceYear == financeYear && sprints.Select(s => s.Id).Contains(sr.SprintId)).ToList();

            var forecast = new Forecast
            {
                FinanceYear = financeYear,
                Name = "Snapshot-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                Created = DateTime.Now,
                Entries = new List<ForecastEntry>()
            };

            scenarioReportActuals.ForEach(sr =>
            {
                var entry = new ForecastEntry {Project = sr.Project, Hours = sr.Hours, Sprint = sr.Sprint};
                forecast.Entries.Add(entry);
            });

            if (extrapolateRemainingTime && scenarioId != null)
            {
                var blah = AddExtrapolatedForecast(forecast, months, (int)scenarioId, sprints);
            }
            
            forecast.Updated = DateTime.Now;

            _context.Forecasts.Add(forecast);
            _context.SaveChanges();
        }

        private string AddExtrapolatedForecast(Forecast forecast, int months, int scenarioId, List<Sprint> sprints)
        {
            var scenario = _context.Scenarios
                .Include(s => s.ScenarioProjects)
                .FirstOrDefault(s => s.Id == scenarioId);
            if (scenario == null)
            {
                _logger.LogError($"Invalid scenario id: {scenarioId}");
                return null;
            }

            var projects = scenario.ScenarioProjects.Select(sp => sp.Project).Distinct();

            var scenarioReports =
                _context.ScenarioReports
                    .Where(sr => sr.FinanceYear == scenario.FinanceYear && (sr.ScenarioId == scenarioId)).ToList();

            var lastSnapshotSprint = sprints.Last();
            var sprintsToForecast = _context.Sprints.Where(s => s.FinanceYear == scenario.FinanceYear && s.StartDate > lastSnapshotSprint.EndDate).ToList();
            
            foreach (var project in projects)
            {
                var totalHoursInOriginalForecast = scenarioReports.Where(sr => sr.Project == project).Select(sr => sr.Hours).Sum();
                var totalHoursInSnapshot =
                    forecast.Entries.Where(fe => fe.Project == project).Select(fe => fe.Hours).Sum();
                var totalHoursRemaining = totalHoursInOriginalForecast - totalHoursInSnapshot;
                if (totalHoursRemaining > 0)
                {
                    var hoursPerSprint = totalHoursRemaining / sprintsToForecast.Count();
                    foreach (var sprint in sprintsToForecast)
                    {
                        ForecastEntry entry = new ForecastEntry{Project = project, Hours = hoursPerSprint, Sprint = sprint.Name};
                        forecast.Entries.Add(entry);
                    }
                }
            }
            return "done";

        }

        private string[] GetMonthsToSnapshot(Sprint firstSprint, int months)
        {
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;
            if (monthNames.Last() == "")
            {
                monthNames = monthNames.Take(monthNames.Length - 1).ToArray();
            }
            var index = Array.IndexOf(monthNames, firstSprint.FinanceMonth);
            LeftShiftArray(monthNames, index);
            monthNames = monthNames[..months];
            return monthNames;
        }
        
        private void LeftShiftArray<T>(T[] arr, int shift)
        {
            shift = shift % arr.Length;
            T[] buffer = new T[shift];
            Array.Copy(arr, buffer, shift);
            Array.Copy(arr, shift, arr, 0, arr.Length - shift);
            Array.Copy(buffer, 0, arr, arr.Length - shift, shift);
        }
    }
}