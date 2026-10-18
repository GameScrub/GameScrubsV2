using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameScrubsV2.Migrations
{
    /// <summary>
    /// Crosses over the losers bracket drops so players who already met in the winners bracket
    /// are not paired again right away in the losers bracket (standard double elimination seeding).
    ///
    /// - Winners R2 losers drop in reverse order (top of the winners bracket → bottom of the losers bracket)
    /// - Later winners rounds (R3 for 16/32, R4 for 32) drop with each adjacent pair swapped
    ///
    /// The losers matches that receive the drops get their MarkerPosition updated so the
    /// roman numeral markers still point at the winners match that feeds them.
    /// </summary>
    public partial class LosersBracketCrossover : Migration
    {
        // (Type, Player1, Player2, Old LoseLocation, New LoseLocation) for winners matches
        private static readonly (int Type, string Player1, string Player2, string Old, string New)[] LoseLocationChanges =
        [
            // Double Elimination 8-player (Type 3) - Winners R2
            (3, "w9", "w10", "l6", "l8"),
            (3, "w11", "w12", "l8", "l6"),

            // Double Elimination 16-player (Type 4) - Winners R2
            (4, "w17", "w18", "l10", "l16"),
            (4, "w19", "w20", "l12", "l14"),
            (4, "w21", "w22", "l14", "l12"),
            (4, "w23", "w24", "l16", "l10"),
            // Winners R3
            (4, "w25", "w26", "l22", "l24"),
            (4, "w27", "w28", "l24", "l22"),

            // Double Elimination 32-player (Type 5) - Winners R2
            (5, "w33", "w34", "l25", "l32"),
            (5, "w35", "w36", "l26", "l31"),
            (5, "w37", "w38", "l27", "l30"),
            (5, "w39", "w40", "l28", "l29"),
            (5, "w41", "w42", "l29", "l28"),
            (5, "w43", "w44", "l30", "l27"),
            (5, "w45", "w46", "l31", "l26"),
            (5, "w47", "w48", "l32", "l25"),
            // Winners R3
            (5, "w49", "w50", "l45", "l46"),
            (5, "w51", "w52", "l46", "l45"),
            (5, "w53", "w54", "l47", "l48"),
            (5, "w55", "w56", "l48", "l47"),
            // Winners R4
            (5, "w57", "w58", "l55", "l56"),
            (5, "w59", "w60", "l56", "l55"),
        ];

        // (Type, Player1, Player2, Old MarkerPosition, New MarkerPosition) for losers matches receiving the drops
        private static readonly (int Type, string Player1, string Player2, int Old, int New)[] MarkerChanges =
        [
            // Double Elimination 8-player (Type 3) - Losers R2
            (3, "l5", "l6", 3, 4),
            (3, "l7", "l8", 4, 3),

            // Double Elimination 16-player (Type 4) - Losers R2
            (4, "l9", "l10", 5, 8),
            (4, "l11", "l12", 6, 7),
            (4, "l13", "l14", 7, 6),
            (4, "l15", "l16", 8, 5),
            // Losers R4
            (4, "l21", "l22", 9, 10),
            (4, "l23", "l24", 10, 9),

            // Double Elimination 32-player (Type 5) - Losers R2
            (5, "l17", "l25", 9, 16),
            (5, "l18", "l26", 10, 15),
            (5, "l19", "l27", 11, 14),
            (5, "l20", "l28", 12, 13),
            (5, "l21", "l29", 13, 12),
            (5, "l22", "l30", 14, 11),
            (5, "l23", "l31", 15, 10),
            (5, "l24", "l32", 16, 9),
            // Losers R4
            (5, "l41", "l45", 17, 18),
            (5, "l42", "l46", 18, 17),
            (5, "l43", "l47", 19, 20),
            (5, "l44", "l48", 20, 19),
            // Losers R6
            (5, "l53", "l55", 21, 22),
            (5, "l54", "l56", 22, 21),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => Apply(migrationBuilder, reverse: false);

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => Apply(migrationBuilder, reverse: true);

        private static void Apply(MigrationBuilder migrationBuilder, bool reverse)
        {
            foreach (var (type, player1, player2, oldLocation, newLocation) in LoseLocationChanges)
            {
                var (from, to) = reverse ? (newLocation, oldLocation) : (oldLocation, newLocation);

                migrationBuilder.Sql($@"
                    UPDATE [dbo].[BracketPositions] SET [LoseLocation] = '{to}'
                    WHERE [Type] = {type} AND [Player1] = '{player1}' AND [Player2] = '{player2}' AND [LoseLocation] = '{from}';");
            }

            foreach (var (type, player1, player2, oldMarker, newMarker) in MarkerChanges)
            {
                var (from, to) = reverse ? (newMarker, oldMarker) : (oldMarker, newMarker);

                migrationBuilder.Sql($@"
                    UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = {to}
                    WHERE [Type] = {type} AND [Player1] = '{player1}' AND [Player2] = '{player2}' AND [MarkerPosition] = {from};");
            }
        }
    }
}
