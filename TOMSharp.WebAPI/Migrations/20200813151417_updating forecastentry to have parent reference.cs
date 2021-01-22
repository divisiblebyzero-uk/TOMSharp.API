using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMSharp_Loader.Migrations
{
    public partial class updatingforecastentrytohaveparentreference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForecastEntry_Forecasts_ForecastId",
                table: "ForecastEntry");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                table: "Forecasts",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Forecasts",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "ForecastId",
                table: "ForecastEntry",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ForecastEntry_Forecasts_ForecastId",
                table: "ForecastEntry",
                column: "ForecastId",
                principalTable: "Forecasts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForecastEntry_Forecasts_ForecastId",
                table: "ForecastEntry");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                table: "Forecasts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Forecasts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ForecastId",
                table: "ForecastEntry",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ForecastEntry_Forecasts_ForecastId",
                table: "ForecastEntry",
                column: "ForecastId",
                principalTable: "Forecasts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
