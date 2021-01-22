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

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly TimeEntryDownloader _timeEntryDownloader;
        private readonly TogglContext _context;
        private readonly SprintGeneratorService _sprintGeneratorService;
        private readonly ILogger<ActionsController> _logger;

        public ActionsController(ILogger<ActionsController> logger, TimeEntryDownloader timeEntryDownloader, TogglContext context, SprintGeneratorService sprintGeneratorService)
        {
            _logger = logger;
            _timeEntryDownloader = timeEntryDownloader;
            _context = context;
            _sprintGeneratorService = sprintGeneratorService;
        }

        
        // GET: api/Actions/DownloadSprint
        [HttpGet("DownloadSprint/{sprintName}")]
        public ActionResult<string> DownloadSprint(string sprintName)
        {
            _logger.LogInformation($"Beginning sprint download for {sprintName}");
            Sprint sprint = _context.Sprints.FirstOrDefault(s => s.Name == sprintName);
            if (sprint == null)
            {
                return NotFound();
            }

            _timeEntryDownloader.DownloadTimeEntries(sprint);
            return Ok();
        }

        // GET: api/Actions/GenerateSprints
        [HttpGet("GenerateSprints")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public ActionResult GenerateSprints()
        {
            _sprintGeneratorService.CreateOrUpdateSprintDefinitions();
            return Ok();
        }
    }
}
