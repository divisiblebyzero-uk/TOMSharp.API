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

using System;
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
    public class ForecastsController : ControllerBase
    {
        private readonly ILogger<ForecastsController> _logger;
        private readonly TogglContext _context;
        private readonly SnapshotService _snapshotService;

        public ForecastsController(ILogger<ForecastsController> logger, TogglContext context, SnapshotService snapshotService)
        {
            _logger = logger;
            _context = context;
            _snapshotService = snapshotService;
        }

        // GET: api/Forecasts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Forecast>>> GetForecasts()
        {
            _logger.LogInformation("Downloading Forecasts");
            return await _context.Forecasts.OrderBy(f => f.Created).ToListAsync();
        }

        // GET: api/Forecasts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Forecast>> GetForecast(int id)
        {
            var forecast = await _context.Forecasts.Include(f => f.Entries).FirstOrDefaultAsync(f => f.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }

            return forecast;
        }

        // PUT: api/Forecasts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> EditForecast(int id, Forecast forecast)
        {
            if (id != forecast.Id)
            {
                return BadRequest();
            }

            var forecastToUpdate = _context.Forecasts.Include(f => f.Entries)
                .Single(f => f.Id == id);

            forecastToUpdate.Name = forecast.Name;
            forecastToUpdate.Updated = DateTime.Now;

            var deletedEntries = forecastToUpdate.Entries.Except(forecast.Entries).ToList();
            var addedEntries = forecast.Entries.Except(forecastToUpdate.Entries).ToList();

            deletedEntries.ForEach(entry =>
            {
                _context.Entry(entry).State = EntityState.Deleted;
                forecastToUpdate.Entries.Remove(forecastToUpdate.Entries.First(e => e.Id == entry.Id));
            });
            addedEntries.ForEach(entry =>
            {
                _context.Entry(entry).State = EntityState.Added;
                forecastToUpdate.Entries.Add(entry);
            });

            _context.Entry(forecastToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForecastExists(id))
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

        // POST: api/Forecasts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<ActionResult<Person>> AddForecast(Forecast forecast)
        {
            forecast.Created = DateTime.Now;
            forecast.Updated = DateTime.Now;
            _context.Forecasts.Add(forecast);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddForecast", new { id = forecast.Id }, forecast);
        }

        private bool ForecastExists(int id)
        {
            return _context.Forecasts.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> DeleteForecast(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var forecast = await _context.Forecasts.FirstOrDefaultAsync(f => f.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }
            _context.Forecasts.Remove(forecast);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("availableProjects")]
        //TODO get a proper list of projects
        public IEnumerable<string> GetAvailableProjects()
        {
            return _context.TimeEntries.Select(t => t.Project).Distinct();
        }

        [HttpGet("createSnapshot")]
        [AllowAnonymous]
        public void CreateSnapshot(string financeYear, int months = 12, bool extrapolateRemainingTime = false, int? scenarioId = null)
        {
            _snapshotService.CreateSnapshot(financeYear, months, extrapolateRemainingTime, scenarioId);
        }
        
    }
}