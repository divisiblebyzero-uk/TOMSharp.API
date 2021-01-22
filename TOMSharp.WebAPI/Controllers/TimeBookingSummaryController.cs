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
    public class TimeBookingSummaryController : Controller
    {
        private readonly TogglContext _context;
        private readonly SpreadsheetService _spreadsheetService;

        public TimeBookingSummaryController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }

        // GET: /api/FinanceSummary/FY21
        [HttpGet("{financeYear}")]
        public async Task<ActionResult<IEnumerable<TimeBookingSummary>>> GetTimeBookingSummary(string financeYear)
        {
            return await GetData(financeYear).ToListAsync();
        }

        [HttpGet("{financeYear}/Excel")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetTimeBookingSummaryExcel(string financeYear)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: GetData(financeYear),
                new []
                {
                    new PivotOptions
                    {
                        PivotName = "PivotTable",
                        PivotRows = new[] { "Team", "Person" },
                        PivotColumns = new[] { "Sprint" },
                        PivotValues = new[] { "Hours" }
                    }
                });
            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, financeYear));
        }

        private IQueryable<TimeBookingSummary> GetData(string financeYear)
        {
            return _context.TimeBookingSummaries.Where(bs => bs.FinanceYear == financeYear);
        }
    }


}
