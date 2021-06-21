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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TOMSharp_Loader.Model;

namespace TOMSharp_Loader.Service
{
    public class TimeEntryDownloader
    {
        private readonly TogglContext _togglContext;
        private readonly ILogger _logger;
        private readonly TogglOptions _togglOptions;
        
        private readonly ProjectActivityDeterminer _projectActivityDeterminer = new ProjectActivityDeterminer();

        // These are two ids which have been created in Toggl, but I can't for the life of me figure out how to remove
        // them. They are skewing the results, consequently they will be hardcoded to be skipped!
        private readonly List<long> _skippedUids;
        
        public TimeEntryDownloader(TogglContext togglContext, ILogger<TimeEntryDownloader> logger, IOptions<TogglOptions> togglOptions)
        {
            _togglContext = togglContext;
            _logger = logger;
            _togglOptions = togglOptions.Value;
            _skippedUids = _togglOptions.SkippedUserIds.Split(",").Select<string, long>(long.Parse).ToList();
        }

        private Sprint GetLatestSprint()
        {
            DateTime now = DateTime.Now;
            return _togglContext.Sprints.First(s=>s.StartDate < now && s.EndDate > now);
        }

        private string GetCurrentFinanceYear()
        {
            var now = DateTime.Now;
            var yearStub = now.Year % 2000;
            if (now.Month >= 4)
            {
                yearStub++;
            }
            return $"FY{yearStub}";
        }

        public int DownloadTimeEntries()
        {
            return DownloadTimeEntries(GetLatestSprint());
        }

        public int DownloadTimeEntries(string sprintName)
        {
            return DownloadTimeEntries(_togglContext.Sprints.First(s => s.Name == sprintName));
        }

        private string GetJsonFromFilesystem()
        {
            string path = @".\sampledata.json";
            return File.ReadAllText(path);
        }
        
        private string GetJsonFromWeb(Sprint sprint, int pageNumber)
        {
            string sprintUrl = String.Format(_togglOptions.DownloadUrl,
                sprint.StartDate.ToString("yyyy-MM-dd"),
                sprint.EndDate.ToString("yyyy-MM-dd"),
                pageNumber,
                _togglOptions.WorkspaceId);
            _logger.LogInformation($"Downloading from Url: {sprintUrl}");
            using var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] =
                "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_togglOptions.AuthorisationString));
            return webClient.DownloadString(sprintUrl);
        }

        private Project CheckAndCreateProject(TimeEntry timeEntry, BudgetLine defaultBudgetLine)
        {
            var project = _togglContext.Projects.FirstOrDefault(p => p.TimeEntryName == timeEntry.Project);
            if (project == null)
            {
                
                project = new Project
                {
                    Name = timeEntry.Project,
                    TimeEntryName = timeEntry.Project,
                    BudgetLine = defaultBudgetLine,
                    Activity = _projectActivityDeterminer.DetermineProjectActivity(timeEntry.Project),
                    FinanceYear = GetCurrentFinanceYear()
                };
                _togglContext.Projects.Add(project);
            }

            return project;
        }


        
        public int DownloadTimeEntriesPage(Sprint sprint, int pageNumber)
        {
            _logger.LogInformation($"Downloading page {pageNumber}");
            string json = _togglOptions.Download ? GetJsonFromWeb(sprint, pageNumber) : GetJsonFromFilesystem();

            TogglTransferObject tto = JsonConvert.DeserializeObject<TogglTransferObject>(json, new JsonSerializerSettings
                { NullValueHandling = NullValueHandling.Ignore });

            var projectMap = new Dictionary<string, Project>();
            
            var defaultBudgetLine = _togglContext.BudgetLines.FirstOrDefault(bl => bl.Name == "DEFAULT" && bl.FinanceYear == sprint.FinanceYear) ??
                                    new BudgetLine {Name = "DEFAULT", FinanceYear = sprint.FinanceYear};

            foreach (TimeEntry timeEntry in tto.Data)
            {
                _logger.LogInformation("Skipped Uids - " + string.Join(",",_skippedUids));
                if (!_skippedUids.Contains(timeEntry.Uid))
                {
                    if (timeEntry.Project == null)
                    {
                        timeEntry.Project = "";
                    }

                    _logger.LogInformation($"Adding time entry {timeEntry}");
                    if (timeEntry.Start >= sprint.StartDate && timeEntry.Start <= sprint.EndDate)
                    {
                        if (!projectMap.ContainsKey(timeEntry.Project))
                        {
                            projectMap[timeEntry.Project] = CheckAndCreateProject(timeEntry, defaultBudgetLine);
                        }

                        var conflictingTimeEntry =
                            _togglContext.TimeEntries.FirstOrDefault(t => t.Id == timeEntry.Id);
                        if (conflictingTimeEntry != null)
                        {
                            _logger.LogInformation(
                                $"Updating timeentry with id {conflictingTimeEntry.Id} as it already exists. Consider updating a previous sprint.");
                            conflictingTimeEntry.UpdateFrom(timeEntry);
                            _togglContext.Update(conflictingTimeEntry);
                        }
                        else
                        {
                            _togglContext.Add(timeEntry);
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            $"Time entry falls outside of sprint boundaries (query failure?): {timeEntry}");
                    }
                }
            }

            _togglContext.SaveChanges();

            if (tto.total_count > pageNumber * tto.per_page)
            {
                return DownloadTimeEntriesPage(sprint, pageNumber + 1);
            }

            return 0;
        }

        public int DownloadTimeEntries(Sprint sprint)
        {
            _logger.LogInformation($"Downloading sprint {sprint.Name}");
            _logger.LogInformation($"Removing old time entries for sprint range ({sprint.StartDate} to {sprint.EndDate})");
            _togglContext.TimeEntries.RemoveRange(_togglContext.TimeEntries.Where(te => te.Start >= sprint.StartDate && te.Start < sprint.EndDate.AddDays(1)));
            _togglContext.SaveChanges();

            return DownloadTimeEntriesPage(sprint, 1);
            
        }

        class TogglTransferObject
        {
            public int total_count { get; set; }
            public int per_page { get; set; }
            public List<TimeEntry> Data { get; set; }
        }
    }
}
