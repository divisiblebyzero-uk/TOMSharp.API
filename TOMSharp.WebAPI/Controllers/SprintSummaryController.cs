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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintSummaryController
    {
        private readonly TogglContext _context;

        public SprintSummaryController(TogglContext context)
        {
            _context = context;
        }

        // GET: /api/SprintSummary/FY21
        [HttpGet("{financeYear}")]
        public async Task<ActionResult<IEnumerable<SprintSummary>>> GetSprintSummary(string financeYear)
        {
            return await _context.SprintSummaries.Where(ss => ss.FinanceYear == financeYear).ToListAsync();

        }
    }
}
