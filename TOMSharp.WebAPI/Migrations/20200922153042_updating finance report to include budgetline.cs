using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class updatingfinancereporttoincludebudgetline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFinanceSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vFinanceSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vFinanceSummary AS
select BudgetLines.Name as BudgetLine, isnull([Projects].[Name], Project) as Project, [Projects].[FinanceWIPCode] as WIPCode, [Projects].[Activity] as Activity, case when [Projects].[Activity] = 'Development' then 'Yes' else 'No' end as Capitalised, FinanceMonth, Sprints.FinanceYear, ResourceType, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join [Projects] on timeentries.project = [Projects].[TimeEntryName]
  left join people on timeentries.[User] = people.Name
  left join [BudgetLines] on [Projects].[BudgetLineId] = [BudgetLines].[Id]
 group by projects.[Name], [Projects].[FinanceWIPCode], [Projects].[Activity], Project, Sprints.FinanceMonth, Sprints.FinanceYear, ResourceType, BudgetLines.Name
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFinanceSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vFinanceSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vFinanceSummary AS
select isnull([Projects].[Name], Project) as Project, [Projects].[FinanceWIPCode] as WIPCode, [Projects].[Activity] as Activity, case when [Projects].[Activity] = 'Development' then 'Yes' else 'No' end as Capitalised, FinanceMonth, FinanceYear, ResourceType, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join [Projects] on timeentries.project = [Projects].[TimeEntryName]
  left join people on timeentries.[User] = people.Name
 group by projects.[Name], [Projects].[FinanceWIPCode], [Projects].[Activity], Project, FinanceMonth, FinanceYear, ResourceType
GO
");
        }
    }
}
