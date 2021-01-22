using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class ANotherview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

CREATE VIEW vPeople AS
select [People].[Name],
       [Teams].[Name] as [Team],
       [ResourceTypes].[Name] as [Resource type],
       [People].[EmailAddress] as [Email address],
       [People].[StartDate] as [Start Date],
       [People].[EndDate] as [End Date]
  from [People], [Teams], [ResourceTypes]
 where [People].[TeamId] = [Teams].[Id]
   and [People].[ResourceTypeId] = [ResourceTypes].[Id]

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vPeople");
        }
    }
}
