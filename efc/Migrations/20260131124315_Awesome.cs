using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace efc.Migrations
{
    /// <inheritdoc />
    public partial class Awesome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "FeatureFlags",
                type: "datetime(6)",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnabledOn",
                table: "FeatureFlags",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOnUtc",
                table: "FeatureFlags",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "EnabledOn",
                table: "FeatureFlags");

            migrationBuilder.DropColumn(
                name: "UpdatedOnUtc",
                table: "FeatureFlags");
        }
    }
}
