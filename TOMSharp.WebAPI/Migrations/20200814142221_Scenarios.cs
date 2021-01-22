using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class Scenarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scenario",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    financeYear = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(type: "date", nullable: true),
                    Updated = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioForecast",
                columns: table => new
                {
                    ScenarioId = table.Column<int>(nullable: false),
                    ForecastId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioForecast", x => new { x.ScenarioId, x.ForecastId });
                    table.ForeignKey(
                        name: "FK_ScenarioForecast_Forecasts_ForecastId",
                        column: x => x.ForecastId,
                        principalTable: "Forecasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioForecast_Scenario_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioProject",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenarioId = table.Column<int>(nullable: false),
                    Project = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioProject_Scenario_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioForecast_ForecastId",
                table: "ScenarioForecast",
                column: "ForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioProject_ScenarioId",
                table: "ScenarioProject",
                column: "ScenarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioForecast");

            migrationBuilder.DropTable(
                name: "ScenarioProject");

            migrationBuilder.DropTable(
                name: "Scenario");
        }
    }
}
