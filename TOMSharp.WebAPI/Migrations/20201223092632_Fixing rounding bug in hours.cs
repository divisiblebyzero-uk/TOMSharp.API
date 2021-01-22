using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class Fixingroundingbuginhours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeEntryReport]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeEntryReport] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeEntryReport AS
select 
BudgetLines.[Id] as BudgetLinesId,
BudgetLines.[Name] as BudgetLinesName,
BudgetLines.[FinanceYear] as BudgetLinesFinanceYear,
BudgetLines.[Budget] as BudgetLinesBudget,
People.[Id] as PeopleId,
People.[Name] as PeopleName,
People.[EmailAddress] as PeopleEmailAddress,
People.[StartDate] as PeopleStartDate,
People.[EndDate] as PeopleEndDate,
People.[ResourceType] as PeopleResourceType,
People.[Team] as PeopleTeam,
People.[Employer] as PeopleEmployer,
Projects.[Id] as ProjectsId,
Projects.[Name] as ProjectsName,
Projects.[TimeEntryName] as ProjectsTimeEntryName,
Projects.[BudgetLineId] as ProjectsBudgetLineId,
Projects.[FinanceWIPCode] as ProjectsFinanceWIPCode,
Projects.[Type] as ProjectsType,
Projects.[FinanceName] as ProjectsFinanceName,
Projects.[Capitalised] as ProjectsCapitalised,
Projects.[Activity] as ProjectsActivity,
Sprints.[Id] as SprintsId,
Sprints.[Name] as SprintsName,
Sprints.[StartDate] as SprintsStartDate,
Sprints.[EndDate] as SprintsEndDate,
Sprints.[FinanceMonth] as SprintsFinanceMonth,
Sprints.[FinanceYear] as SprintsFinanceYear,
TimeEntries.[Id] as TimeEntriesId,
TimeEntries.[Pid] as TimeEntriesPid,
TimeEntries.[Tid] as TimeEntriesTid,
TimeEntries.[Uid] as TimeEntriesUid,
TimeEntries.[Description] as TimeEntriesDescription,
TimeEntries.[Start] as TimeEntriesStart,
TimeEntries.[End] as TimeEntriesEnd,
TimeEntries.[Updated] as TimeEntriesUpdated,
TimeEntries.[Dur] as TimeEntriesDur,
TimeEntries.[User] as TimeEntriesUser,
TimeEntries.[UseStop] as TimeEntriesUseStop,
TimeEntries.[Client] as TimeEntriesClient,
TimeEntries.[Project] as TimeEntriesProject,
TimeEntries.[ProjectColor] as TimeEntriesProjectColor,
TimeEntries.[ProjectHexColor] as TimeEntriesProjectHexColor,
TimeEntries.[Task] as TimeEntriesTask,
TimeEntries.[Billable] as TimeEntriesBillable,
TimeEntries.[IsBillable] as TimeEntriesIsBillable,
TimeEntries.[Cur] as TimeEntriesCur,
convert(decimal(16,2), timeentries.dur)/1000/60/60 as Hours,
case when [Projects].[Activity] = 'Development' then 'Yes' else 'No' end as Capitalised

from timeentries
left join projects on timeentries.project = projects.timeentryname
left join budgetlines on projects.BudgetLineId = BudgetLines.Id
left join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
left join people on timeentries.[User] = people.Name;
GO
");
                        
                        migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeBookingSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeBookingSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeBookingSummary AS
select s.Id as SprintId, s.Name as Sprint, p.Team as Team, p.Name as Person, convert(decimal(16,2), sum(te.dur))/1000/60/60 as Hours, s.FinanceYear as FinanceYear
  from timeentries te,
       sprints s,
       people p
 where te.[User] = p.[Name]
   and te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
 group by s.Id, s.Name, p.Team, p.Name, s.FinanceYear
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeEntryReport]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeEntryReport] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeEntryReport AS
select 
BudgetLines.[Id] as BudgetLinesId,
BudgetLines.[Name] as BudgetLinesName,
BudgetLines.[FinanceYear] as BudgetLinesFinanceYear,
BudgetLines.[Budget] as BudgetLinesBudget,
People.[Id] as PeopleId,
People.[Name] as PeopleName,
People.[EmailAddress] as PeopleEmailAddress,
People.[StartDate] as PeopleStartDate,
People.[EndDate] as PeopleEndDate,
People.[ResourceType] as PeopleResourceType,
People.[Team] as PeopleTeam,
People.[Employer] as PeopleEmployer,
Projects.[Id] as ProjectsId,
Projects.[Name] as ProjectsName,
Projects.[TimeEntryName] as ProjectsTimeEntryName,
Projects.[BudgetLineId] as ProjectsBudgetLineId,
Projects.[FinanceWIPCode] as ProjectsFinanceWIPCode,
Projects.[Type] as ProjectsType,
Projects.[FinanceName] as ProjectsFinanceName,
Projects.[Capitalised] as ProjectsCapitalised,
Projects.[Activity] as ProjectsActivity,
Sprints.[Id] as SprintsId,
Sprints.[Name] as SprintsName,
Sprints.[StartDate] as SprintsStartDate,
Sprints.[EndDate] as SprintsEndDate,
Sprints.[FinanceMonth] as SprintsFinanceMonth,
Sprints.[FinanceYear] as SprintsFinanceYear,
TimeEntries.[Id] as TimeEntriesId,
TimeEntries.[Pid] as TimeEntriesPid,
TimeEntries.[Tid] as TimeEntriesTid,
TimeEntries.[Uid] as TimeEntriesUid,
TimeEntries.[Description] as TimeEntriesDescription,
TimeEntries.[Start] as TimeEntriesStart,
TimeEntries.[End] as TimeEntriesEnd,
TimeEntries.[Updated] as TimeEntriesUpdated,
TimeEntries.[Dur] as TimeEntriesDur,
TimeEntries.[User] as TimeEntriesUser,
TimeEntries.[UseStop] as TimeEntriesUseStop,
TimeEntries.[Client] as TimeEntriesClient,
TimeEntries.[Project] as TimeEntriesProject,
TimeEntries.[ProjectColor] as TimeEntriesProjectColor,
TimeEntries.[ProjectHexColor] as TimeEntriesProjectHexColor,
TimeEntries.[Task] as TimeEntriesTask,
TimeEntries.[Billable] as TimeEntriesBillable,
TimeEntries.[IsBillable] as TimeEntriesIsBillable,
TimeEntries.[Cur] as TimeEntriesCur,
convert(decimal(16,2), timeentries.dur/1000/60/60) as Hours,
case when [Projects].[Activity] = 'Development' then 'Yes' else 'No' end as Capitalised

from timeentries
left join projects on timeentries.project = projects.timeentryname
left join budgetlines on projects.BudgetLineId = BudgetLines.Id
left join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
left join people on timeentries.[User] = people.Name;
GO
");
                        
                        migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeBookingSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeBookingSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeBookingSummary AS
select s.Id as SprintId, s.Name as Sprint, p.Team as Team, p.Name as Person, convert(decimal(16,2), sum(te.dur)/1000/60/60) as Hours, s.FinanceYear as FinanceYear
  from timeentries te,
       sprints s,
       people p
 where te.[User] = p.[Name]
   and te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
 group by s.Id, s.Name, p.Team, p.Name, s.FinanceYear
");
        }
    }
}
