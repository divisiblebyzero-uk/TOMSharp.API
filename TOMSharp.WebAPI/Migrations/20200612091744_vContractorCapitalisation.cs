using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class vContractorCapitalisation : Migration
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vContractorCapitalisationSummary");
        }
    }
}
