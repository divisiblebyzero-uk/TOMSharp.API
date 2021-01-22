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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceProjectMappingsController
    {
        private readonly TogglContext _context;

        public FinanceProjectMappingsController(TogglContext context)
        {
            _context = context;
        }

        // GET: api/FinanceProjectMappings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinanceProjectMapping>>> GetMappings()
        {
            return await _context.FinanceProjectMappings.ToListAsync();
        }
    }
}
