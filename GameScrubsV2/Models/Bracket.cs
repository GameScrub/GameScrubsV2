using GameScrubsV2.Enums;

namespace GameScrubsV2.Models;

public sealed class Bracket
{
	public int Id { get; set; }

	public string? Name { get; set; }

	public string? Game { get; set; }

	public int? LockCode { get; set; }

	public bool IsLocked { get; set; }

	public BracketType Type { get; set; }

	public BracketStatus Status { get; set; }

	public CompetitionType Competition { get; set; }

	/// <summary>
	/// Double elimination only: when the losers bracket player wins the grand finals,
	/// a reset match is played since both players then have one loss.
	/// </summary>
	public bool GrandFinalReset { get; set; }

	public DateTime StartDate { get; set; }

	public DateTime CreatedDate { get; set; }

	public ICollection<Placement> Placements { get; set; } = new List<Placement>();

	public ICollection<PlayerList> PlayerLists { get; set; } = new List<PlayerList>();
}