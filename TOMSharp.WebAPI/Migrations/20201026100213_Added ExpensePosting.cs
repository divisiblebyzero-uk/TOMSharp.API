using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class AddedExpensePosting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpensePostings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "date", nullable: true),
                    Updated = table.Column<DateTime>(type: "date", nullable: true),
                    AsOfDate = table.Column<DateTime>(type: "date", nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    AmountGBP = table.Column<decimal>(nullable: false),
                    Project = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Vendor = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensePostings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpensePostings");
        }
    }
}
