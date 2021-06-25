using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TOMSharp_Loader.Model;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportController : Controller
    {
        // This controller performs import/export functions, designed for dev use only (ie copy data from prod to dev)
        // For proper backup control, use db-level operations
        //
        // Limitations:
        //  - the exporter will faithfully export JSON representations of the specified object types
        //    (currently sprints, people, budgetlines, projects and timeentries)
        //  - the importer will attempt to process a ZIP containing JSON representations of the same object types
        //    Notes:
        //     1) Each object type is processed separately with a table deletion followed by re-insertion.
        //        ie. if (eg) people import fails, then the db could be in an inconsistent state!
        //     2) Budgetlines are only imported if there is a refered project. Budgetlines without projects will NOT
        //        be imported.
        
        private const string ZipMimeType = "application/zip";
        private readonly ILogger<ImportExportController> _logger;
        private readonly TogglContext _context;

        public ImportExportController(TogglContext context, ILogger<ImportExportController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public string GetExportFilename()
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            return $"TOM-Export-{dateString}";
        }

        private async Task<List<T>> GetListFromStream<T>(Stream stream)
        {
            await using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var unzippedArray = ms.ToArray();
            var jsonString = Encoding.Unicode.GetString(unzippedArray);
            return JsonConvert.DeserializeObject<List<T>>(jsonString);            
        }

        private async Task<IActionResult> ProcessEntry(ZipArchiveEntry entry)
        {
            _logger.LogInformation($"Processing entry: {entry.Name}");
            if (entry.Name == "sprints.json")
            {
                await ProcessSprints(entry);
            }
            else if (entry.Name == "people.json")
            {
                await ProcessPeople(entry);
            }
            else if (entry.Name == "budgetlines.json")
            {
                // Do nothing, as the budgetlines are added as part of the project import
                //await ProcessBudgetLines(entry);
            }
            else if (entry.Name == "projects.json")
            {
                await ProcessProjects(entry);
            }
            else if (entry.Name == "timeentries.json")
            {
                await ProcessTimeEntries(entry);
            }

            return Ok();
        }

        private async Task<IActionResult> ProcessSprints(ZipArchiveEntry entry)
        {
            var sprints = GetListFromStream<Sprint>(entry.Open()).Result;
            foreach (var sprint in sprints)
            {
                sprint.Id = 0;
            }
            _context.Sprints.RemoveRange(_context.Sprints);
            await _context.SaveChangesAsync();
            await _context.Sprints.AddRangeAsync(sprints);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        private async Task<IActionResult> ProcessPeople(ZipArchiveEntry entry)
        {
            var people = GetListFromStream<Person>(entry.Open()).Result;
            foreach (var person in people)
            {
                person.Id = null;
            }
            _context.People.RemoveRange(_context.People);
            await _context.SaveChangesAsync();
            await _context.People.AddRangeAsync(people);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private void LinkOrCreateBudgetLine(Project project, List<BudgetLine> budgetLines)
        {
            var budgetLineFromCache = budgetLines.Find(b =>
                b.Name == project.BudgetLine.Name && b.FinanceYear == project.BudgetLine.FinanceYear);
            if (budgetLineFromCache == null)
            {
                project.BudgetLineId = 0;
                project.BudgetLine.Id = null;
                budgetLines.Add(project.BudgetLine);
            }
            else
            {
                project.BudgetLineId = 0;
                project.BudgetLine = budgetLineFromCache;
            }
        }
        
        private async Task<IActionResult> ProcessProjects(ZipArchiveEntry entry)
        {
            var projects = GetListFromStream<Project>(entry.Open()).Result;
            var budgetLines = new List<BudgetLine>();
            foreach (var project in projects)
            {
                project.Id = null;
                LinkOrCreateBudgetLine(project, budgetLines);
            }
            _context.Projects.RemoveRange(_context.Projects);
            await _context.SaveChangesAsync();
            await _context.Projects.AddRangeAsync(projects);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<IActionResult> ProcessTimeEntries(ZipArchiveEntry entry)
        {
            var timeEntries = GetListFromStream<TimeEntry>(entry.Open()).Result;

            _context.TimeEntries.RemoveRange(_context.TimeEntries);
            await _context.SaveChangesAsync();
            await _context.TimeEntries.AddRangeAsync(timeEntries);
            await _context.SaveChangesAsync();

            return Ok();
        }

        
        [HttpPost]
        [DisableRequestSizeLimit]
        [Authorize(Policy = "RequireAdminPolicy")]
        public async Task<IActionResult> Import()
        {
            var files = Request.Form.Files;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    _logger.LogInformation("Got a file: " + formFile.Name);
                    
                    using var zip = new ZipArchive(formFile.OpenReadStream(), ZipArchiveMode.Read);
                    foreach (var entry in zip.Entries)
                    {
                        await ProcessEntry(entry);
                    }
                }
            }

            return Ok();
        }
        
        private static void AddExportEntry(string name, object data, ZipArchive archive)
        {
            byte[] jsonPayload = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(data));
            var zipEntry = archive.CreateEntry(name + ".json", CompressionLevel.Optimal);
            using var zipStream = zipEntry.Open();
            zipStream.Write(jsonPayload, 0, jsonPayload.Length);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExportAllData()
        {
            var filename = GetExportFilename();

            using var ms = new MemoryStream();
            using (var archive =
                new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                AddExportEntry("sprints", _context.Sprints.ToArray(), archive);
                AddExportEntry("people", _context.People.ToArray(), archive);
                AddExportEntry("budgetlines", _context.BudgetLines.ToArray(), archive);
                AddExportEntry("projects", _context.Projects.ToArray(), archive);
                AddExportEntry("timeentries", _context.TimeEntries.ToArray(), archive);

            }
            return File(ms.ToArray(), "application/zip", filename + ".zip");
        }
    }
}