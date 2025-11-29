using System.ComponentModel.DataAnnotations;

namespace GameScrubsV2.Enums;

public enum BracketStatus
{
	[Display(Name = "Setup")]
	Setup,
	[Display(Name = "In Progress")]
	Started,
	[Display(Name = "On Hold")]
	OnHold,
	[Display(Name = "Completed")]
	Completed
}