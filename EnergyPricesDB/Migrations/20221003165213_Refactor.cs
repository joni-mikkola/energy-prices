using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnergyPricesDB.Migrations
{
    public partial class Refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "NordicElectricityPrices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "NordicElectricityPrices");
        }
    }
}
