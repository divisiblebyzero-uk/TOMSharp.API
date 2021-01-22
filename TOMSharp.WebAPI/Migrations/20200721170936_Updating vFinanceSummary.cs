using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class UpdatingvFinanceSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFinanceSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vFinanceSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vFinanceSummary AS
select isnull(FinanceName, Project) as Project, case when Capitalised = 1 then 'Yes' else 'No' end as Capitalised, FinanceMonth, FinanceYear, ResourceType, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join financeprojectmappings on timeentries.project = financeprojectmappings.projectname
  left join people on timeentries.[User] = people.Name
 group by FinanceName, Project, Capitalised, FinanceMonth, FinanceYear, ResourceType
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFinanceSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vFinanceSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vFinanceSummary AS
select isnull(FinanceName, Project) as Project, case when Capitalised = 1 then 'Yes' else 'No' end as Capitalised, FinanceMonth, FinanceYear, sum(convert(decimal(16,2), dur))/1000/60/60 as Hours, min(sprints.startDate) as MonthStartDate
  from timeentries
  join sprints on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
  left join financeprojectmappings on timeentries.project = financeprojectmappings.projectname
 group by FinanceName, Project, Capitalised, FinanceMonth, FinanceYear
");
        }
    }
}
