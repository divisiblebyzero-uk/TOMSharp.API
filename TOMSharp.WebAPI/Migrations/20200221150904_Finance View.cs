using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class FinanceView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vFinanceSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vFinanceSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vFinanceSummary AS
select Project, ResourceType, CASE WHEN Client LIKE '%BAU' THEN 'No' ELSE 'Yes' END as Capitalised, FinanceMonth, sum(te.dur)/1000/60/60 as Hours
  from timeentries te,
       people p,
       sprints s
 where te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
   and te.[User] = p.Name
group by Project, ResourceType, Client, FinanceMonth
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vFinanceSummary");
        }
    }
}
