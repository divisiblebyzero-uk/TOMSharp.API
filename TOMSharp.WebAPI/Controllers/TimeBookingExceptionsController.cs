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
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeBookingExceptionsController : Controller
    {
        private readonly TogglContext _context;
        private readonly SpreadsheetService _spreadsheetService;

        public TimeBookingExceptionsController(TogglContext context, SpreadsheetService spreadsheetService)
        {
            _context = context;
            _spreadsheetService = spreadsheetService;
        }

        // GET: /api/TimeBookingExceptions/Sprint%2020-12?includeCompleted=true
        [HttpGet("{sprint}")]
        public IEnumerable<TimeBookingException> GetTimeBookingExceptions(string sprint, bool includeCompleted)
        {
            return GetData(sprint, includeCompleted);
        }

        // GET: /api/TimeBookingExceptions/Sprint%2020-12
        [HttpGet("{sprint}/Excel")]
        [AllowAnonymous]
        public async Task<FileStreamResult> GetTimeBookingExceptionsExcel(string sprint, bool includeCompleted)
        {
            var stream = await _spreadsheetService.GetGenericTableExcel(
                rows: GetData(sprint, includeCompleted).Select(tbe => new
                {
                    tbe.Person, 
                    tbe.ExpectedHours, 
                    tbe.ActualHours, 
                    Errors = tbe.Errors == null || tbe.Errors.Count == 0?"":tbe.Errors.Aggregate((i, j) => i + "," + j)
                }));
            string paramString = $"{sprint}-{includeCompleted}";
            return File(stream, SpreadsheetService.ExcelMimeType, _spreadsheetService.GetFilename(this, paramString));
        }

        private IEnumerable<TimeBookingException> GetData(string sprint, bool includeCompleted)
        {
            if (includeCompleted)
            {
                return _context.TimeBookingExceptions
                    .Where(tbe => tbe.Sprint == sprint);

            }
            else
            {
                return _context.TimeBookingExceptions
                    .Where(tbe => tbe.Sprint == sprint)
                    .AsEnumerable()
                    .Where(tbe => tbe.IsCorrect == false);

            }
        }

    }


}
