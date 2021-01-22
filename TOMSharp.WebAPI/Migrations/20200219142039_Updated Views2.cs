using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class UpdatedViews2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP VIEW vTimeBookingSummary
GO

CREATE VIEW vTimeBookingSummary AS
select s.Name as Sprint, p.Team as Team, p.Name as Person, sum(te.dur)/1000/60/60 as Hours
  from timeentries te,
       sprints s,
       people p
 where te.[User] = p.[Name]
   and te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
 group by s.Name, p.Team, p.Name

");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
