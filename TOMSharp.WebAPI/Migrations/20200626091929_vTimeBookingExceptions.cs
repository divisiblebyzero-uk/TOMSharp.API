using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class vTimeBookingExceptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vTimeBookingExceptions]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vTimeBookingExceptions] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vTimeBookingExceptions AS
select vTimeBookingExpectations.Sprint, vTimeBookingExpectations.Person, convert(decimal(16,2), ExpectedHours) as ExpectedHours, Hours as ActualHours
  from vTimeBookingExpectations
left join vTimeBookingSummary
    on vTimeBookingExpectations.Sprint = vTimeBookingSummary.Sprint and vTimeBookingExpectations.Person = vTimeBookingSummary.Person
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vTimeBookingExceptions");
        }
    }
}
