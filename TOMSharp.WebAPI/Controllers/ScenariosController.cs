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
    public class ScenariosController : ControllerBase
    {
        private readonly ILogger<ScenariosController> _logger;
        private readonly TogglContext _context;

        public ScenariosController(ILogger<ScenariosController> logger, TogglContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Scenarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scenario>>> GetScenarios()
        {
            _logger.LogInformation("Downloading Scenarios");
            return await _context.Scenarios.OrderBy(f => f.Created).ToListAsync();
        }

        // GET: api/Scenarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetScenario(int id)
        {
            var scenario = await _context.Scenarios
                .Select(s => new ScenarioDto
                {
                    Id = s.Id,
                    Created = s.Created,
                    Updated = s.Updated,
                    FinanceYear = s.FinanceYear,
                    Name = s.Name,
                    Forecasts = s.ScenarioForecasts.OrderBy(sf => sf.Priority).Select(sf => sf.Forecast).ToList(),
                    Projects = s.ScenarioProjects.Select(sp => sp.Project).ToList()
                })
                .FirstOrDefaultAsync(s => s.Id == id);
            if (scenario == null)
            {
                return NotFound();
            }

            return scenario;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> EditScenario(int id, ScenarioDto updatedScenario)
        {
            if (id != updatedScenario.Id)
            {
                return BadRequest();
            }

            var scenarioToUpdate = _context.Scenarios
                .Include(s => s.ScenarioForecasts)
                .ThenInclude(sf => sf.Forecast)
                .Include(s => s.ScenarioProjects)
                .Single(s => s.Id == id);
            scenarioToUpdate.Name = updatedScenario.Name;
            scenarioToUpdate.FinanceYear = updatedScenario.FinanceYear;
            scenarioToUpdate.Updated = DateTime.Now;

            var deletedForecasts =
                scenarioToUpdate.ScenarioForecasts.Where(sf => !updatedScenario.Forecasts.Exists(f => f.Id == sf.ForecastId)).ToList();
            var addedForecasts =
                updatedScenario.Forecasts.Except(scenarioToUpdate.ScenarioForecasts.Select(sf => sf.Forecast)).ToList();

            deletedForecasts.ForEach(scenarioForecast =>
            {
                _context.Entry(scenarioForecast).State = EntityState.Deleted;
                scenarioToUpdate.ScenarioForecasts.Remove(
                    scenarioToUpdate.ScenarioForecasts.First(sf => sf.ForecastId == scenarioForecast.ForecastId));
            });
            int priority = 0;
            updatedScenario.Forecasts.ForEach(forecast =>
            {
                if (!scenarioToUpdate.ScenarioForecasts.Exists(sf => sf.ForecastId == forecast.Id))
                {
                    var sf = new ScenarioForecast
                    {
                        Forecast = forecast,
                        ForecastId = forecast.Id,
                        Priority = priority,
                        Scenario = scenarioToUpdate,
                        ScenarioId = scenarioToUpdate.Id
                    };
                    scenarioToUpdate.ScenarioForecasts.Add(sf);
                    _context.Entry(sf).State = EntityState.Added;
                }
                else
                {
                    var sf = scenarioToUpdate.ScenarioForecasts.Single(sf => sf.ForecastId == forecast.Id);
                    sf.Priority = priority;
                    _context.Entry(sf).State = EntityState.Modified;
                }

                priority++;
            });
            
                        
            var deletedProjects =
                scenarioToUpdate.ScenarioProjects.Where(sp => !updatedScenario.Projects.Exists(p => p == sp.Project)).ToList();
            var addedProjects =
                updatedScenario.Projects.Except(scenarioToUpdate.ScenarioProjects.Select(sp => sp.Project)).ToList();
                
            deletedProjects.ForEach(scenarioProject =>
            {
                _context.Entry(scenarioProject).State = EntityState.Deleted;
                scenarioToUpdate.ScenarioProjects.Remove(scenarioToUpdate.ScenarioProjects.First(sp => sp.Id == scenarioProject.Id));
            });
            addedProjects.ForEach(project =>
            {
                var sp = new ScenarioProject
                {
                    ScenarioId = scenarioToUpdate.Id,
                    Scenario = scenarioToUpdate,
                    Project = project
                };
                _context.Entry(sp).State = EntityState.Added;
                scenarioToUpdate.ScenarioProjects.Add(sp);
            });
            
            _context.Entry(scenarioToUpdate).State = EntityState.Modified;


            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScenarioExists(id))
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
/*
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
*/
        // POST: api/Scenarios
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<ActionResult<Scenario>> AddScenario(Scenario scenario)
        {
            scenario.Created = DateTime.Now;
            scenario.Updated = DateTime.Now;
            await _context.Scenarios.AddAsync(scenario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddScenario", new { id = scenario.Id }, scenario);
        }

        private bool ScenarioExists(int id)
        {
            return _context.Scenarios.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> DeleteScenario(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var scenario = await _context.Scenarios.FirstOrDefaultAsync(s => s.Id == id);
            if (scenario == null)
            {
                return NotFound();
            }
            _context.Scenarios.Remove(scenario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}