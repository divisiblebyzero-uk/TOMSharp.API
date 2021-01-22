using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioForecast_Scenario_ScenarioId",
                table: "ScenarioForecast");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioProject_Scenario_ScenarioId",
                table: "ScenarioProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario");

            migrationBuilder.RenameTable(
                name: "Scenario",
                newName: "Scenarios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioForecast_Scenarios_ScenarioId",
                table: "ScenarioForecast",
                column: "ScenarioId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioProject_Scenarios_ScenarioId",
                table: "ScenarioProject",
                column: "ScenarioId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioForecast_Scenarios_ScenarioId",
                table: "ScenarioForecast");

            migrationBuilder.DropForeignKey(
                name: "FK_ScenarioProject_Scenarios_ScenarioId",
                table: "ScenarioProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios");

            migrationBuilder.RenameTable(
                name: "Scenarios",
                newName: "Scenario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenario",
                table: "Scenario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioForecast_Scenario_ScenarioId",
                table: "ScenarioForecast",
                column: "ScenarioId",
                principalTable: "Scenario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScenarioProject_Scenario_ScenarioId",
                table: "ScenarioProject",
                column: "ScenarioId",
                principalTable: "Scenario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
