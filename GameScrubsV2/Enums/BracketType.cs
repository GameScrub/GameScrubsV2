using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Enums;

public enum BracketType
{
	[Display(Name = "8 Single Elimination")]
	Single_8,
	[Display(Name = "16 Single Elimination")]
	Single_16,
	[Display(Name = "32 Single Elimination")]
	Single_32,
	[Display(Name = "8 Double Elimination")]
	Double_8,
	[Display(Name = "16 Double Elimination")]
	Double_16,
	[Display(Name = "32 Double Elimination")]
	Double_32
}