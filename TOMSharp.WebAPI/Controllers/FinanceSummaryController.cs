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

using System.Collections;
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
    public class FinanceSummaryController : ControllerBase
    {
        private readonly TogglContext _context;
        private readonly SpreadsheetService _spreadsheetService;

        public FinanceSummaryController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }

        private async Task<IEnumerable<FinanceSummary>> GetData(string financeYear, bool includeNonCapitalised)
        {
            var summaries = _context.FinanceSummaries.Where(fs => fs.FinanceYear == financeYear);
            if (!includeNonCapitalised)
            {
                summaries = summaries.Where(fs => fs.Capitalised == "Yes");
            }
            return await summaries.OrderBy(fs => fs.MonthStartDate).ThenBy(fs => fs.Project).ToListAsync();
        }

        // GET: /api/FinanceSummary/FY21
        [HttpGet("{financeYear}")]
        public async Task<IEnumerable> GetFinanceSummary(string financeYear, bool includeNonCapitalised)
        {
            return await Task.Run(() =>
            {

                var dbResults2 = GetData(financeYear, includeNonCapitalised).Result;

                return dbResults2.GroupBy(
                    fs => new {fs.Project, fs.BudgetLine, fs.WIPCode, fs.Activity, fs.Capitalised, fs.ResourceType},
                    fs => fs,
                    (key, group) =>
                    {
                        var months = new Dictionary<string, decimal>();
                        foreach (var financeSummary in @group)
                        {
                            months[financeSummary.FinanceMonth] = financeSummary.Charge;
                        }
                        return new
                        {
                            key.Project, key.BudgetLine, key.WIPCode, key.Activity, key.Capitalised, key.ResourceType,
                            Months = months
                        };
                    
                    }
                );
            });
        }

        [HttpGet("{financeYear}/Excel")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetTimeBookingSummaryExcel(string financeYear, bool includeNonCapitalised)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: await GetData(financeYear, includeNonCapitalised),
                pivots: new []
                {
                    new PivotOptions
                    {
                        PivotName = "PivotTable",
                        PivotRows = new[] { "Capitalised", "WIPCode", "BudgetLine", "ResourceType" },
                        PivotColumns = new[] { "FinanceMonth" },
                        PivotValues = new[] { "Charge" }
                    }
                });
               
            string paramString = (includeNonCapitalised ? $"{financeYear}-full" : financeYear);
            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, paramString));
        }

    }

}
