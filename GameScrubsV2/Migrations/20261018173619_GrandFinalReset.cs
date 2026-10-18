using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameScrubsV2.Migrations
{
    /// <summary>
    /// Adds the grand final reset for double elimination brackets.
    ///
    /// - Brackets.GrandFinalReset: existing brackets default to false so they keep the rules they were played with
    /// - A reset match position per double elimination type: r1 (winners bracket player) vs r2 (losers bracket player),
    ///   whose winner goes to the same champion slot as the grand finals
    /// - r1 and r2 are scored 2, the same as the grand finals players, since the loser of the reset finishes 2nd
    /// </summary>
    public partial class GrandFinalReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GrandFinalReset",
                table: "Brackets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[BracketPositions] ([Type], [Player1], [Player2], [WinLocation], [LoseLocation], [MarkerPosition])
                VALUES
                (3, 'r1', 'r2', 'w17', NULL, NULL),
                (4, 'r1', 'r2', 'w33', NULL, NULL),
                (5, 'r1', 'r2', 'w64', NULL, NULL);

                INSERT INTO [dbo].[BracketScores] ([Type], [BracketPlace], [Score])
                VALUES
                (3, 'r1', 2), (3, 'r2', 2),
                (4, 'r1', 2), (4, 'r2', 2),
                (5, 'r1', 2), (5, 'r2', 2);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[Placements] WHERE [BracketPlace] IN ('r1', 'r2');
                DELETE FROM [dbo].[BracketPositions] WHERE [Player1] = 'r1' AND [Player2] = 'r2';
                DELETE FROM [dbo].[BracketScores] WHERE [BracketPlace] IN ('r1', 'r2');
            ");

            migrationBuilder.DropColumn(
                name: "GrandFinalReset",
                table: "Brackets");
        }
    }
}
