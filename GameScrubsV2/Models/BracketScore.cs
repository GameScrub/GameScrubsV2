using GameScrubsV2.Enums;

namespace GameScrubsV2.Models;

public partial class BracketScore
{
	public int Id { get; set; }

	public BracketType Type { get; set; }

	public string? BracketPlace { get; set; }

	public int Score { get; set; }
}