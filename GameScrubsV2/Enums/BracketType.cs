using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Enums;

public enum BracketType
{
	[Display(Name = "8 Single Elimination")]
	Single_8 = 0,
	[Display(Name = "16 Single Elimination")]
	Single_16 = 1,
	[Display(Name = "32 Single Elimination")]
	Single_32 = 2,
	[Display(Name = "8 Double Elimination")]
	Double_8 = 3,
	[Display(Name = "16 Double Elimination")]
	Double_16 = 4,
	[Display(Name = "32 Double Elimination")]
	Double_32 = 5
}