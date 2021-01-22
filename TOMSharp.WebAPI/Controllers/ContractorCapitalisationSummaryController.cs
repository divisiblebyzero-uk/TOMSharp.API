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
    public class ContractorCapitalisationSummaryController : Controller
    {
        private readonly TogglContext _context;
        private readonly SpreadsheetService _spreadsheetService;

        public ContractorCapitalisationSummaryController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }

        // GET: /api/ContractorCapitalisationSummary/FY21
        [HttpGet("{financeYear}")]
        public async Task<ActionResult<IEnumerable<ContractorCapitalisationSummary>>> GetContractorCapitalisationSummary(string financeYear)
        {
            return await GetData(financeYear).ToListAsync();
        }

        [HttpGet("{financeYear}/Excel")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetContractorCapitalisationSummaryExcel(string financeYear)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: GetData(financeYear),
                pivots: new []
                {
                    new PivotOptions
                    {
                        PivotName = "PivotTable",
                        PivotRows = new[] { "CapexString", "Employer" },
                        PivotColumns = new[] { "FinanceMonth" },
                        PivotValues = new[] { "Charge" }
                    }
                });

            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, financeYear));
        }

        private IQueryable<ContractorCapitalisationSummary> GetData(string financeYear)
        {
            return _context.ContractorCapitalisationSummaries
                .Where(ccs => ccs.FinanceYear == financeYear)
                .OrderBy(ccs => ccs.MonthStartDate);
        }

    }


}
