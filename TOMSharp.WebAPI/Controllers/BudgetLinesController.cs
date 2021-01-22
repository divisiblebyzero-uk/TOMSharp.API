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
using Microsoft.Extensions.Logging;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetLinesController : ControllerBase
    {
        private readonly ILogger<BudgetLinesController> _logger;
        private readonly TogglContext _context;

        public BudgetLinesController(ILogger<BudgetLinesController> logger, TogglContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        // GET: api/BudgetLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetLine>>> GetBudgetLines()
        {
            _logger.LogInformation("Downloading BudgetLines");
            return await _context.BudgetLines.OrderBy(p => p.Name).ToListAsync();
        }
        
        // GET: api/BudgetLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetLine>> GetBudgetLine(int id)
        {
            var budgetLine = await _context.BudgetLines.FirstOrDefaultAsync(bl => bl.Id == id);
            if (budgetLine == null)
            {
                return NotFound();
            }

            return budgetLine;
        }
        
        // PUT: api/BudgetLines/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> EditBudgetLine(int id, BudgetLine budgetLine)
        {
            if (id != budgetLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(budgetLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetLineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        private bool BudgetLineExists(int id)
        {
            return _context.BudgetLines.Any(e => e.Id == id);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> DeleteBudgetLine(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var budgetLine = await _context.BudgetLines.FirstOrDefaultAsync(bl => bl.Id == id);
            if (budgetLine == null)
            {
                return NotFound();
            }
            _context.BudgetLines.Remove(budgetLine);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<ActionResult<BudgetLine>> AddBudgetLine(BudgetLine budgetLine)
        {
            _context.BudgetLines.Add(budgetLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddBudgetLine", new { id = budgetLine.Id }, budgetLine);
        }
    }

}