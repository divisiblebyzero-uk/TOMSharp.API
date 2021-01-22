using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class simplifiedpeoplemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_ResourceTypes_ResourceTypeId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Teams_TeamId",
                table: "People");

            migrationBuilder.DropTable(
                name: "ResourceTypes");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_People_ResourceTypeId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_TeamId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "ResourceTypeId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "People",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "People",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Team",
                table: "People");

            migrationBuilder.AddColumn<int>(
                name: "ResourceTypeId",
                table: "People",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "People",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResourceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_ResourceTypeId",
                table: "People",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_People_TeamId",
                table: "People",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTypes_Name",
                table: "ResourceTypes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_People_ResourceTypes_ResourceTypeId",
                table: "People",
                column: "ResourceTypeId",
                principalTable: "ResourceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Teams_TeamId",
                table: "People",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
