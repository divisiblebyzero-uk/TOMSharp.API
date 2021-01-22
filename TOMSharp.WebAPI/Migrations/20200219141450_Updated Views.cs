using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class UpdatedViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vPeople]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vPeople] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vPeople AS
select [People].[Name],
       [People].[Team] as [Team],
       [People].[ResourceType] as [Resource type],
       [People].[EmailAddress] as [Email address],
       [People].[StartDate] as [Start Date],
       [People].[EndDate] as [End Date]
  from [People]
GO

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vPeople]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vPeople] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vPeople AS
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
    }
}
