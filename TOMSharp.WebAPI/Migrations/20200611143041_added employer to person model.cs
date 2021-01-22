using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class addedemployertopersonmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Employer",
                table: "People",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Employer",
                table: "People");
        }
    }
}
