using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class RemovingProjectMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vContractorCapitalisationSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vContractorCapitalisationSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vContractorCapitalisationSummary AS
select people.Employer as [Employer],
	   people.ResourceType as [ResourceType],
	   sprints.FinanceMonth as [FinanceMonth],
	   sprints.FinanceYear as [FinanceYear],
	   case when isnull(projects.Activity, 'Other') = 'Development' then 1 else 0 end as [NewCapitalised],
	   isnull(sum(convert(decimal(16,2), dur))/1000/60/60, 0) as [Hours]
  from sprints
left join timeentries on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
left join people on timeentries.[User] = people.Name
left join Projects on timeentries.Project = Projects.TimeEntryName
where dur > 0
group by people.Employer, people.ResourceType, sprints.financeMonth, sprints.financeYear, case when isnull(projects.Activity, 'Other') = 'Development' then 1 else 0 end 

GO
");
            migrationBuilder.DropTable(
                name: "FinanceProjectMappings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceProjectMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capitalised = table.Column<bool>(type: "bit", nullable: false),
                    FinanceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceProjectMappings", x => x.Id);
                });
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vContractorCapitalisationSummary]'))
    EXEC sp_executesql N'CREATE VIEW [dbo].[vContractorCapitalisationSummary] AS SELECT ''This is a code stub which will be replaced by an Alter Statement'' as [code_stub]'
GO
ALTER VIEW vContractorCapitalisationSummary AS
select people.Employer as [Employer],
	   people.ResourceType as [ResourceType],
	   sprints.FinanceMonth as [FinanceMonth],
	   sprints.FinanceYear as [FinanceYear],
	   isnull(FinanceProjectMappings.Capitalised, 0) as [Capitalised],
	   isnull(sum(convert(decimal(16,2), dur))/1000/60/60, 0) as [Hours]
  from sprints
left join timeentries on timeentries.start >= sprints.startDate and timeentries.start <= sprints.endDate
left join people on timeentries.[User] = people.Name
left join FinanceProjectMappings on timeentries.Project = FinanceProjectMappings.ProjectName
where dur > 0
group by people.Employer, people.ResourceType, sprints.financeMonth, sprints.financeYear, FinanceProjectMappings.Capitalised
GO
");
            
        }
    }
}
