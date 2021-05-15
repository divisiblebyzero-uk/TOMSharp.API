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

using Microsoft.EntityFrameworkCore;
using TOMSharp_Loader.Model;

namespace TOMSharp_Loader.Service
{
    public class TogglContext : DbContext
    {
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<FinanceSummary> FinanceSummaries { get; set; }
        public DbSet<TimeBookingSummary> TimeBookingSummaries { get; set; }
        public DbSet<SprintSummary> SprintSummaries { get; set; }
        public DbSet<ContractorCapitalisationSummary> ContractorCapitalisationSummaries { get; set; }
        public DbSet<TimeBookingException> TimeBookingExceptions { get; set; }
        public DbSet<Forecast> Forecasts { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioReport> ScenarioReports { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeEntryReportItem> TimeEntryReportItems { get; set; }
        public DbSet<ExpensePosting> ExpensePostings { get; set; }
        
        public TogglContext(DbContextOptions<TogglContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Sprint>().HasIndex(s => s.Name).IsUnique();
            builder.Entity<Person>().HasIndex(p => p.Name).IsUnique();
            builder.Entity<FinanceSummary>(fs =>
            {
                fs.HasNoKey();
                fs.ToView("vFinanceSummary");
            });
            builder.Entity<TimeBookingSummary>(bs =>
            {
                bs.HasNoKey();
                bs.ToView("vTimeBookingSummary");
            });
            builder.Entity<SprintSummary>(ss =>
            {
                ss.HasNoKey();
                ss.ToView("vSprintSummary");
            });
            builder.Entity<ContractorCapitalisationSummary>(ccs =>
            {
                ccs.HasNoKey();
                ccs.ToView("vContractorCapitalisationSummary");
            });
            builder.Entity<TimeBookingException>(tbe =>
            {
                tbe.HasNoKey();
                tbe.ToView("vTimeBookingExceptions");
            });
            builder.Entity<TimeEntryReportItem>(teri =>
            {
                teri.HasNoKey();
                teri.ToView("vTimeEntryReport");
            });

            builder.Entity<ScenarioForecast>().HasKey(sf => new {sf.ScenarioId, sf.ForecastId});
            builder.Entity<ScenarioForecast>()
                .HasOne(sf => sf.Scenario)
                .WithMany(s => s.ScenarioForecasts)
                .HasForeignKey(sf => sf.ScenarioId);
            builder.Entity<ScenarioForecast>()
                .HasOne(sf => sf.Forecast)
                .WithMany(f => f.ScenarioForecasts)
                .HasForeignKey(sf => sf.ForecastId);
            
            builder.Entity<ScenarioReport>(sr =>
            {
                sr.HasNoKey();
                sr.ToView("vScenarioReport");
            });
        }
    }
}
