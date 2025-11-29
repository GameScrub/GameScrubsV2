using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Enums;

public enum CompetitionType
{
	[Display(Name = "Video Game")]
	VideoGames,
	[Display(Name = "Sport")]
	Sports,
	[Display(Name = "Board Game")]
	BoardGames,
	[Display(Name = "Other")]
	Other
}