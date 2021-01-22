using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class Updatedtimebookingsummaryview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeBookingSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeBookingSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeBookingSummary AS
select s.Name as Sprint, p.Team as Team, p.Name as Person, sum(te.dur)/1000/60/60 as Hours, s.FinanceYear as FinanceYear
  from timeentries te,
       sprints s,
       people p
 where te.[User] = p.[Name]
   and te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
 group by s.Name, p.Team, p.Name, s.FinanceYear
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
