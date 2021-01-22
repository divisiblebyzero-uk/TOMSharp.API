using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class AddingViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

CREATE VIEW vTimeBookingSummary AS
select s.Name as Sprint, t.Name as Team, p.Name as Person, sum(te.dur)/1000/60/60 as Hours
  from timeentries te,
       sprints s,
       people p,
	   teams t
 where te.[User] = p.[Name]
   and te.[Start] >= s.[StartDate]
   and te.[Start] <= s.[EndDate]
   and p.[TeamId] = t.[Id]
 group by s.Name, t.Name, p.Name

");
            migrationBuilder.Sql(@"
CREATE VIEW vTimeBookingExpectations AS
select s.Name as Sprint,
       p.Name as Person,
	   ((DATEDIFF(dd, CASE WHEN (p.StartDate is not null and p.StartDate > s.StartDate) then p.StartDate else s.StartDate END, CASE WHEN (p.EndDate is not null and p.EndDate < s.EndDate) then p.EndDate else s.EndDate END) + 1)
		-(DATEDIFF(wk, CASE WHEN (p.StartDate is not null and p.StartDate > s.StartDate) then p.StartDate else s.StartDate END, CASE WHEN (p.EndDate is not null and p.EndDate < s.EndDate) then p.EndDate else s.EndDate END) * 2)
		-(CASE WHEN DATENAME(dw, CASE WHEN (p.StartDate is not null and p.StartDate > s.StartDate) then p.StartDate else s.StartDate END) = 'Sunday' THEN 1 ELSE 0 END)
		-(CASE WHEN DATENAME(dw, CASE WHEN (p.EndDate is not null and p.EndDate < s.EndDate) then p.EndDate else s.EndDate END) = 'Saturday' THEN 1 ELSE 0 END)) * 8 as ExpectedHours
  from sprints s,
       people p
 where (p.StartDate is null or p.StartDate <= s.EndDate)
   and (p.EndDate is null or p.EndDate >= s.StartDate);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vTimeBookingExpectations");
            migrationBuilder.Sql("DROP VIEW vTimeBookingSummary");
        }
    }
}
