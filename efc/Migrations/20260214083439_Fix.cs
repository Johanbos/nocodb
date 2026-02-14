using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace efc.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureFlagAssignments_FeatureFlagUsers_FeatureFlagUserId",
                table: "FeatureFlagAssignments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FeatureFlagAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureFlagUserId",
                table: "FeatureFlagAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureFlagAssignments_FeatureFlagUsers_FeatureFlagUserId",
                table: "FeatureFlagAssignments",
                column: "FeatureFlagUserId",
                principalTable: "FeatureFlagUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureFlagAssignments_FeatureFlagUsers_FeatureFlagUserId",
                table: "FeatureFlagAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "FeatureFlagUserId",
                table: "FeatureFlagAssignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FeatureFlagAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureFlagAssignments_FeatureFlagUsers_FeatureFlagUserId",
                table: "FeatureFlagAssignments",
                column: "FeatureFlagUserId",
                principalTable: "FeatureFlagUsers",
                principalColumn: "Id");
        }
    }
}
