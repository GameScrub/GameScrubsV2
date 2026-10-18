using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameScrubsV2.Migrations
{
    /// <summary>
    /// Fixes the Double Elimination 32-player (Type 5) scores to match its grand finals (w63,l61→w64),
    /// following the same pattern as Double 16 (w31,w32→w33: w33 = 1, w31 = 2, w32 = 2):
    ///
    /// - w64 (champion) was scored 2, now 1
    /// - w65 was scored 1 but no match ever places a player there, removed
    /// - l61 (losers bracket winner going to grand finals) had no score, now 2
    ///
    /// Existing 32-player double elimination placements are corrected to the new scores.
    /// </summary>
    public partial class FixDouble32ChampionScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [dbo].[BracketScores] SET [Score] = 1
                WHERE [Type] = 5 AND [BracketPlace] = 'w64' AND [Score] = 2;

                DELETE FROM [dbo].[BracketScores]
                WHERE [Type] = 5 AND [BracketPlace] = 'w65';

                IF NOT EXISTS (SELECT 1 FROM [dbo].[BracketScores] WHERE [Type] = 5 AND [BracketPlace] = 'l61')
                    INSERT INTO [dbo].[BracketScores] ([Type], [BracketPlace], [Score]) VALUES (5, 'l61', 2);

                UPDATE [placement] SET [Score] = 1
                FROM [dbo].[Placements] [placement]
                INNER JOIN [dbo].[Brackets] [bracket] ON [bracket].[ID] = [placement].[BracketID]
                WHERE [bracket].[Type] = 5 AND [placement].[BracketPlace] = 'w64';

                UPDATE [placement] SET [Score] = 2
                FROM [dbo].[Placements] [placement]
                INNER JOIN [dbo].[Brackets] [bracket] ON [bracket].[ID] = [placement].[BracketID]
                WHERE [bracket].[Type] = 5 AND [placement].[BracketPlace] = 'l61';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [placement] SET [Score] = 0
                FROM [dbo].[Placements] [placement]
                INNER JOIN [dbo].[Brackets] [bracket] ON [bracket].[ID] = [placement].[BracketID]
                WHERE [bracket].[Type] = 5 AND [placement].[BracketPlace] = 'l61';

                UPDATE [placement] SET [Score] = 2
                FROM [dbo].[Placements] [placement]
                INNER JOIN [dbo].[Brackets] [bracket] ON [bracket].[ID] = [placement].[BracketID]
                WHERE [bracket].[Type] = 5 AND [placement].[BracketPlace] = 'w64';

                DELETE FROM [dbo].[BracketScores]
                WHERE [Type] = 5 AND [BracketPlace] = 'l61';

                INSERT INTO [dbo].[BracketScores] ([Type], [BracketPlace], [Score]) VALUES (5, 'w65', 1);

                UPDATE [dbo].[BracketScores] SET [Score] = 2
                WHERE [Type] = 5 AND [BracketPlace] = 'w64' AND [Score] = 1;
            ");
        }
    }
}
