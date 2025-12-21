using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameScrubsV2.Migrations
{
    /// <inheritdoc />
    public partial class PositionMarkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MarkerPosition",
                table: "BracketPositions",
                type: "int",
                nullable: true);

            // Populate MarkerPosition values for all bracket types
            // The marker indicates where a player drops to in the losers bracket (or goes to finals)
            //
            // Logic:
            // - Winners bracket matches show which losers position they drop to
            // - Losers bracket matches show the same marker as the winners matches that drop into them
            // - The last losers match before Grand Finals shows -1 (trophy icon)
            // - Grand Finals shows -1 (trophy icon with connector)

            migrationBuilder.Sql(@"
                -- ============================================
                -- Double Elimination 8-player (Type 3)
                -- ============================================
                -- Data reference from BracketData migration:
                -- Winners: (8) w1,w2→w9,l1  (9) w3,w4→w10,l2  (10) w5,w6→w11,l3  (11) w7,w8→w12,l4
                --          (12) w9,w10→w13,l6  (13) w11,w12→w14,l8  (19) w13,w14→w15,l12
                --          (21) w15,w16→w17 [Grand Finals]
                -- Losers:  (14) l1,l2→l5  (15) l3,l4→l7  (16) l5,l6→l9  (17) l7,l8→l10
                --          (18) l9,l10→l11  (20) l11,l12→w16 [Losers Finals]

                -- Winners Round 1: Each match drops to unique losers position
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 3 AND [Player1] = 'w1' AND [Player2] = 'w2' AND [LoseLocation] = 'l1';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 3 AND [Player1] = 'w3' AND [Player2] = 'w4' AND [LoseLocation] = 'l2';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 3 AND [Player1] = 'w5' AND [Player2] = 'w6' AND [LoseLocation] = 'l3';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 3 AND [Player1] = 'w7' AND [Player2] = 'w8' AND [LoseLocation] = 'l4';

                -- Winners Round 2: Sequential markers continuing
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 3 AND [Player1] = 'w9' AND [Player2] = 'w10' AND [LoseLocation] = 'l6';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 3 AND [Player1] = 'w11' AND [Player2] = 'w12' AND [LoseLocation] = 'l8';

                -- Winners Round 3 (Semi-finals): Continues sequence
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 3 AND [Player1] = 'w13' AND [Player2] = 'w14' AND [LoseLocation] = 'l12';

                -- Losers Round 1: Receives from Winners R1 (l1,l2 and l3,l4)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 3 AND [Player1] = 'l1' AND [Player2] = 'l2' AND [WinLocation] = 'l5';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 3 AND [Player1] = 'l3' AND [Player2] = 'l4' AND [WinLocation] = 'l7';

                -- Losers Round 2: Receives from Winners R2 (l5,l6 and l7,l8)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 3 AND [Player1] = 'l5' AND [Player2] = 'l6' AND [WinLocation] = 'l9';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 3 AND [Player1] = 'l7' AND [Player2] = 'l8' AND [WinLocation] = 'l10';

                -- Losers Round 3: No marker needed (internal losers bracket merging)

                -- Losers Finals: Receives from Winners R3 (marker 5) - l11,l12→w16
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 3 AND [Player1] = 'l11' AND [Player2] = 'l12' AND [WinLocation] = 'w16';

                -- Grand Finals: Trophy icon (-1) - w15,w16→w17
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = -1
                WHERE [Type] = 3 AND [Player1] = 'w15' AND [Player2] = 'w16' AND [WinLocation] = 'w17';

                -- ============================================
                -- Double Elimination 16-player (Type 4)
                -- ============================================
                -- Data reference from BracketData migration (records 37-66):
                -- Winners R1: (37) w1,w2→w17,l1  (39) w3,w4→w18,l2  (41) w5,w6→w19,l3  (43) w7,w8→w20,l4
                --             (45) w9,w10→w21,l5  (47) w11,w12→w22,l6  (49) w13,w14→w23,l7  (51) w15,w16→w24,l8
                -- Winners R2: (53) w17,w18→w25,l10  (55) w19,w20→w26,l12  (57) w21,w22→w27,l14  (59) w23,w24→w28,l16
                -- Winners R3: (61) w25,w26→w29,l22  (64) w27,w28→w30,l24
                -- Winners R4: (65) w29,w30→w31,l28
                -- Grand Finals: (66) w31,w32→w33
                -- Losers R1: (38) l1,l2→l9  (40) l3,l4→l11  (42) l5,l6→l13  (44) l7,l8→l15
                -- Losers R2: (46) l9,l10→l17  (48) l11,l12→l18  (50) l13,l14→l19  (52) l15,l16→l20
                -- Losers R3: (54) l17,l18→l21  (56) l19,l20→l23
                -- Losers R4: (58) l21,l22→l25  (60) l23,l24→l26
                -- Losers R5: (62) l25,l26→l27
                -- Losers Finals: (63) l27,l28→w32

                -- Winners Round 1: Each pair of matches drops to consecutive losers positions
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 4 AND [Player1] = 'w1' AND [Player2] = 'w2' AND [LoseLocation] = 'l1';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 4 AND [Player1] = 'w3' AND [Player2] = 'w4' AND [LoseLocation] = 'l2';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 4 AND [Player1] = 'w5' AND [Player2] = 'w6' AND [LoseLocation] = 'l3';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 4 AND [Player1] = 'w7' AND [Player2] = 'w8' AND [LoseLocation] = 'l4';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 4 AND [Player1] = 'w9' AND [Player2] = 'w10' AND [LoseLocation] = 'l5';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 4 AND [Player1] = 'w11' AND [Player2] = 'w12' AND [LoseLocation] = 'l6';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 4 AND [Player1] = 'w13' AND [Player2] = 'w14' AND [LoseLocation] = 'l7';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 4 AND [Player1] = 'w15' AND [Player2] = 'w16' AND [LoseLocation] = 'l8';

                -- Winners Round 2: Sequential markers continuing (5-8)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 4 AND [Player1] = 'w17' AND [Player2] = 'w18' AND [LoseLocation] = 'l10';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 6
                WHERE [Type] = 4 AND [Player1] = 'w19' AND [Player2] = 'w20' AND [LoseLocation] = 'l12';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 7
                WHERE [Type] = 4 AND [Player1] = 'w21' AND [Player2] = 'w22' AND [LoseLocation] = 'l14';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 8
                WHERE [Type] = 4 AND [Player1] = 'w23' AND [Player2] = 'w24' AND [LoseLocation] = 'l16';

                -- Winners Round 3: Sequential markers continuing (9-10)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 9
                WHERE [Type] = 4 AND [Player1] = 'w25' AND [Player2] = 'w26' AND [LoseLocation] = 'l22';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 10
                WHERE [Type] = 4 AND [Player1] = 'w27' AND [Player2] = 'w28' AND [LoseLocation] = 'l24';

                -- Winners Round 4 (Semi-finals): Continues sequence (11)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 11
                WHERE [Type] = 4 AND [Player1] = 'w29' AND [Player2] = 'w30' AND [LoseLocation] = 'l28';

                -- Losers Round 1: Receives from Winners R1 (markers 1-4)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 4 AND [Player1] = 'l1' AND [Player2] = 'l2' AND [WinLocation] = 'l9';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 4 AND [Player1] = 'l3' AND [Player2] = 'l4' AND [WinLocation] = 'l11';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 4 AND [Player1] = 'l5' AND [Player2] = 'l6' AND [WinLocation] = 'l13';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 4 AND [Player1] = 'l7' AND [Player2] = 'l8' AND [WinLocation] = 'l15';

                -- Losers Round 2: Receives from Winners R2 (markers 5-8)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 4 AND [Player1] = 'l9' AND [Player2] = 'l10' AND [WinLocation] = 'l17';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 6
                WHERE [Type] = 4 AND [Player1] = 'l11' AND [Player2] = 'l12' AND [WinLocation] = 'l18';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 7
                WHERE [Type] = 4 AND [Player1] = 'l13' AND [Player2] = 'l14' AND [WinLocation] = 'l19';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 8
                WHERE [Type] = 4 AND [Player1] = 'l15' AND [Player2] = 'l16' AND [WinLocation] = 'l20';

                -- Losers Round 3: No markers (internal losers bracket merging)

                -- Losers Round 4: Receives from Winners R3 (markers 9-10) via l22 and l24
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 9
                WHERE [Type] = 4 AND [Player1] = 'l21' AND [Player2] = 'l22' AND [WinLocation] = 'l25';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 10
                WHERE [Type] = 4 AND [Player1] = 'l23' AND [Player2] = 'l24' AND [WinLocation] = 'l26';

                -- Losers Round 5: No markers (internal losers bracket merging)

                -- Losers Finals: Receives from Winners R4 (marker 11) - l27,l28→w32
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 11
                WHERE [Type] = 4 AND [Player1] = 'l27' AND [Player2] = 'l28' AND [WinLocation] = 'w32';

                -- Grand Finals: Trophy icon (-1) - w31,w32→w33
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = -1
                WHERE [Type] = 4 AND [Player1] = 'w31' AND [Player2] = 'w32' AND [WinLocation] = 'w33';

                -- ============================================
                -- Double Elimination 32-player (Type 5)
                -- ============================================
                -- Data reference from BracketData migration (records 98-159):
                -- Winners R1: 98-113 (16 matches w1,w2→w33,l1 through w31,w32→w48,l16)
                -- Losers R1: 114-121 (8 matches l1,l2→l17 through l15,l16→l24)
                -- Winners R2: 122-129 (8 matches w33,w34→w49,l25 through w47,w48→w56,l32)
                -- Losers R2: 130-137 (8 matches l17,l25→l33 through l24,l32→l40)
                -- Losers R3: 138-141 (4 matches l33,l34→l41 through l39,l40→l44)
                -- Winners R3: 142-145 (4 matches w49,w50→w57,l45 through w55,w56→w60,l48)
                -- Losers R4: 146-149 (4 matches l41,l45→l49 through l44,l48→l52)
                -- Losers R5: 150-151 (2 matches l49,l50→l53 and l51,l52→l54)
                -- Winners R4: 152-153 (2 matches w57,w58→w61,l55 and w59,w60→w62,l56)
                -- Losers R6: 154-155 (2 matches l53,l55→l57 and l54,l56→l58)
                -- Losers R7: 156 (1 match l57,l58→l59)
                -- Winners R5: 157 (1 match w61,w62→w63,l60)
                -- Losers R8: 158 (1 match l59,l60→l61) [Losers Finals]
                -- Grand Finals: 159 (1 match w63,l61→w64)

                -- Winners Round 1: Each pair of matches drops to consecutive losers positions (markers 1-8)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 5 AND [LoseLocation] IN ('l1', 'l2');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 5 AND [LoseLocation] IN ('l3', 'l4');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 5 AND [LoseLocation] IN ('l5', 'l6');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 5 AND [LoseLocation] IN ('l7', 'l8');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 5 AND [LoseLocation] IN ('l9', 'l10');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 6
                WHERE [Type] = 5 AND [LoseLocation] IN ('l11', 'l12');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 7
                WHERE [Type] = 5 AND [LoseLocation] IN ('l13', 'l14');

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 8
                WHERE [Type] = 5 AND [LoseLocation] IN ('l15', 'l16');

                -- Winners Round 2: Sequential markers continuing (9-16)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 9
                WHERE [Type] = 5 AND [LoseLocation] = 'l25';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 10
                WHERE [Type] = 5 AND [LoseLocation] = 'l26';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 11
                WHERE [Type] = 5 AND [LoseLocation] = 'l27';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 12
                WHERE [Type] = 5 AND [LoseLocation] = 'l28';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 13
                WHERE [Type] = 5 AND [LoseLocation] = 'l29';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 14
                WHERE [Type] = 5 AND [LoseLocation] = 'l30';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 15
                WHERE [Type] = 5 AND [LoseLocation] = 'l31';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 16
                WHERE [Type] = 5 AND [LoseLocation] = 'l32';

                -- Winners Round 3: Sequential markers continuing (17-20)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 17
                WHERE [Type] = 5 AND [LoseLocation] = 'l45';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 18
                WHERE [Type] = 5 AND [LoseLocation] = 'l46';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 19
                WHERE [Type] = 5 AND [LoseLocation] = 'l47';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 20
                WHERE [Type] = 5 AND [LoseLocation] = 'l48';

                -- Winners Round 4: Sequential markers continuing (21-22)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 21
                WHERE [Type] = 5 AND [LoseLocation] = 'l55';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 22
                WHERE [Type] = 5 AND [LoseLocation] = 'l56';

                -- Winners Round 5 (Semi-finals): Continues sequence (23)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 23
                WHERE [Type] = 5 AND [LoseLocation] = 'l60';

                -- Losers Round 1: Receives from Winners R1 (markers 1-8)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 1
                WHERE [Type] = 5 AND [Player1] = 'l1' AND [Player2] = 'l2' AND [WinLocation] = 'l17';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 2
                WHERE [Type] = 5 AND [Player1] = 'l3' AND [Player2] = 'l4' AND [WinLocation] = 'l18';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 3
                WHERE [Type] = 5 AND [Player1] = 'l5' AND [Player2] = 'l6' AND [WinLocation] = 'l19';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 4
                WHERE [Type] = 5 AND [Player1] = 'l7' AND [Player2] = 'l8' AND [WinLocation] = 'l20';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 5
                WHERE [Type] = 5 AND [Player1] = 'l9' AND [Player2] = 'l10' AND [WinLocation] = 'l21';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 6
                WHERE [Type] = 5 AND [Player1] = 'l11' AND [Player2] = 'l12' AND [WinLocation] = 'l22';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 7
                WHERE [Type] = 5 AND [Player1] = 'l13' AND [Player2] = 'l14' AND [WinLocation] = 'l23';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 8
                WHERE [Type] = 5 AND [Player1] = 'l15' AND [Player2] = 'l16' AND [WinLocation] = 'l24';

                -- Losers Round 2: Receives from Winners R2 (markers 9-16)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 9
                WHERE [Type] = 5 AND [Player1] = 'l17' AND [Player2] = 'l25' AND [WinLocation] = 'l33';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 10
                WHERE [Type] = 5 AND [Player1] = 'l18' AND [Player2] = 'l26' AND [WinLocation] = 'l34';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 11
                WHERE [Type] = 5 AND [Player1] = 'l19' AND [Player2] = 'l27' AND [WinLocation] = 'l35';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 12
                WHERE [Type] = 5 AND [Player1] = 'l20' AND [Player2] = 'l28' AND [WinLocation] = 'l36';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 13
                WHERE [Type] = 5 AND [Player1] = 'l21' AND [Player2] = 'l29' AND [WinLocation] = 'l37';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 14
                WHERE [Type] = 5 AND [Player1] = 'l22' AND [Player2] = 'l30' AND [WinLocation] = 'l38';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 15
                WHERE [Type] = 5 AND [Player1] = 'l23' AND [Player2] = 'l31' AND [WinLocation] = 'l39';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 16
                WHERE [Type] = 5 AND [Player1] = 'l24' AND [Player2] = 'l32' AND [WinLocation] = 'l40';

                -- Losers Round 3: No markers (internal losers bracket merging)

                -- Losers Round 4: Receives from Winners R3 (markers 17-20)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 17
                WHERE [Type] = 5 AND [Player1] = 'l41' AND [Player2] = 'l45' AND [WinLocation] = 'l49';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 18
                WHERE [Type] = 5 AND [Player1] = 'l42' AND [Player2] = 'l46' AND [WinLocation] = 'l50';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 19
                WHERE [Type] = 5 AND [Player1] = 'l43' AND [Player2] = 'l47' AND [WinLocation] = 'l51';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 20
                WHERE [Type] = 5 AND [Player1] = 'l44' AND [Player2] = 'l48' AND [WinLocation] = 'l52';

                -- Losers Round 5: No markers (internal losers bracket merging)

                -- Losers Round 6: Receives from Winners R4 (markers 21-22)
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 21
                WHERE [Type] = 5 AND [Player1] = 'l53' AND [Player2] = 'l55' AND [WinLocation] = 'l57';

                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 22
                WHERE [Type] = 5 AND [Player1] = 'l54' AND [Player2] = 'l56' AND [WinLocation] = 'l58';

                -- Losers Round 7: No markers (internal losers bracket merging)

                -- Losers Round 8 / Losers Finals: Receives from Winners R5 (marker 23) - l59,l60→l61
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = 23
                WHERE [Type] = 5 AND [Player1] = 'l59' AND [Player2] = 'l60' AND [WinLocation] = 'l61';

                -- Grand Finals: Trophy icon (-1) - w63,l61→w64
                UPDATE [dbo].[BracketPositions] SET [MarkerPosition] = -1
                WHERE [Type] = 5 AND [Player1] = 'w63' AND [Player2] = 'l61' AND [WinLocation] = 'w64';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkerPosition",
                table: "BracketPositions");
        }
    }
}
