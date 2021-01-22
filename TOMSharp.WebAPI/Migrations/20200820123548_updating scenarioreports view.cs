using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class updatingscenarioreportsview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "financeYear",
                table: "Forecasts",
                newName: "FinanceYear");
            
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vScenarioReport]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vScenarioReport] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vScenarioReport AS
select null as ScenarioId, 'TimeEntry' as ForecastName, isnull(FinanceName, Project) as Project, FinanceMonth, FinanceYear, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate, sprints.Name as Sprint, sprints.Id as SprintId
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join financeprojectmappings on timeentries.project = financeprojectmappings.projectname
  left join people on timeentries.[User] = people.Name
 group by FinanceName, Project, FinanceMonth, FinanceYear, sprints.Name, sprints.Id
union
select distinct ScenarioId,
                first_value(ForecastName) over (partition by ScenarioId, Project, FinanceMonth, FinanceYear, MonthStartDate, Sprint order by priority) as ForecastName,
				Project,
				FinanceMonth,
				FinanceYear,
				first_value(Hours) over (partition by ScenarioId, Project, FinanceMonth, FinanceYear, MonthStartDate, Sprint order by priority) as Hours,
				MonthStartDate,
				Sprint,
                SprintId
  from 
(select scenarios.id as ScenarioId, forecasts.id as ForecastId, forecasts.Name as ForecastName, scenarioforecast.Priority, forecastentry.Project as Project, sprints.FinanceMonth as FinanceMonth, sprints.FinanceYear as FinanceYear, Hours, min(sprints.startDate) as MonthStartDate, sprints.Name as Sprint, sprints.Id as SprintId
  from scenarios
  join scenarioforecast on scenarios.id = scenarioforecast.scenarioid
  join forecasts on scenarioforecast.forecastid = forecasts.id
  join forecastentry on forecasts.id = forecastentry.forecastid
  join sprints on forecastentry.Sprint = Sprints.Name
 where forecastentry.Project in (select scenarioproject.Project from scenarioproject where scenarioproject.ScenarioId = scenarios.id)
 group by scenarios.id, forecasts.id, forecasts.Name, scenarioforecast.priority, forecastentry.Project, sprints.FinanceMonth, sprints.FinanceYear, Hours, sprints.startDate, sprints.Name, sprints.Id
having Priority = (select min(priority) from scenarioforecast where scenarioid = scenarios.id and scenarioforecast.forecastid = forecasts.id)) t
GO
");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinanceYear",
                table: "Forecasts",
                newName: "financeYear");
            
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vScenarioReport]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vScenarioReport] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vScenarioReport AS
select null as ScenarioId, 'TimeEntry' as ForecastName, isnull(FinanceName, Project) as Project, FinanceMonth, FinanceYear, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate, sprints.Name as Sprint
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join financeprojectmappings on timeentries.project = financeprojectmappings.projectname
  left join people on timeentries.[User] = people.Name
 group by FinanceName, Project, FinanceMonth, FinanceYear, sprints.Name
union
select distinct ScenarioId,
                first_value(ForecastName) over (partition by ScenarioId, Project, FinanceMonth, FinanceYear, MonthStartDate, Sprint order by priority) as ForecastName,
				Project,
				FinanceMonth,
				FinanceYear,
				first_value(Hours) over (partition by ScenarioId, Project, FinanceMonth, FinanceYear, MonthStartDate, Sprint order by priority) as Hours,
				MonthStartDate,
				Sprint
  from 
(select scenarios.id as ScenarioId, forecasts.id as ForecastId, forecasts.Name as ForecastName, scenarioforecast.Priority, forecastentry.Project as Project, sprints.FinanceMonth as FinanceMonth, sprints.FinanceYear as FinanceYear, Hours, min(sprints.startDate) as MonthStartDate, sprints.Name as Sprint
  from scenarios
  join scenarioforecast on scenarios.id = scenarioforecast.scenarioid
  join forecasts on scenarioforecast.forecastid = forecasts.id
  join forecastentry on forecasts.id = forecastentry.forecastid
  join sprints on forecastentry.Sprint = Sprints.Name
 where forecastentry.Project in (select scenarioproject.Project from scenarioproject where scenarioproject.ScenarioId = scenarios.id)
 group by scenarios.id, forecasts.id, forecasts.Name, scenarioforecast.priority, forecastentry.Project, sprints.FinanceMonth, sprints.FinanceYear, Hours, sprints.startDate, sprints.Name
having Priority = (select min(priority) from scenarioforecast where scenarioid = scenarios.id and scenarioforecast.forecastid = forecasts.id)) t
GO
");
        }
    }
}
