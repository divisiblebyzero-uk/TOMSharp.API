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
    public class TimeEntryReportController : ControllerBase
    {
        private readonly TogglContext _context;
        private readonly SpreadsheetService _spreadsheetService;

        public TimeEntryReportController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }
        
        [HttpGet("{financeYear}")]
        public async Task<ActionResult<IEnumerable<TimeEntryReportItem>>> GetFullReport(string financeYear)
        {
            return await GetData(financeYear).ToArrayAsync();
        }

        private IQueryable<TimeEntryReportItem> GetData(string financeYear)
        {
            return _context.TimeEntryReportItems
                .Where(teri => teri.SprintsFinanceYear == financeYear)
                .OrderBy(teri => teri.SprintsStartDate);
        }
        
        [AllowAnonymous]
        [HttpGet("{financeYear}/Excel")]
        public async Task<FileStreamResult> GetFullReportExcel(string financeYear)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: GetData(financeYear),
                new []
                {
                    new PivotOptions
                    {
                        PivotName = "ChargeByProjectAndResource",
                        PivotRows = new[] { "BudgetLinesName", "ProjectsFinanceWIPCode", "TimeEntriesProject", "ProjectsActivity", "PeopleTeam", "PeopleName" },
                        PivotColumns = new[] {"SprintsFinanceMonth", "SprintsName"},
                        PivotValues = new[] { "Charge" }
                    }
                }
            );
            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, financeYear));
        }
    }
}    