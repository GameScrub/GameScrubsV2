using GameScrubsV2.Enums;

namespace GameScrubsV2.Models;

public partial class Placement
{
	public int Id { get; set; }

	public string? PlayerName { get; set; }

	public string? BracketPlace { get; set; }

	public int BracketId { get; set; }

	public int Score { get; set; }

	public PlacementStatus Status { get; set; }

	public string? PreviousBracketPlace { get; set; }

	public bool IsTop { get; set; }

	public virtual Bracket Bracket { get; set; } = null!;
}