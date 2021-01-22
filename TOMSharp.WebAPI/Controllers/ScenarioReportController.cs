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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScenarioReportController : Controller
    {
        private readonly TogglContext _context;
        private SpreadsheetService _spreadsheetService;

        public ScenarioReportController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }
        
        // GET: /api/ScenarioReports/FY21
        [HttpGet("{financeYear}")]
        public IEnumerable<ScenarioReport> GetScenarioReports(string financeYear, int scenarioId)
        {
            return GetData(financeYear, scenarioId);
        }
        
        [HttpGet("{financeYear}/Excel")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetScenarioReportsExcel(string financeYear, int scenarioId)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: GetData(financeYear, scenarioId));
            string paramString = ($"{financeYear}");
            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, paramString));
        }
        
        private IEnumerable<ScenarioReport> GetData(string financeYear, int scenarioId)
        {
            var projects =
                _context.Scenarios
                    .Include(s => s.ScenarioProjects)
                    .Single(s => s.Id == scenarioId)
                    .ScenarioProjects
                    .Select(sp => sp.Project);
            
            var scenarioForecasts =
                _context.ScenarioReports
                    .Where(sr => sr.FinanceYear == financeYear && (sr.ScenarioId == scenarioId));
            
            var actualBookings=
                _context.ScenarioReports
                .Where(sr => sr.FinanceYear == financeYear && projects.Contains(sr.Project));
            
            return actualBookings.Union(scenarioForecasts).OrderBy(sr => sr.ScenarioId).ThenBy(sr => sr.Project).ThenBy(sr => sr.Sprint);
        }
    }
}