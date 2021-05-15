using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class Projectsgainingafinanceyearproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinanceYear",
                table: "Projects",
                nullable: true);

            migrationBuilder.Sql("update projects set financeyear = 'FY22' where timeentryname like 'FY22/%'");
            migrationBuilder.Sql("update projects set financeyear = 'FY21' where timeentryname like 'FY21/%'");
            migrationBuilder.Sql("update projects set financeyear = 'FY20' where financeyear is null");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinanceYear",
                table: "Projects");
        }
    }
}
