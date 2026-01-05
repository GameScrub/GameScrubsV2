using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameScrubsV2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBracketColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Brackets");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Brackets");

            migrationBuilder.AlterColumn<int>(
                name: "LockCode",
                table: "Brackets",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LockCode",
                table: "Brackets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Brackets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Brackets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
