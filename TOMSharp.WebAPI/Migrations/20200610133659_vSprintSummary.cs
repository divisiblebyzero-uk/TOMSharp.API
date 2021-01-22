using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class vSprintSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vSprintSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vSprintSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vSprintSummary AS
select sprints.name as [Name], 
       sprints.StartDate as [StartDate], 
	   sprints.EndDate as [EndDate],
	   sprints.FinanceMonth as [FinanceMonth],
	   sprints.FinanceYear as [FinanceYear],
	   isnull(sum(convert(decimal(16,2), dur))/1000/60/60, 0) as [Hours],
	   count(distinct timeentries.[User]) as [People],
	   max(timeentries.[Updated]) as [LastUpdated]
  from sprints
left join timeentries on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
group by sprints.name, sprints.startDate, sprints.endDate, sprints.financeMonth, sprints.financeYear
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vSprintSummary");
        }
    }
}
